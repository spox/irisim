/** Currently not using this anymore. 
    May bring it back later. Also will probably
    use it for generating the encryption keys
    to get rid of the stall when starting
    the application **/


using System;
using IrisIM.Client;
using IrisIM.Utilities;

using Glade;
using Gtk;

namespace IrisIM
{
	namespace UI
	{
		public class ConnectionStatus
		{
			private Glade.XML _gxml;
			
			[Glade.Widget] Dialog connectionStatusDialog;
			[Glade.Widget] ProgressBar connectionProgressBar;
			[Glade.Widget] Label connectionStatusText;
			
			public ConnectionStatus(Controller controller, string username, string password, string servername, string port)
			{
				Logger.log("Adding UI event processor", Logger.Verbosity.moderate);
				controller.message_pump.ui_event += new UIEvent(this.Processor);
				this._gxml = new Glade.XML(null, "irisim.glade", "connectionStatusDialog", null);
				this._gxml.Autoconnect(this);
				this.connectionProgressBar.Fraction = 0.2;
				this.SendConnectionMessage(controller, username, password, servername, port);
			}

			private void SendConnectionMessage(Controller controller, string username, string password, string servername, string port)
			{
				ClientMessage message = new ClientMessage("connect");
				message.Add("server", servername);
				message.Add("port", port);
				message.Add("username", username);
				message.Add("password", password);
				controller.message_pump.process_message(message);
			}
			
			public void Processor(UIMessage message)
			{
				if(message.type == "connectionStatus")
				{
					string status = message.Get("status");
					if(status == "Failed" || status == "Connected")
					{
						this.connectionProgressBar.Fraction = 1.0;
					}
					this.connectionStatusText.Text = status;
				}
			}
			
			public void on_closeButton_clicked(System.Object o, EventArgs e)
			{
				this.connectionStatusDialog.Destroy();
			}
		}
	}
}