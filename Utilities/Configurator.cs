using System;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;

namespace IrisIM
{
	namespace Utilities
	{
		public class Configurator
		{
			private Hashtable _settings;
			
			public Hashtable settings
			{
				get{ return this._settings; }
			}
			
			public Configurator()
			{
				this.build_hash();
			}
			
			private void build_hash()
			{
				this._settings = new Hashtable();
				string[] keys = null;
				NameValueCollection settings = ConfigurationSettings.AppSettings;
				keys = settings.AllKeys;
				foreach(string key in keys)
				{
					this._settings[key] = settings[key];
				}
			}
			
			public string get_value(string key)
			{
				if(this._settings.ContainsKey(key))
				{
					return (string)this._settings[key];
				}
				else
				{
					throw new ItemNotFound(key);
				}
			}
			
			public void set_value(string key, string val)
			{
				this._settings[key] = val;
			}
			
			public short get_asShort(string key)
			{
				try
				{
					return Convert.ToInt16(this.get_value(key));
				}
				catch(InvalidCastException e)
				{
					throw new FailedConversion("Failed to convert "+key+" to Int16.");
				}
			}
			
			public int get_asInt(string key)
			{
				try
				{
					return Convert.ToInt32(this.get_value(key));
				}
				catch(InvalidCastException e)
				{
					throw new FailedConversion("Failed to convert "+key+" to Int32.");
				}
			}
			
			public long get_asLong(string key)
			{
				try
				{
					return Convert.ToInt64(this.get_value(key));
				}
				catch(InvalidCastException e)
				{
					throw new FailedConversion("Failed to convert "+key+" to Int64.");
				}
			}
			
			public bool exists(string key)
			{
				return this._settings.ContainsKey(key);
			}
		}
	}
}