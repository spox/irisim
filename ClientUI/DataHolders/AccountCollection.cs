using System.Collections;

namespace IrisIM
{
	namespace UI
	{
		class AccountCollection
		{
			Hashtable collection;
			
			public AccountCollection()
			{
				this.collection = new Hashtable();
			}
			
			public void Add(Account acct)
			{
				if(this.collection.ContainsKey(acct.ID))
				{
					throw new KeyExistsException(acct.ID + " already exists in collection.");
				}
				else
				{
					this.collection[acct.ID] = acct;
				}
			}
			
			public void ForceAdd(Account acct)
			{
				this.collection[acct.ID] = acct;
			}
			
			public void Modify(Account acct)
			{
				this.ForceAdd(acct);
			}
			
			public void Remove(Account acct)
			{
				this.collection.Remove(acct.ID);
			}
			
			public Account Get(string name)
			{
				if(this.collection.ContainsKey(name))
				{
					return (Account)this.collection[name];
				}
				else
				{
					throw new NotFoundException("No key found matching: "+name.ToString());
				}
			}
			
			public bool Exists(string name)
			{
				return this.collection.ContainsKey(name);
			}
			
			public ICollection Names()
			{
				return this.collection.Keys;
			}
		}
	}
}