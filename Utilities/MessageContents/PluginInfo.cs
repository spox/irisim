
namespace IrisIM
{
	namespace Utilities
	{
		public class PluginInfo
		{
			public string name;
			public string version;
			public string hash;
			public string required_name;
			public string required_version;
			
			public PluginInfo()
			{
				this.hash = null;
				this.name = null;
				this.required_name = null;
				this.required_version = null;
				this.version = null;
			}
		}
	}
}