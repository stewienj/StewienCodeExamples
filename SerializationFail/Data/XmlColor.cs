using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SerializationFail.Data
{
	/// <summary>
	/// Converter for serializing colors. From stackoverflow author: bvj
	/// </summary>
	/// <example>
	/// [XmlElement(Type=typeof(XmlColor)]
	/// public Color MyColor { get; set; }
	/// </example>
	public class XmlColor : IXmlSerializable
	{
		private Color _color = Colors.Black;
		private string _hexPrefix = "0x";

		public XmlColor()
		{

		}

		public XmlColor(Color color)
		{
			_color = color;
		}

		public XmlColor(string color)
		{
			StringValue = color;
		}

		public string StringValue
		{
			get
			{
				var parts = new byte[]
				{
					_color.A,
					_color.R,
					_color.G,
					_color.B
				};
				return parts.Select(bt => bt.ToString("X2")).Aggregate(_hexPrefix, (a, b) => a + b);
			}
			set
			{
				string hex = value;

				if (hex.StartsWith("0x"))
				{
					hex = hex.Substring(2);
					_hexPrefix = "0x";
				}
				else if (hex.StartsWith("#"))
				{
					hex = hex.Substring(1);
					_hexPrefix = "#";
				}

				Int64 number = Convert.ToInt64(hex, 16);
				byte b = (byte)(number & 0xFF);
				number = number >> 8;
				byte g = (byte)(number & 0xFF);
				number = number >> 8;
				byte r = (byte)(number & 0xFF);
				number = number >> 8;
				byte a = (byte)(number & 0xFF);
				_color = Color.FromArgb(a, r, g, b);
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			StringValue = reader.ReadElementContentAsString();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteString(StringValue);
		}

		public static implicit operator Color(XmlColor xmlColor)
		{
			return xmlColor._color;
		}

		public static implicit operator XmlColor(Color color)
		{
			return new XmlColor(color);
		}
	}
}
