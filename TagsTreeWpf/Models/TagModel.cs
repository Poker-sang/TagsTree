using System.Xml;

namespace TagsTreeWpf.Models
{
	public class PathTagModel
	{
		public string Name { get; set; }
		public PathTagModel(string name) => Name = name;
	}
	public class TagModel : PathTagModel
	{
		public static int Num { get; set; }

		public int Id { get; }
		public string Path { get; set; }
		public XmlElement XmlElement { get; }
		public TagModel(string name, string path, XmlElement xmlElement) : base(name)
		{
			Id = Num;
			Num++;
			Path = path;
			XmlElement = xmlElement;
		}
		public TagModel(int id, string name, string path, XmlElement xmlElement) : base(name)
		{
			Num = Num - 1 >= id ? Num : id + 1;
			Id = id;
			Path = path;
			XmlElement = xmlElement;
		}

		public string FullName => (Path is "" ? "" : Path + '\\') + Name;
		public override string ToString() => Name;

		public bool HasChildTag(TagModel child) => $"\\{child.Path}\\".Contains($"\\{Name}\\"); //不包含自己
	}
}