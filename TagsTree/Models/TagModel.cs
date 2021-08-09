using System.Xml;

namespace TagsTree.Models
{
	public class PathTagModel
	{
		public string Name { get; }
		public PathTagModel(string name) => Name = name;
	}
	public class TagModel : PathTagModel
	{
		public string Path { get; }
		public XmlElement XmlElement { get; }
		public TagModel(string name, string path, XmlElement xmlElement) : base(name)
		{
			Path = path;
			XmlElement = xmlElement;
		}

		public string FullName => (Path is "" ? "" : Path + '\\') + Name;
		public override string ToString() => Name;

		public bool HasChildTag(TagModel child) => $"\\{child.Path}\\".Contains($"\\{Name}\\"); //不包含自己
	}
}