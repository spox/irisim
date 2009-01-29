
using System.Xml;

namespace IrisIM
{
	namespace Utilities
	{
		abstract public class ComplexContent : MessageContent
		{
			public ComplexContent() : base()
			{}
			
			abstract override public void DumpContent(XmlDocument document, XmlElement element);
		}
	}
}