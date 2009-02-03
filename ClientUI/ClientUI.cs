using System.Xml;
using IrisIM.Client;
using IrisIM.Utilities;
using System.Windows.Forms;
namespace IrisIM
{
	namespace UI
	{
		class ClientUI
		{
			public static void Main(string[] args)
			{
				Logger.loudness = Logger.Verbosity.moderate;
				System.Windows.Forms.Application.Run(new FormBase());
			}
		}
	}
}