using System;
using System.Collections;

namespace IrisIM
{
	namespace Utilities
	{
		public interface Plugin
		{
			string name{ get; }
			string version{ get; }
			string description{ get; }
			ArrayList dependencies{ get; }
			ArrayList foreign_dependencies{ get; }
			IController controller{ set; }
			void link_dependency(Object obj);
			void process(Message message);
			void initialize();
		}
	}
}