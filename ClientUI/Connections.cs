using System;
using System.Collections;
using System.Windows.Forms;

using IrisIM.Client;
using IrisIM.Utilities;

namespace IrisIM
{
	namespace UI
	{
		class Connections
		{
			private ConnectionCollection _connectionCollection;
			private ListStore _connectionStore;
			private string _connectionSelected;
			private ListStore _accountStore;
			private string _accountSelected;
			private AccountCollection _accountCollection;
			private FormBase _ui_base;
			
			public Connections(ConnectionCollection c, AccountCollection a, FormBase base_window)
			{
				this._ui_base = base_window;
				this._connectionCollection = c;
			}
		}
	}
}