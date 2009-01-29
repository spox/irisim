
/** this is an incomplete message type not yet
    ready for use. it is intended for large
    data transfers like files. **/

using System.Xml;

namespace IrisIM
{
	namespace Utilities
	{
		public class Part : ComplexContent
		{
			private int _ID;
			private int _total;
			private string _data;
			
			public int ID
			{
				get{ return this._ID; }
				set{ this._ID = value; }
			}
			
			public int total
			{
				get{ return this._total; }
				set{ this._total = value; }
			}
			
			public string data
			{
				get{ return this._data; }
				set{ this._data = value; }
			}
			
			public Part()
			{}
			
			public Part(string content) : base()
			{
				this.ParseContent(content);
			}
			
			private void ParseContent(string content)
			{
				XmlDocument document = new XmlDocument();
				document.LoadXml(content);
				XmlNodeList list = document.GetElementsByTagName("Part");
				XmlElement element = (XmlElement)list.Item(0);
				this._ID = System.Convert.ToInt32(element.GetAttribute("id"));
				this._total = System.Convert.ToInt32(element.GetAttribute("total"));
				list = document.GetElementsByTagName("Value");
				element = (XmlElement)list.Item(0);
				this._data = element.InnerXml;
			}
			
			override public void DumpContent(XmlDocument document, XmlElement element)
			{
				XmlElement elm = document.CreateElement("Part");
				XmlAttribute id = document.CreateAttribute("id");
				id.Value = this._ID.ToString();
				elm.Attributes.Append(id);
				XmlAttribute total = document.CreateAttribute("total");
				total.Value = this._total.ToString();
				elm.Attributes.Append(total);
				XmlElement val = document.CreateElement("Value");
				XmlAttribute name = document.CreateAttribute("name");
				name.Value = "data";
				val.Attributes.Append(name);
				val.InnerXml = this._data;
				elm.AppendChild(val);
				element.AppendChild(elm);
			}
		}
	}
}