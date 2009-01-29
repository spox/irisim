using System;
using System.Collections;

namespace IrisIM
{
	namespace Utilities
	{
		public class ByteHolder
		{
			private Queue _holder;

			public int size
			{
				get{ return this._holder.Count; }
			}
			
			public ByteHolder()
			{
				this._holder = new Queue();
			}
			
			public void Add(byte[] bytes)
			{
				this._holder.Enqueue(bytes);
			}
			
			public byte[] Remove()
			{
				int count = 0;
				byte[] data;
				while(this._holder.Count == 0 && count++ < 10)
				{
					System.Threading.Thread.Sleep(10);
				}
				try{ data = (byte[])this._holder.Dequeue(); }
				catch(Exception e){ return null; }
				if(data == null)
				{
					throw new Exception("No more data available.");
				}
				return data;
			}
		}
	}
}