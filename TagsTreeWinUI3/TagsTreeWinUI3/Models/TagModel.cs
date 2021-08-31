using System.Text.Json.Serialization;

namespace TagsTreeWinUI3.Models
{
	public class PathTagModel
	{
		public string Name { get; protected set; }
		public PathTagModel(string name) => Name = name;
		public override string ToString() => Name; //AutoSuggestBox选择建议时会用到
	}

	public class TagModel : PathTagModel
	{
		[JsonIgnore] private static int Num { get; set; } = 1;
		public int Id { get; }
		[JsonIgnore] public string Path { get; set; } = "";
		[JsonIgnore] public string FullName => (Path is "" ? "" : Path + '\\') + Name;
		protected TagModel(int id, string name) : base(name)
		{
			Num = System.Math.Max(Num, id + 1);
			Id = id;
		}
		protected TagModel(string name, string path) : base(name)
		{
			Id = Num;
			Num++;
			Path = path;
		}
		public bool HasChildTag(TagModel child) => $"\\\\{child.Path}\\".Contains($"\\{Name}\\"); //不包含自己
	}
}
