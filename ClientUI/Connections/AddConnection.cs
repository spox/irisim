using System;
using System.Collections;

using Glade;
using Gtk;
using GtkSharp;

namespace IrisIM
{
	namespace UI
	{
		class AddConnection
		{
			private ConnectionCollection connections;
			private Glade.XML gxml;
			private Connections cons;
			
			[Glade.Widget] Dialog addConnectionDialog;
			[Glade.Widget] Entry connectionNameEntry;
			[Glade.Widget] Entry serverNameEntry;
			[Glade.Widget] SpinButton portEntry;
			
			public AddConnection(ConnectionCollection collection, Connections cons)
			{
				this.cons = cons;
				this.connections = collection;
				this.gxml = new Glade.XML(null, "irisim.glade", "addConnectionDialog", null);
				this.gxml.Autoconnect(this);
			}
			
			public void on_okButton_clicked(System.Object o, EventArgs e)
			{
				string conName = this.connectionNameEntry.Text;
				string serName = this.serverNameEntry.Text;
				string port = this.portEntry.Text;
				if(conName == "")
				{
					new Error("You must specify a name for the connection.");
				}
				else if(serName == "")
				{
					new Error("You must specify a server to connect to.");
				}
				else if(port == "")
				{
					new Error("You must specify a port to connect to.");
				}
				else
				{
					int portnum = Convert.ToInt32(port);
					try
					{
						Connection con = new Connection(conName, serName, portnum);
						this.connections.Add(con);
						this.cons.AddNewConnectionName(conName);
						this.addConnectionDialog.Destroy();
					}
					catch(OutOfRangeException ex)
					{
						new Error(ex.Message);
					}
					catch(KeyExistsException ex)
					{
						new Error("Connection name is already in use.\nPease choose another name.");
					}
				}
			}
			
			public void on_cancelButton_clicked(System.Object o, EventArgs e)
			{
				this.addConnectionDialog.Destroy();
			}
		}
	}
}