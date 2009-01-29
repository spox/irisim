using System;
using Gtk;
using Glade;

namespace IrisIM
{
	namespace UI
	{
		class EditConnection
		{
			private ConnectionCollection _connection_collection;
			private string _edit_key;
			private Glade.XML _gxml;
			
			[Glade.Widget] Dialog editConnectionDialog;
			[Glade.Widget] Entry serverNameEntry;
			[Glade.Widget] SpinButton portSpinButton;
			[Glade.Widget] Label connectionNameText;
			
			public EditConnection(ConnectionCollection con, string key)
			{
				this._connection_collection = con;
				this._edit_key = key;
				this._gxml = new Glade.XML(null, "irisim.glade", "editConnectionDialog", null);
				this._gxml.Autoconnect(this);
				this.PopulateFields();
			}
			
			private void PopulateFields()
			{
				Connection con = this._connection_collection.Get(this._edit_key);
				this.serverNameEntry.Text = con.serverName;
				this.portSpinButton.Text = con.port.ToString();
				this.connectionNameText.Text = con.connectionName;
			}
			
			public void on_okButton_clicked(System.Object o, EventArgs e)
			{
				Connection con = this._connection_collection.Get(this._edit_key);
				con.port = Convert.ToInt32(this.portSpinButton.Text);
				con.serverName = this.serverNameEntry.Text;
				this._connection_collection.Modify(con);
				this.editConnectionDialog.Destroy();
			}
			
			public void on_cancelButton_clicked(System.Object o, EventArgs e)
			{
				this.editConnectionDialog.Destroy();
			}
			
			
		}
	}
}