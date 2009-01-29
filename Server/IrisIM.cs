// project created on 1/3/2006 at 11:49 AM
using System;
using IrisIM.Utilities;

namespace IrisIM
{
	namespace Server
	{
		class IrisIM
		{
			public static void Main(string[] args)
			{
				Logger.loudness = Logger.Verbosity.moderate;
				Controller controller = new Controller();
				controller.online();
			}
		}
	}
}