
using System;
using System.Xml;
using System.Collections;
using System.Security.Cryptography;

using IrisIM.Client;
using IrisIM.Utilities;

public class Cryptor : ClientPlugin
{

	private RSACryptoServiceProvider _local_cryptor;
	private RSACryptoServiceProvider _remote_cryptor;
	private string _local_public_key;
	private string _server_cryptor_hash;
	private Hashtable _keys;
	
	public Cryptor()
	{
		this._name = "Cryptor";
		this._version = "0.1";
		this._keys = new Hashtable();
	}
	
	~Cryptor()
	{}

	override public void initialize()
	{
		this._local_cryptor = new RSACryptoServiceProvider(2048);
		this._remote_cryptor = new RSACryptoServiceProvider();
		this._local_public_key = this._local_cryptor.ToXmlString(false);
	}
	
	override public void link_dependency(Object o)
	{}

	override public void process(Message message)
	{
		switch(message.type)
		{
			case Message.Type.Plugins:
				this.InitialServerSetup(message);
			break;
			case Message.Type.Loop:
				Event evnt = (Event)message.content;
				if(evnt.etype == "connection" && evnt.GetContent("status") == "disconnected")
				{
					this.DiscardKeys();
				}
			break;
		}
		if(message.creator_plugin_hash == this._server_cryptor_hash)
		{
			this.ProcessServerCryptoManager(message);
		}
	}
	
	override public void process(ClientMessage message, Transceiver connection)
	{
		if(message.type == "newUser")
		{
			this.RequestKey(message.Get("username"));
		}
	}
	
	private void DiscardKeys()
	{
		this._keys.Clear();
		this.UnhookConnection(this._controller.connection);
	}
	
	private void RequestKey(string username)
	{
		Message message = new Message();
		message.type = Message.Type.UserToServerPlugin;
		message.creator_plugin_hash = this._hash_code;
		message.destination_plugin_hash = this._server_cryptor_hash;
		Command content = new Command();
		content.ctype = "publickey";
		content.AddContent("username", username);
		message.content = content;
		this._controller.connection.Write(message);
		Logger.log("Sent request for "+username+"'s publickey.", Logger.Verbosity.moderate);
	}
	
	private void ProcessServerCryptoManager(Message message)
	{
		try
		{
			Response content = (Response)message.content;
			if(content.rtype == "success" && content.GetContent("command") == "publickey")
			{
				string key = content.GetContent("publickey");
				string user = content.GetContent("username");
				this._keys[user] = key;
				Logger.log("Received and stored "+user+"'s public key.", Logger.Verbosity.moderate);
				this.SendUINotice("Received "+user+"'s key.");
				if(user == "server")
				{
					this.HookConnection(message.origin);
				}
			}
		}
		catch(Exception ex)
		{
			Logger.log("Failed to process message. Might be unsupported type.", Logger.Verbosity.moderate);
			Logger.log(ex.Message, Logger.Verbosity.moderate);
			Logger.log(ex.StackTrace, Logger.Verbosity.moderate);
		}
	}
	
	private void InitialServerSetup(Message message)
	{
		Logger.log("Cryptor: Received Plugin information list.", Logger.Verbosity.moderate);
		Plugins content = (Plugins)message.content;
		PluginInfo info = content.Get("CryptManager");
		this._server_cryptor_hash = info.hash;
		this.SendKeyInformation(message.origin);
		Logger.log("Hashcode for server's CryptManager plugin has been stored.", Logger.Verbosity.moderate);
	}
	
	private void SendKeyInformation(Transceiver connection)
	{
		this.SendUINotice("Sending public key to server.");
		Message message = new Message();
		message.creator_plugin_hash = this._hash_code;
		message.destination_plugin_hash = this._server_cryptor_hash;
		message.type = Message.Type.UserToServerPlugin;
		Information content = new Information();
		content.type = BasicContent.Type.Information;
		content.AddContent("publickey", this._local_public_key);
		message.content = content;
		connection.Write(message);
	}
	
	private void SendUINotice(string notice)
	{
		UIMessage message = new UIMessage("encryptionStatus");
		message.Add("status", notice);
		this._controller.message_pump.process_message(message);
	}

	private void HookConnection(Transceiver connection)
	{
		connection.plugin_writers += new WriterOverload(this.WriteEncrypted);
		connection.plugin_readers += new ReaderOverload(this.ReadEncrypted);
		connection.filter_writer += new WriteFilter(this.WriteFilter);
		Message message = new Message();
		message.type = Message.Type.Loop;
		message.creator_plugin_hash = this._hash_code;
		Event content = new Event();
		content.type = MessageContent.Type.Event;
		content.etype = "Encryption";
		content.AddContent("status", "crypted");
		this.SendUINotice("Channel Encrypted.");
		message.content = content;
		this._controller.message_pump.process_message(message);
		Logger.log("Encryption hooks have been placed.", Logger.Verbosity.moderate);
	}
	
	private void UnhookConnection(Transceiver connection)
	{
		connection.plugin_writers -= new WriterOverload(this.WriteEncrypted);
		connection.plugin_readers -= new ReaderOverload(this.ReadEncrypted);
		Logger.log("Cryptor: Encryption hooks removed from transceiver.", Logger.Verbosity.moderate);
	}
	
	public string WriteFilter(Message message)
	{
		if(message.content.type == BasicContent.Type.Chat)
		{
			try
			{
				string key = (string)this._keys[message.destination_plugin_hash];
				this._remote_cryptor.FromXmlString(key);
				string basemsg = message.DumpMessage();
				XmlDocument document = new XmlDocument();
				document.LoadXml(basemsg);
				XmlNodeList list = document.GetElementsByTagName("Content");
				XmlElement element = (XmlElement)list.Item(0);
				string full = element.InnerXml;
				byte[] full_bytes = System.Text.Encoding.Unicode.GetBytes(full);
				byte[] crypted_bytes = this.Encrypt(this._remote_cryptor, full_bytes);
				string crypted = System.Text.Encoding.Unicode.GetString(crypted_bytes);
				element.InnerText = crypted;
				return document.InnerXml;
			}
			catch(Exception e)
			{
				Logger.log("Error filtering: "+e.Message, Logger.Verbosity.moderate); 
				return message.DumpMessage();
			}
		}
		else
		{
			return message.DumpMessage();
		}
	}
	
	public void WriteEncrypted(Transceiver connection, string message)
	{
		Logger.log("Encryption hook is encrypting outgoing message.", Logger.Verbosity.moderate);
		string key = (string)this._keys["server"];
		this._remote_cryptor.FromXmlString(key);
		byte[] byte_message = System.Text.Encoding.Unicode.GetBytes(message);
		byte[] crypted = Encrypt(this._remote_cryptor, byte_message);
		connection.WriteBytes(crypted);
	}
	
	public string ReadEncrypted(Transceiver connection, string message)
	{
		Logger.log("Encryption hook is decrypting incoming message.", Logger.Verbosity.moderate);
		byte[] bytes = System.Text.Encoding.Unicode.GetBytes(message);
		byte[] decrypted = Decrypt(this._local_cryptor, bytes);
		string final = System.Text.Encoding.Unicode.GetString(decrypted);
		try
		{
			Message test_message = new Message(null, final);
			Logger.log("THE MESSAGE: "+test_message.DumpMessage(), Logger.Verbosity.moderate);
			Logger.log("Type of message is: "+test_message.StringType(), Logger.Verbosity.moderate);
			if(test_message.type == Message.Type.UserToUser)
			{
					
				/** Test for encrypted middle **/
			
				Logger.log("Attempting to scrape inner message.", Logger.Verbosity.moderate);
				
				XmlDocument document  = new XmlDocument();
				document.LoadXml(final);
				XmlNodeList list = document.GetElementsByTagName("Content");
				XmlElement element = (XmlElement)list.Item(0);
				string core = element.InnerText;
				byte[] core_bytes = System.Text.Encoding.Unicode.GetBytes(core);
				byte[] decrypted_bytes = this.Decrypt(this._local_cryptor, core_bytes);
				string cdecrypted = System.Text.Encoding.Unicode.GetString(decrypted_bytes);
				Logger.log("Decrypted inner message: "+cdecrypted, Logger.Verbosity.loud);
				element.InnerXml = cdecrypted;
				return document.InnerXml;
			}
			else
			{
				return final;
			}
		}
		catch(Exception e)
		{
			Logger.log("Decryptor ERROR: "+e.Message+"\n"+e.StackTrace, Logger.Verbosity.moderate);
			return final;
		}
	}

	/*********************
	 * Encryption methods
	 * Lifted from:
	 * http://www.go-mono.com/docs/index.aspx?link=M%3aSystem.Security.Cryptography.RSACryptoServiceProvider.Encrypt(System.Byte%5b%5d%2cSystem.Boolean)
	 * http://www.go-mono.com/docs/index.aspx?link=M%3aSystem.Security.Cryptography.RSACryptoServiceProvider.Decrypt(System.Byte%5b%5d%2cSystem.Boolean)
	 * respectively
	 *********************/

	private byte[] Encrypt (RSA rsa, byte[] input) 
	{
	     // by default this will create a 128 bits AES (Rijndael) object
	     SymmetricAlgorithm sa = SymmetricAlgorithm.Create ();
	     ICryptoTransform ct = sa.CreateEncryptor ();
	     byte[] encrypt = ct.TransformFinalBlock (input, 0, input.Length);

	     RSAPKCS1KeyExchangeFormatter fmt = new RSAPKCS1KeyExchangeFormatter (rsa);
	     byte[] keyex = fmt.CreateKeyExchange (sa.Key);

	     // return the key exchange, the IV (public) and encrypted data
	     byte[] result = new byte [keyex.Length + sa.IV.Length + encrypt.Length];
	     Buffer.BlockCopy (keyex, 0, result, 0, keyex.Length);
	     Buffer.BlockCopy (sa.IV, 0, result, keyex.Length, sa.IV.Length);
	     Buffer.BlockCopy (encrypt, 0, result, keyex.Length + sa.IV.Length, encrypt.Length);
	     return result;
	}

	private byte[] Decrypt (RSA rsa, byte[] input) 
	{
	     // by default this will create a 128 bits AES (Rijndael) object
	     SymmetricAlgorithm sa = SymmetricAlgorithm.Create ();

	     byte[] keyex = new byte [rsa.KeySize >> 3];
	     Buffer.BlockCopy (input, 0, keyex, 0, keyex.Length);

	     RSAPKCS1KeyExchangeDeformatter def = new RSAPKCS1KeyExchangeDeformatter (rsa);
	     byte[] key = def.DecryptKeyExchange (keyex);

	     byte[] iv = new byte [sa.IV.Length];
	     Buffer.BlockCopy (input, keyex.Length, iv, 0, iv.Length);

	     ICryptoTransform ct = sa.CreateDecryptor (key, iv);
	     byte[] decrypt = ct.TransformFinalBlock (input, keyex.Length + iv.Length, input.Length - (keyex.Length + iv.Length));
	     return decrypt;
	}

}