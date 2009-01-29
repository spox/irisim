
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Xml;
using IrisIM.Server;
using IrisIM.Utilities;

public class CryptManager : Plugin
{
	private string _name;
	private string _description;
	private string _version;
	private ArrayList _dependencies;
	private ArrayList _foreign_dependencies;
	private Controller _controller;

	public string name
	{
		get{ return this._name;	}
		set{ this._name = value; }
	}
	
	public string version
	{
		get{ return this._version; }
	}
	
	public string description
	{
		get{ return this._description; }
		set{ this._description = value; }
	}
	
	public ArrayList dependencies
	{
		get{ return this._dependencies; }
	}
	
	public ArrayList foreign_dependencies
	{
		get{ return this._foreign_dependencies; }
	}
	
	public IController controller
	{
		set{ this._controller = (Controller)value; }
	}

	private Hashtable _user_keys;
	private RSACryptoServiceProvider _server_cryptor;
	private RSACryptoServiceProvider _user_cryptor;
	private string _server_public_key;
	private string _hash_code;
	private UserManager _user_manager;

	public CryptManager()
	{
		this._dependencies = new ArrayList();
		this._name = "CryptManager";
		this._version = "0.1";
		this._user_keys = new Hashtable();
		this._dependencies.Add("UserManager");
	}
	
	public void process(Message message)
	{
		if(message.destination_plugin_hash == this._hash_code)
		{
			Logger.log("Received message with CryptManager for destination.", Logger.Verbosity.moderate);
			try
			{
				this.ProcessDirectedMessage(message);
			}
			catch(Exception e)
			{
				Logger.log("CryptManager: Failed to process directed message. ("+e.Message+")", Logger.Verbosity.moderate);
			}
		}
		else
		{
			if(message.type == Message.Type.Unknown)
			{
				try
				{
					this.ProcessUnknownMessage(message);
				}
				catch(Exception e)
				{
					Logger.log("CryptManager: Failed to process Unknown message. ("+e.Message+")", Logger.Verbosity.moderate);
				}
			}
		}
	}
	
	public void initialize()
	{
		this._hash_code = this.GetHashCode().ToString();
		this._user_cryptor = new RSACryptoServiceProvider();
		this._server_cryptor = new RSACryptoServiceProvider(2048);
		this._server_public_key = this._server_cryptor.ToXmlString(false);
	}
	
	public void link_dependency(Object obj)
	{
		try
		{
			this._user_manager = (UserManager)obj;
			Logger.log("CryptManager: UserManager successfully linked.", Logger.Verbosity.moderate);
		}
		catch{}
	}
	
	private void ProcessDirectedMessage(Message message)
	{
		try
		{
			Information content = (Information)message.content;
			string xml = message.DumpMessage();
			Logger.log(xml, Logger.Verbosity.moderate);
			XmlDocument document = new XmlDocument();
			document.LoadXml(xml);
			XmlNodeList list = document.GetElementsByTagName("Value");
			XmlElement element = (XmlElement)list.Item(0);
			if(element.HasAttribute("name") && element.GetAttribute("name") == "publickey")
			{
				string key = element.InnerXml;
				this._user_keys[message.origin] = key;
				this.SendKey(message.origin, null, message.creator_plugin_hash);
				Logger.log("Sending server public key to client.", Logger.Verbosity.moderate);
				message.origin.plugin_writers += this.WriteEncryptedMessage;
				message.origin.plugin_readers += this.ReadEncryptedMessage;
				Logger.log("Encryption hooks have been added to transceiver.", Logger.Verbosity.moderate);
			}
		}
		catch(Exception ex)
		{
			try
			{
				Command content = (Command)message.content;
				if(content.ctype == "publickey" && content.Exists("username"))
				{
					this.PublicKeyRequest(message, content.GetContent("username"));
				}
				else
				{
					throw new Exception("User has not been logged in.");
				}
			}
			catch(Exception e)
			{
				Logger.log("Message type received is not supported. Ignoring.: "+e.Message+" : "+e.StackTrace, Logger.Verbosity.moderate);
			}
		}
	}
	
	private void ProcessUnknownMessage(Message message)
	{
		Unknown contents = (Unknown)message.content;
		string content = contents.message_data;
		byte[] byte_content = System.Text.Encoding.Unicode.GetBytes(content);
		byte[] decrypt = Decrypt(this._server_cryptor, byte_content);
		string mesg = System.Text.Encoding.Unicode.GetString(decrypt);
		try
		{
			Message new_message = new Message(message.origin, mesg);
			this._controller.message_pump.process_message(new_message);
		}
		catch(Exception e)
		{
			Logger.log("Decrypted message but still failed creating message object.", Logger.Verbosity.loud);
			Logger.log("Discarding garbage message.", Logger.Verbosity.moderate);
		}
	}
	
	private void PublicKeyRequest(Message message, string username)
	{
		User request = this._user_manager.GetUser(username);
		User origin = this._user_manager.GetUser(message.origin);
		this.SendKey(origin.connection, request, null); /*** fix this ***/
		this.SendKey(request.connection, origin, null);
	}
	
	private void SendKey(Transceiver connection, User request, string userPluginHash)
	{
		string publickey;
		string username;
		Message message = new Message();
		message.creator_plugin_hash = this._hash_code;
		message.type = Message.Type.ServerToUser;
		if(request == null)
		{
			publickey = this._server_public_key;
			username = "server";
		}
		else
		{
			publickey = (string)this._user_keys[request.connection];
			username = request.username;
		}
		Response content = new Response();
		message.content = content;
		content.type = BasicContent.Type.Response;
		content.rtype = "success";
		content.AddContent("command", "publickey");
		content.AddContent("username", username);
		content.AddContent("publickey", publickey);
		string mess = message.DumpMessage();
		connection.Write(mess);
	}
	
	public string ReadEncryptedMessage(Transceiver connection, string message)
	{
		Logger.log("Encryption hook is decrypting incoming message.", Logger.Verbosity.moderate);
		byte[] byte_message = System.Text.Encoding.Unicode.GetBytes(message);
		byte[] decrypted = Decrypt(this._server_cryptor, byte_message);
		return System.Text.Encoding.Unicode.GetString(decrypted);
	}
	
	public void WriteEncryptedMessage(Transceiver connection, string message)
	{
		Logger.log("Encryption hook is encrypting outgoing message.", Logger.Verbosity.moderate);
		string key = (string)this._user_keys[connection];
		this._user_cryptor.FromXmlString(key);
		byte[] byte_message = System.Text.Encoding.Unicode.GetBytes(message);
		byte[] crypted_message = Encrypt(this._user_cryptor, byte_message);
		connection.WriteBytes(crypted_message);
	}
	
	/*********************
	 * Encryption methods
	 * Lifted from:
	 * http://www.go-mono.com/docs/index.aspx?link=M%3aSystem.Security.Cryptography.RSACryptoServiceProvider.Encrypt(System.Byte%5b%5d%2cSystem.Boolean)
	 * http://www.go-mono.com/docs/index.aspx?link=M%3aSystem.Security.Cryptography.RSACryptoServiceProvider.Decrypt(System.Byte%5b%5d%2cSystem.Boolean)
	 * repectively
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