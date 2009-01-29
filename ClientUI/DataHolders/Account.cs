
namespace IrisIM
{
	namespace UI
	{
		class Account
		{
			private string _ID;
			private string _password;
			private string _alias;
			
			public Account(string id, string password, string alias)
			{
				this._alias = alias;
				this._ID = id;
				this._password = password;
			}
			
			public string ID
			{
				get{ return this._ID; }
				set{ this._ID = value; }
			}
			
			public string password
			{
				get{ return this._password; }
				set{ this._password = value; }
			}
			
			public string alias
			{
				get{ return this._alias; }
				set{ this._alias = value; }
			}
		}
	}
}