using System;
using System.Linq;
using System.Xml;

namespace TagsTree.Models
{
	public class TagModel
	{
		public string Name { get; }
		public string Path { get; }
		public XmlElement XmlElement { get; }
		public TagModel(string name, string path, XmlElement xmlElement)
		{
			Name = name;
			Path = path;
			XmlElement = xmlElement;
		}

		public string FullName => (Path is "" ? "" : Path + '\\') + Name;
		public override string ToString() => Name;

		public bool HasChildTag(TagModel child) => $"\\{child.Path}\\".Contains($"\\{Name}\\"); //不包含自己
	}
}