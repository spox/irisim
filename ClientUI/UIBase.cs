using System;
using System.Collections;
using Glade;
using Gtk;
using IrisIM.Client;
using IrisIM.Utilities;

namespace IrisIM
{
	namespace UI
	{
		class UIBase
		{
		
			private Glade.XML gxml;
			private ConnectionCollection _user_connections;
			private AccountCollection _user_accounts;
			private Controller _controller;
			private Connections _connections;
			private ConnectionStatus _connection_status;
			private ListStore _chat_list_store;
			private Hashtable _user_chats;
			private string _current_chat;
		
			[Glade.Widget] Window chatWindow;
			[Glade.Widget] TextView conversationView;
			[Glade.Widget] Label connectionLabel;
			[Glade.Widget] Label encryptionLabel;
			[Glade.Widget] Label lagLabel;
			[Glade.Widget] TreeView chatListView;
			[Glade.Widget] Entry userTextEntry;
			
			public Controller controller
			{
				get{ return this._controller; }
			}
			
			public UIBase()
			{
				this._controller = new Controller();
				this._controller.message_pump.ui_event += new UIEvent(this.UpdateConnectionStatus);
				this._controller.message_pump.ui_event += new UIEvent(this.UpdateEncryptionStatus);
				this._controller.message_pump.ui_event += new UIEvent(this.ErrorWatch);
				this._controller.message_pump.ui_event += new UIEvent(this.MessageWatch);
				this._chat_list_store = new ListStore(typeof(string), typeof(string));
				this._user_connections = new ConnectionCollection();
				this._user_accounts = new AccountCollection();
				this._user_chats = new Hashtable();
				this._current_chat = "";
				Application.Init();
				this.gxml = new Glade.XML(null, "irisim.glade", "chatWindow", null);
				this.gxml.Autoconnect(this);
				this.chatListView.FixedHeightMode = false;
				this.chatListView.HeadersVisible = true;
				this.chatListView.AppendColumn("Chats", new CellRendererText(), "text", 0);
				this.chatListView.Model = this._chat_list_store;
				this.chatListView.Selection.Changed += this.on_chatListView_changed;
				this.chatListView.ShowAll();
				
				Application.Run();
			}
			
			~UIBase()
			{
			}
			
			private void Cleanup()
			{
				try{ this._controller.message_pump.connection.Close(); }
				catch(Exception e)
				{
					Logger.log("Message Pump error message: "+e.Message, Logger.Verbosity.moderate);
				}
			}
			
			// Events //
			
			public void on_quit_activate(System.Object o, EventArgs e)
			{
				this.Cleanup();
				Application.Quit();
			}
			
			public void on_copy_activate(System.Object o, EventArgs e)
			{
				
			}
			
			public void on_cut_activate(System.Object o, EventArgs e)
			{
				
			}
			
			public void on_paste_activate(System.Object o, EventArgs e)
			{
				
			}
			
			public void on_delete_activate(System.Object o, EventArgs e)
			{
				
			}
			
			public void on_connections_activate(System.Object o, EventArgs e)
			{
				this._connections = new Connections(this._user_connections, this._user_accounts, this);
			}
		
			public void on_about_activate(System.Object o, EventArgs e)
			{
				this.gxml = new Glade.XML(null, "irisim.glade", "aboutDialog", null);
				gxml.Autoconnect(this);
			}
			
			public void on_chat_activate(System.Object o, EventArgs e)
			{
				new UserChat(this._controller);
			}

			public void on_chatWindow_destroy(System.Object o, EventArgs e)
			{
				this.Cleanup();
				Application.Quit();
			}
			
			public void on_userTextEntry_activate(System.Object o, EventArgs e)
			{
				if(this._current_chat == "")
				{
					Application.Invoke(delegate{ new Error("You must start a chat first."); });
				}
				else
				{
					ClientMessage message = new ClientMessage("userMessage");
					message.Add("destination", this._current_chat);
					message.Add("message", this.userTextEntry.Text);
					message.Add("time", DateTime.Now.ToString());
					this._controller.message_pump.process_message(message);
					TextBuffer t = (TextBuffer)this._user_chats[this._current_chat];
					t.Text += DateTime.Now.ToString() +" (you): "+this.userTextEntry.Text+"\n";
					this._user_chats[this._current_chat] = t;
					this.userTextEntry.Text = "";
				}
			}
			
			/*** All functionality of the actual window ***/
			
			private void on_chatListView_changed(System.Object o, EventArgs e)
			{
				TreeIter iter;
				TreeModel model;
				if(((TreeSelection)o).GetSelected(out model, out iter))
				{
					this._current_chat = (string)model.GetValue(iter, 0);
					this.UpdateView();
				}
			}
			
			private void UpdateView()
			{
				this.conversationView.Buffer = (TextBuffer)this._user_chats[this._current_chat];
			}
			
			/** Watchers **/
			
			public void UpdateConnectionStatus(UIMessage message)
			{
				if(message.type == "connectionStatus")
				{
					Application.Invoke(delegate{ this.connectionLabel.Text = message.Get("status"); });
				}
			}
			
			public void UpdateEncryptionStatus(UIMessage message)
			{
				if(message.type == "encryptionStatus")
				{
					Application.Invoke(delegate{ this.encryptionLabel.Text = message.Get("status"); });
				}
				
			}
			
			public void ErrorWatch(UIMessage message)
			{
				if(message.type == "error")
				{
					Application.Invoke(delegate{ new Error(message.Get("message")); });
				}
			}
			
			public void MessageWatch(UIMessage message)
			{
				if(message.type == "startChat")
				{
					if(this._user_chats.ContainsKey(message.Get("username")))
					{
						Application.Invoke(delegate{ new Error("Conversation is already running with "+message.Get("username"));});
					}
					else
					{
						Logger.log("Added new user: "+message.Get("username"), Logger.Verbosity.moderate);
						this._user_chats[message.Get("username")] = new TextBuffer(null);
						Application.Invoke(delegate{ try{this._chat_list_store.AppendValues(message.Get("username"));}
						catch(Exception e){ Logger.log("Failed: "+e.Message+"\n"+e.StackTrace, Logger.Verbosity.moderate);}});
						if(this._current_chat == "")
						{
							this._current_chat = message.Get("username");
							Application.Invoke(delegate{ this.conversationView.Buffer = (TextBuffer)this._user_chats[this._current_chat]; });
						}
					}
				}
				else if(message.type == "chatMessage")
				{
				
					Application.Invoke(
					delegate{
						try{
						string key = message.Get("originator");
						string user_message = message.Get("message");
						TextBuffer t = (TextBuffer)this._user_chats[key];
						t.Text += DateTime.Now.ToString() +" "+ this._current_chat+": "+user_message+"\n";
						this._user_chats[key] = t;
						}
						catch(Exception e)
						{
							Logger.log("Processing exception: "+e.Message, Logger.Verbosity.moderate);
						}
					});
				}
			}
		}
	}
}