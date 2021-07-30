using System.Linq;
using System.Text.Json.Serialization;
using static TagsTree.Properties.Settings;

namespace TagsTree.Models
{
	public class FileModel
	{
		public static int Num { get; set; }
		public int Id { get; }
		public string Name { get; set; }
		public string Path { get; set; }

		public bool IsFolder { get; set; }

		public FileModel(string name, string path, bool isFolder)
		{
			Id = Num;
			Num++;
			Name = name;
			Path = path;
			IsFolder = isFolder;
		}
		public static bool ValidPath(string fullName) => fullName.Contains(Default.LibraryPath);

		[JsonIgnore] public string PartialPath => "..." + Path[Default.LibraryPath.Length..]; //Path必然包含文件路径
		[JsonIgnore] public string FullName => Path + '\\' + Name; //Path必然包含文件路径
		[JsonIgnore] public string UniqueName => IsFolder + FullName;
		[JsonIgnore]
		public string Tags
		{
			get
			{
				var tags = App.Relations.GetTags(this).Aggregate("", (current, tag) => current + " " + tag);
				return tags is "" ? "" : tags[1..];
			}
		}
	}
}


