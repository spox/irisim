using System.Xml;
using IrisIM.Client;
using IrisIM.Utilities;

namespace IrisIM
{
	namespace UI
	{
		class ClientUI
		{
			public static void Main(string[] args)
			{
				Logger.loudness = Logger.Verbosity.moderate;
				new UIBase();
			}
		}
	}
}