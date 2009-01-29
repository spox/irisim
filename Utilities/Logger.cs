namespace IrisIM
{
	namespace Utilities
	{
		public class Logger
		{
			public enum Verbosity{silent = 0, quiet, moderate, loud, yelling, screaming}
			public enum LogType{console = 0, file, syslog}
			
			public static Verbosity loudness;
			public static LogType type;
			
			public static void log(string message, Verbosity level)
			{
				if(Logger.loudness == Verbosity.silent)
				{
					return;
				}
				if(level <= Logger.loudness)
				{
					System.Console.WriteLine(message);
				}
			}
		}
	}
}