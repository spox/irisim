// created on 1/5/2006 at 10:40 AM

/* collection type objects should be derived from this to add thread safety methods */

using System;

namespace IrisIM
{
	namespace Utilities
	{
		public class Access
		{
			
			private bool _lock;
			
			public Access()
			{
				this._lock = false;
			}
			
			protected void request_access()
			{
				int count = 0;
				while(this._lock)
				{
					/* if we spin for too long we can assume that we 
					   ended up in a deadlock so we will just bail out. */
					if(count++ > 20)
					{
						throw new Exception("Failed to gain access.");
					}
					System.Threading.Thread.Sleep(5);
				}
				this._lock = true;			
			}
			
			protected void relieve_access()
			{
				this._lock = false;
			}
		}
	}
}