// created on 1/3/2006 at 11:55 AM

using System;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using System.Threading;

using IrisIM.Utilities;

namespace IrisIM
{
	namespace Server
	{
		public class Listener
		{
		
			private static int _default_port = 9090;
		
			private int _port;
			private TcpListener _listener;
			private Thread _worker;
			private bool _listen;
			private ArrayList _connections;
			private event NewConnection _connection_announcement;
			
			public event NewConnection connection_announcement
			{
				add{ this._connection_announcement += value; }
				remove{ this._connection_announcement -= value; }
			}
			
			public ArrayList connections
			{
				get
				{
					return this.connections;
				}
			}
			
			public int port
			{
				get
				{
					return this._port;
				}
				set
				{
					if(value > 65335)
					{
						Logger.log("Port number exceeded maximum port number. Setting to default port number.", Logger.Verbosity.moderate);
						this._port = -1;
						throw new IrisIMException("Port value exceeds maximum valid port number.");
					}
					this._port = value;
				}
			}
			
			public Listener(ArrayList connections)
			{
				this._port = -1;
				this._connections = connections;
				initialize();
			}
			
			public Listener(int port_number, ArrayList connections)
			{
				this._connections = connections;
				try{ this.port = port_number; }
				catch(Exception e){ /*ignore*/}
				initialize();
			}
			
			private void initialize()
			{
				if(this._port == -1)
				{
					this._port = Listener._default_port;
				}
				this._listen = false;
				this._worker = null;
			}

	        public void start()
	        {
	           if(!this._listen)
	           {
	              this._listen = true;
	              this.start_listening();
	           }
	        }
	        
	        public void stop()
	        {
	           if(this._listen)
	           {
	              this._listen = false;
	              this.stop_listening();
	           }
	        }
	        
	        private void start_listening()
	        {
	        	this._listener = new TcpListener(this._port);
	        	Logger.log("New listener has been attached to port ("+this._port+")", Logger.Verbosity.quiet);
	        	this._worker = new Thread(new ThreadStart(this.do_listen));
	        	this._worker.Start();
	        	Logger.log("Listener attached to port "+this._port+" has been activated.", Logger.Verbosity.moderate);
	        }
	        
	        private void stop_listening()
	        {
	        	this._listen = false;
	        	this._worker.Join();
	        	this._listener.Stop();
	        	this._worker = null;
	        	this._listener = null;
	        	Logger.log("Listener has been deactivated and dettached from port "+this._port, Logger.Verbosity.quiet);
	        }
	        
	        private void do_listen()
	        {
	        	Socket holder;
	        	Transceiver connection;
	        	this._listener.Start();
	        	while(this._listen)
	        	{
	        		holder = this._listener.AcceptSocket();
	        		connection = new Transceiver(holder);
	        		Logger.log("Listener attached to port "+this._port+" has accepted a connection.", Logger.Verbosity.moderate);
	        		this._connections.Add(connection);
	        		this._connection_announcement(connection);
	        	}
	        }
	     }
	}
}