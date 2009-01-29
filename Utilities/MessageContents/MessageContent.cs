/* this is just use for a common 
   parent, nothing else */
using System.Xml;

namespace IrisIM
{
	namespace Utilities
	{
		abstract public class MessageContent
		{
		
			public enum Type
			{
				Response = 0,
				Command,
				Event,
				Chat,
				Information,
				Plugins,
				Unknown
			}
			
			protected MessageContent.Type _type;
			protected XmlElement _raw_content;
		
			public MessageContent.Type type
			{
				get{ return this._type; }
				set{ this._type = value; }
			}
		
			public MessageContent()
			{
				this._type = MessageContent.Type.Unknown;
				this._raw_content = null;
			}
		
			public MessageContent(XmlElement message)
			{}
			
			abstract public void DumpContent(XmlDocument document, XmlElement element);
			
			protected XmlElement DiscoverType()
			{
				XmlNodeList list = null;
				list = this._raw_content.GetElementsByTagName("ChatMessage");
				if(list.Count > 0)
				{
					this._type = MessageContent.Type.Chat;
					return (XmlElement)list.Item(0);
				}
				list = this._raw_content.GetElementsByTagName("Response");
				if(list.Count > 0)
				{
					this._type = MessageContent.Type.Response;
					return (XmlElement)list.Item(0);
				}
				list = this._raw_content.GetElementsByTagName("Command");
				if(list.Count > 0)
				{
					this._type = MessageContent.Type.Command;
					return (XmlElement)list.Item(0);
				}
				list = this._raw_content.GetElementsByTagName("Event");
				if(list.Count > 0)
				{
					this._type = MessageContent.Type.Event;
					return (XmlElement)list.Item(0);
				}				
				list = this._raw_content.GetElementsByTagName("Information");
				if(list.Count > 0)
				{
					this._type = MessageContent.Type.Information;
					return (XmlElement)list.Item(0);
				}
				list = this._raw_content.GetElementsByTagName("Plugins");
				if(list.Count > 0)
				{
					this._type = MessageContent.Type.Plugins;
					return (XmlElement)list.Item(0);
				}
				this._type = MessageContent.Type.Unknown;
				return null;
			}
		}
	}
}