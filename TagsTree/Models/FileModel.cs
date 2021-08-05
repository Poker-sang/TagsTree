using System;
using System.Linq;
using System.Text.Json.Serialization;
using static TagsTree.Properties.Settings;

namespace TagsTree.Models
{
	public class FileModel
	{
		public static int Num { get; set; }
		public int Id { get; }
		public string Name { get; }
		public string Path { get; }
		public bool IsFolder { get; }

		public FileModel(string name, string path, bool isFolder)
		{
			Id = Num;
			Num++;
			Name = name;
			Path = path;
			IsFolder = isFolder;
		}

		public static bool ValidPath(string path) => path.Contains(Default.LibraryPath) &&
			path.Split('\\', StringSplitOptions.RemoveEmptyEntries).Length > Default.LibraryPath.Split('\\', StringSplitOptions.RemoveEmptyEntries).Length;
		[JsonIgnore] public string PartialPath => "..." + Path[Default.LibraryPath.Length..]; //Path必然包含文件路径
		[JsonIgnore] public string FullName => Path + '\\' + Name; //Path必然包含文件路径
		[JsonIgnore] public string UniqueName => IsFolder + FullName;
		[JsonIgnore]
		public string Tags
		{
			get
			{
				var tags = "";
				if (Default.PathTags)
					tags = PartialPath[4..].Split('\\', StringSplitOptions.RemoveEmptyEntries).Aggregate(tags, (current, tag) => current + " " + tag);
				tags += App.Relations.GetTags(this).Aggregate("", (current, tag) => current + " " + tag);
				return tags is "" ? "" : tags[1..];
			}
		}
	}
}


