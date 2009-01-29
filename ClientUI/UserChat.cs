
using Gtk;
using Glade;

using IrisIM.Client;
using IrisIM.Utilities;

using System;

namespace IrisIM
{
	namespace UI
	{
		public class UserChat
		{
		
			private Glade.XML _gxml;
			private Controller _controller;
		
			[Glade.Widget] Dialog chatDialog;
			[Glade.Widget] Entry idEntry;
		
			public UserChat(Controller controller)
			{
				this._controller = controller;
				this._gxml = new Glade.XML(null, "irisim.glade", "chatDialog", null);
				this._gxml.Autoconnect(this);
				this.chatDialog.Show();
			}
			
			public void on_cancelButton_clicked(System.Object o, EventArgs e)
			{
				this.chatDialog.Destroy();
			}
			
			public void on_okButton_clicked(System.Object o, EventArgs e)
			{
				string username = this.idEntry.Text;
				ClientMessage message = new ClientMessage("userRequest");
				message.Add("username", username);
				this._controller.message_pump.process_message(message);
				this.chatDialog.Destroy();
			}
		}
	}
}