// Connections dialog class

using System;
using System.Collections;

using Glade;
using Gtk;
using GtkSharp;
using IrisIM.Client;
using IrisIM.Utilities;

namespace IrisIM
{
	namespace UI
	{
		class Connections
		{
		
			private Glade.XML _gxml;
			private ConnectionCollection _connectionCollection;
			private ListStore _connectionStore;
			private string _connectionSelected;
			private ListStore _accountStore;
			private string _accountSelected;
			private AccountCollection _accountCollection;
			private UIBase _ui_base;
			
			[Glade.Widget] Dialog connectionsDialog;
			[Glade.Widget] Notebook connectionsBook;
			[Glade.Widget] TreeView connectionsListView;
			
			[Glade.Widget] TreeView accountsListView;
			[Glade.Widget] Entry idEntry;
			[Glade.Widget] Entry passwordEntry;
			[Glade.Widget] Entry aliasEntry;
			[Glade.Widget] ComboBox accountComboBox;
		
			public Connections(ConnectionCollection c, AccountCollection a, UIBase base_window)
			{
				this._ui_base = base_window;
				this._connectionCollection = c;
				this._accountCollection = a;
				this._gxml = new Glade.XML(null, "irisim.glade", "connectionsDialog", null);
				this._gxml.Autoconnect(this);
				this.connectionsBook.Page = 0;
				
				this.connectionsListView.FixedHeightMode = false;
				this.connectionsListView.HeadersVisible = true;
				this.connectionsListView.AppendColumn("Connection", new CellRendererText(), "text", 0);
				this._connectionStore = new ListStore(typeof(string), typeof(string));
				this.connectionsListView.Model = this._connectionStore;
				this.PopulateConnectionList();
				this.connectionsListView.ShowAll();
				
				this.accountsListView.FixedHeightMode = false;
				this.accountsListView.HeadersVisible = true;
				this.accountsListView.AppendColumn("Accounts", new CellRendererText(), "text", 0);
				this._accountStore = new ListStore(typeof(string), typeof(string));
				this.accountsListView.Model = this._accountStore;
				
				this.accountComboBox.ShowAll();
				
				this.PopulateAccountList();
				this.accountsListView.ShowAll();
				
				this._connectionSelected = "";
				this._accountSelected = "";
		
				this.connectionsListView.Selection.Changed += this.on_connectionsListView_changed;
				this.accountsListView.Selection.Changed += this.on_accountsListView_changed;
			}
		
			private void on_connectionsListView_changed(System.Object o, EventArgs e)
			{
				TreeIter iter;
				TreeModel model;
				if(((TreeSelection)o).GetSelected(out model, out iter))
				{
					this._connectionSelected = (string)model.GetValue(iter, 0);
				}
			}
		
			public void on_connectionAddButton_clicked(System.Object obj, EventArgs e)
			{
				new AddConnection(this._connectionCollection, this);
			}
			
			public void on_connectionEditButton_clicked(System.Object o, EventArgs e)
			{
				if(this._connectionSelected.Length > 0)
				{
					new EditConnection(this._connectionCollection, this._connectionSelected);
				}
			}
			
			public void on_connectionDeleteButton_clicked(System.Object o, EventArgs e)
			{
				if(this._connectionSelected.Length > 0)
				{
					this._connectionCollection.Remove(this._connectionCollection.Get(this._connectionSelected));
					this._connectionSelected = "";
					this.PopulateConnectionList();
				}
			}
			
			public void on_okButton_clicked(System.Object o, EventArgs e)
			{
				this.connectionsDialog.Destroy();
			}
			
			public void on_connectionStore_changed(System.Object o, EventArgs e)
			{
				this.PopulateConnectionList();
			}
			
			public void PopulateConnectionList()
			{
				this._connectionStore.Clear();
				if(this._connectionCollection.Names().Count > 0)
				{
					foreach(string con in this._connectionCollection.Names())
					{
						System.Console.WriteLine("Connection name: "+con);
						this._connectionStore.AppendValues(con);
					}
				}
			}
			
			public void AddNewConnectionName(string name)
			{
				this._connectionStore.AppendValues(name);
			}
			
			/****** Account pane stuff ***********/

			private void on_accountsListView_changed(System.Object o, EventArgs e)
			{
				TreeIter iter;
				TreeModel model;
				if(((TreeSelection)o).GetSelected(out model, out iter))
				{
					this._accountSelected = (string)model.GetValue(iter, 0);
					this.UpdateAccountFields();
				}
			}
			
			public void PopulateAccountList()
			{
				this._accountStore.Clear();
				//this.accountComboBox.Clear();
				if(this._accountCollection.Names().Count > 0)
				{
					foreach(string acct in this._accountCollection.Names())
					{
						System.Console.WriteLine("Account name: "+acct);
						this._accountStore.AppendValues(acct);
						this.accountComboBox.AppendText(acct);
					}
				}
			}
			
			public void on_accountAddButton_clicked(System.Object o, EventArgs e)
			{
				string id = this.idEntry.Text;
				string password = this.passwordEntry.Text;
				string alias = this.aliasEntry.Text;
				if(id == "")
				{
					new Error("You must specify an identification\nname for this account.");
				}
				else if(password == "")
				{
					new Error("You must specify a password for this account.");
				}
				else if(alias == "")
				{
					new Error("You must specify an alias for this account.");
				}
				else
				{
					try
					{
						Account a = new Account(id, password, alias);
						this._accountCollection.Add(a);
					}
					catch(OutOfRangeException ex)
					{
						new Error(ex.Message);
					}
					catch(KeyExistsException ex)
					{
						new Error("Account identification name is already in\nuse. Please choose another name.");
					}
					this.ClearAccountFields();
					this.PopulateAccountList();
					this._accountSelected = "";
				}
			}
			
			public void on_accountDeleteButton_clicked(System.Object o, EventArgs e)
			{
				Account current = null;
				if(this._accountSelected == "")
				{
					new Error("Please select an account to delete.");
					return;
				}
				try
				{
					current = this._accountCollection.Get(this._accountSelected);
				}
				catch(NotFoundException ex)
				{
					new Error("Please select an account to delete.");
					return;
				}
				this._accountCollection.Remove(current);
				this.ClearAccountFields();
				this.PopulateAccountList();
				this._accountSelected = "";
			}
			
			public void on_accountSaveButton_clicked(System.Object o, EventArgs e)
			{
				Account current = null;;
				if(this._accountSelected == "")
				{
					new Error("Please select an account to save to.");
					return;
				}
				try
				{
					current = this._accountCollection.Get(this._accountSelected);
				}
				catch(NotFoundException ex)
				{
					new Error("Please select an account to save to.");
					return;
				}
				string id = this.idEntry.Text;
				string password = this.passwordEntry.Text;
				string alias = this.aliasEntry.Text;
				if(id == "")
				{
					new Error("You must specify an identification\nname for this account.");
				}
				else if(password == "")
				{
					new Error("You must specify a password for this account.");
				}
				else if(alias == "")
				{
					new Error("You must specify an alias for this account.");
				}
				else
				{
					try
					{
						Account a = new Account(id, password, alias);
						this._accountCollection.Remove(current);
						this._accountCollection.Add(a);
					}
					catch(OutOfRangeException ex)
					{
						new Error(ex.Message);
					}
					catch(KeyExistsException ex)
					{
						new Error("Account identification name is already in\nuse. Please choose another name.");
					}
				}
				this.ClearAccountFields();
				this.PopulateAccountList();
				this._accountSelected = "";
			}
			
			private void ClearAccountFields()
			{
				this.idEntry.Text = "";
				this.passwordEntry.Text = "";
				this.aliasEntry.Text = "";
			}
			
			private void UpdateAccountFields()
			{
				Account acct = this._accountCollection.Get(this._accountSelected);
				this.idEntry.Text = acct.ID;
				this.passwordEntry.Text = acct.password;
				this.aliasEntry.Text = acct.alias;
			}
			
			
			/** Connect button **/
			
			public void on_connectButton_clicked(System.Object o, EventArgs e)
			{
				Logger.log("Connect button clicked.", Logger.Verbosity.moderate);
				try
				{
					string accountKey = this.accountComboBox.ActiveText;
					Account acct = this._accountCollection.Get(accountKey);
					string password = acct.password;
					string username = acct.ID;
					Connection con = this._connectionCollection.Get(this._connectionSelected);
					string servername = con.serverName;
					string port = con.port.ToString();
					this.SendLogin(username, password, servername, port);
					this.connectionsDialog.Destroy();
					return;
				}
				catch(Exception ex)
				{
					Logger.log("Connect exception: "+ex.Message+"\n"+ ex.StackTrace , Logger.Verbosity.moderate);
				}
			}
			
			private void SendLogin(string username, string password, string servername, string port)
			{
				ClientMessage message = new ClientMessage("connect");
				message.Add("server", servername);
				message.Add("port", port);
				message.Add("username", username);
				message.Add("password", password);
				this._ui_base.controller.message_pump.process_message(message);				
			}
		}
	}
}