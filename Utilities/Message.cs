using System;
using System.IO;
using System.Xml;
using System.Text;

namespace IrisIM
{
	namespace Utilities
	{
		public class Message
		{
			public enum Type
			{
				Plugins = 0,
				Loop,
				ServerToServer,
				UserToServer,
				UserToServerPlugin,
				UserToUser,
				ServerToUser,
				ServerToUserPlugin,
				Unknown
			}
			
			private Transceiver _origin;
			private MessageContent _content;
			private Message.Type _type;
			private string _creator_plugin_hash;
			private string _destination_plugin_hash;
			private string _raw_message;
			
			public Transceiver origin
			{
				get{ return this._origin; }
				set{ this._origin = value; }
			}
			
			public MessageContent content
			{
				get{ return this._content; }
				set{ this._content = value; }
			}
		
			public Message.Type type
			{
				get{ return this._type; }
				set{ this._type = value; }
			}
			
			public string creator_plugin_hash
			{
				get{ return this._creator_plugin_hash; }
				set{ this._creator_plugin_hash = value; }
			}
			
			public string destination_plugin_hash
			{
				get{ return this._destination_plugin_hash; }
				set{ this._destination_plugin_hash = value; }
			}
			
			public string raw_message
			{
				get{ return this._raw_message; }
				set
				{ 
					this._raw_message = value;
					this.Initialize();
				}
			}
			
			public Message()
			{}
			
			public Message(Transceiver connection, string message)
			{
				this._origin = connection;
				this._raw_message = message;
				this.Initialize();
			}
			
			public Message(Transceiver connection, Message.Type type, MessageContent contents, string creator)
			{
				this._origin = connection;
				this._type = type;
				this._content = contents;
			}
			
			private void Initialize()
			{
				if(this._raw_message == null)
				{
					Logger.log("Error attempting to initialize a null message", Logger.Verbosity.loud);
					throw new NullException("Message contains no data.");
				}
				else
				{
					this.DiscoverType();
					this.BuildContents();
				}
			}
			
			private void DiscoverType()
			{
				try
				{
					Logger.log("Attempting to discovering message type.", Logger.Verbosity.screaming);
					XmlDocument document = new XmlDocument();
					XmlElement element = null;
					XmlNodeList list = null;
					document.LoadXml(this._raw_message);
					list = document.GetElementsByTagName("Message");
					element = (XmlElement)list.Item(0);
					string type = element.GetAttribute("type");
					switch(type)
					{
						case "Plugins":
							this._type = Message.Type.Plugins;
							break;
						case "Loop":
							this._type = Message.Type.Loop;
							break;
						case "ServerToServer":
							this._type = Message.Type.ServerToServer;
							break;
						case "UserToServer":
							this._type = Message.Type.UserToServer;
							break;
						case "UserToServerPlugin":
							this._type = Message.Type.UserToServerPlugin;
							break;
						case "UserToUser":
							this._type = Message.Type.UserToUser;
							break;
						case "ServerToUser":
							this._type = Message.Type.ServerToUser;
							break;
						case "ServerToUserPlugin":
							this._type = Message.Type.ServerToUserPlugin;
							break;
						default:
							this._type = Message.Type.Unknown;
							break;
					}
				}
				catch(Exception e)
				{
					this._type = Message.Type.Unknown;
				}
				Logger.log("Message type found: "+this.StringType(), Logger.Verbosity.screaming);
			}
			
			private void BuildContents()
			{
				try
				{
					Logger.log("Extracting message into content object.", Logger.Verbosity.screaming);
					XmlElement element = null;
					XmlNodeList list = null;
					XmlDocument document = new XmlDocument();
					document.LoadXml(this._raw_message);
					list = document.GetElementsByTagName("Message");
					element = (XmlElement)list.Item(0);
					if(element.HasAttribute("creator"))
					{
						this._creator_plugin_hash = element.GetAttribute("creator");
					}
					list = document.GetElementsByTagName("Content");
					element = (XmlElement)list.Item(0);
					if(element.HasAttribute("destination"))
					{
						this._destination_plugin_hash = element.GetAttribute("destination");
					}
					BasicContent content = new BasicContent(element);
					switch(content.type)
					{
						case MessageContent.Type.Command:
							this._content = new Command(content);
							break;
						case MessageContent.Type.Event:
							this._content = new Event(content);
							break;
						case MessageContent.Type.Chat:
							this._content = new Chat(content);
							break;
						case MessageContent.Type.Response:
							this._content = new Response(content);
							break;
						case MessageContent.Type.Information:
							this._content = new Information(content);
							break;
						case MessageContent.Type.Plugins:
							this._content = new Plugins(element);
							break;
						case MessageContent.Type.Unknown:
							this._content = new Unknown(this._raw_message);
							break;
					}
					Logger.log("Content successfully extracted.", Logger.Verbosity.screaming);
					Logger.log("Content originator hash: "+ this._creator_plugin_hash, Logger.Verbosity.screaming);
				}
				catch
				{
					this._content = new Unknown(this._raw_message);
				}
			}
			
			public string DumpMessage()
			{
				XmlDocument document = new XmlDocument();
				XmlElement message = document.CreateElement("Message");
				document.AppendChild(message);
				XmlAttribute type = document.CreateAttribute("type");
				type.Value = this.StringType();
				message.Attributes.Append(type);
				if(this._creator_plugin_hash != null)
				{
					XmlAttribute origin = document.CreateAttribute("creator");
					origin.Value = this._creator_plugin_hash;
					message.Attributes.Append(origin);
				}
				XmlElement content = document.CreateElement("Content");
				if(this._destination_plugin_hash != null)
				{
					XmlAttribute destination = document.CreateAttribute("destination");
					destination.Value = this._destination_plugin_hash;
					content.Attributes.Append(destination);
				}
				message.AppendChild(content);
				this._content.DumpContent(document, content);
				return this.XmlToString(document);
			}
			
			public string StringType()
			{
				string type = "";
				switch(this._type)
				{
					case Message.Type.Plugins:
						type = "Plugins";
						break;
					case Message.Type.Loop:
						type = "Loop";
						break;
					case Message.Type.ServerToServer:
						type = "ServerToServer";
						break;
					case Message.Type.ServerToUser:
						type = "ServerToUser";
						break;
					case Message.Type.ServerToUserPlugin:
						type = "ServerToUserPlugin";
						break;
					case Message.Type.Unknown:
						type = "Unknown";
						break;
					case Message.Type.UserToServer:
						type = "UserToServer";
						break;
					case Message.Type.UserToServerPlugin:
						type = "UserToServerPlugin";
						break;
					case Message.Type.UserToUser:
						type = "UserToUser";
						break;
				}
				return type;
			}
			
			private string XmlToString(XmlDocument doc)
			{
				StringWriter sw = new StringWriter();
				XmlTextWriter tw = new XmlTextWriter(sw);
				doc.WriteTo(tw);
				return sw.ToString();
			}
		}
	}
}