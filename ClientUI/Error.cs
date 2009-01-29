// created on 5/12/2006 at 5:17 AM

using System;

using Glade;
using Gdk;
using Gtk;
using GtkSharp;

namespace IrisIM
{
	namespace UI
	{
		class Error
		{
		
			private Glade.XML gxml;
			
			[Glade.Widget] Label errorMessage;
			[Glade.Widget] Dialog errorDialog;
		
			public Error(string message)
			{
				this.gxml = new Glade.XML(null, "irisim.glade", "errorDialog", null);
				this.gxml.Autoconnect(this);
				this.errorDialog.Resizable = true;
				this.errorDialog.AllowGrow = true;
				this.errorMessage.Text = message;
				this.errorDialog.Show();
			}
			
			public void on_okButton_clicked(System.Object o, EventArgs e)
			{
				this.errorDialog.Destroy();
			}
		}
	}
}