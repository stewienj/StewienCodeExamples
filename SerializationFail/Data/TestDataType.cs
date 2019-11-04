using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace SerializationFail.Data
{
	public class TestDataType
	{
		public TestDataType() { }


		public TestDataType(string name)
		{
			Name = name;
		}

		public string Name { get; set; }

		[XmlElement(Type=typeof(XmlColor))]
		public Color SomeColor { get; set; } = Colors.Cornsilk;
	}
}
