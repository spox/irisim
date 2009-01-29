using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;
using System.Net.Sockets;

namespace IrisIM
{
	namespace Utilities
	{
		public class Transceiver
		{
			private Socket _connection;
			private Thread _reader;
			private bool _alive;
			
			private event WriterOverload _plugin_writers;
			private event MessageProcessor _message_processor;
			private event Disconnection _disconnection;
			private ReaderOverload _plugin_readers;
			private WriteFilter _filter_writer;
			
			public Socket connection
			{
				get{ return this._connection; }
			}
			
			public bool alive
			{
				get{ return this._alive; }
			}
			
			public event WriterOverload plugin_writers
			{
				add{ this._plugin_writers += value; }
				remove{ this._plugin_writers -= value; }
			}
			
			public event MessageProcessor message_processor
			{
				add{ this._message_processor += value; }
				remove{ this._message_processor -= value; }
			}
			
			public event Disconnection disconnection
			{
				add{ this._disconnection += value; }
				remove{ this._disconnection += value; }
			}
		
			public ReaderOverload plugin_readers
			{
				get{ return this._plugin_readers; }
				set{ this._plugin_readers = value; }
			}
			
			public WriteFilter filter_writer
			{
				get{ return this._filter_writer; }
				set{ this._filter_writer = value; }
			}
		
			public Transceiver()
			{}
		
			public Transceiver(string server, int port)
			{
				this._connection = this.EstablishConnection(server, port);
				this.initialize();
			}
		
			public Transceiver(Socket connection)
			{
				this._connection = connection;
				this.initialize();
			}
			
			private void initialize()
			{
				this._alive = true;
				this._reader = new Thread(new ThreadStart(this.Reader));
				this._reader.Start();
			}
			
			public void Connect(string server, int port)
			{
				this._connection = this.EstablishConnection(server, port);
				this.initialize();
			}
			
			private Socket EstablishConnection(string server, int port)
			{
				Socket sock = null;
				IPEndPoint end_point;
				IPAddress host_addr = null;
				IPHostEntry host_info = Dns.Resolve(server);
				IPAddress[] server_ips = host_info.AddressList;
				for(int i = 0; i < server_ips.Length; i++)
				{
					host_addr = server_ips[i];
					end_point = new IPEndPoint(host_addr, port);
					sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					sock.Connect(end_point);
					if(sock.Connected)
					{
						break;
					}
					else
					{
						sock = null;
					}
				}
				if(sock == null)
				{
					throw new FailedConnection("Failed to establish connection.");
				}
				return sock;
			}
			
			private void Reader()
			{
				int buffer_size = 1024;
				int received = 0;
				byte[] buffer = new byte[buffer_size];
				string message_string = "";
				Message message = null;
				IPEndPoint userinfo = null;
				while(this._alive)
				{
					try
					{
						userinfo = (IPEndPoint)this._connection.RemoteEndPoint;
						Logger.log("Connection alive from: " + userinfo.Address.ToString() +" Code: "+ this.GetHashCode(), Logger.Verbosity.loud);
					}
					catch(Exception e)
					{
						Logger.log("Connection is alive but not connected. Code: "+this.GetHashCode(), Logger.Verbosity.loud);
					}
				
					if(this._connection.Poll(-1, SelectMode.SelectRead))
					{
						message_string = "";
						message = null;
						received = 0;
						if(this._connection.Available > 1)
						{
							while(received < this._connection.Available)
							{
								received += this._connection.Receive(buffer);
								message_string += Encoding.Unicode.GetString(buffer);
							}
							try
							{
								message = new Message(this, this._plugin_readers(this, message_string));
							}
							catch(Exception e)
							{
								message = new Message(this, message_string);
							}
							try
							{
								Logger.log("Transceiver: Message received: "+message.DumpMessage(), Logger.Verbosity.moderate);
								this._message_processor(message);
								Logger.log("Transceiver: Received message and properly processed. ("+this.GetHashCode()+")", Logger.Verbosity.moderate);
							}
							catch(Exception e)
							{
								Logger.log("Failed to process received message. "+e.Message+"\n"+e.StackTrace+" "+message_string, Logger.Verbosity.moderate);
							}
						}
						else
						{
							Logger.log("Connection to client lost. Code: "+this.GetHashCode(), Logger.Verbosity.moderate);
							this._alive = false;
							// Notification needs to be done using a thread so we
							// don't end up in a deadlock.
							(new Thread(new ThreadStart(this.NotifyDisconnection))).Start();
						}
					}
					if(!this._connection.Connected)
					{
						Logger.log("Connection is no longer open.", Logger.Verbosity.moderate);
						break;
					}
				}
			}
			
			public void Write(string message)
			{
				try
				{
					this._plugin_writers(this, message);
				}
				catch(Exception e)
				{
					this._connection.Send(Encoding.Unicode.GetBytes(message));
				}
			}
			
			public void Write(Message message)
			{
				try
				{
					try
					{
						string mod_message = this.filter_writer(message);
						this._plugin_writers(this, mod_message);
					}
					catch
					{
						this._plugin_writers(this, message.DumpMessage());
					}
				}
				catch
				{
					this._connection.Send(Encoding.Unicode.GetBytes(message.DumpMessage()));
				}
			}
			
			public void WriteBytes(byte[] bytes)
			{
				this._connection.Send(bytes);
			}
			
			public void Close()
			{
				Logger.log("Received close command to close transceiver connection. Code: "+ this.GetHashCode(), Logger.Verbosity.loud);
				this._alive = false;
				this._connection.Close();
				Logger.log("Waiting for reader thread to close on transceiver connection. Code: "+this.GetHashCode(), Logger.Verbosity.moderate);
				this._reader.Join();
				Logger.log("Reader thread has closed on transceiver connection. Code: "+this.GetHashCode(), Logger.Verbosity.moderate);
				this._reader = null;
			}
			
			private void NotifyDisconnection()
			{
				this._disconnection(this);
			}
		}
	}
}