using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SQLite;
using static TagsTree.Properties.Settings;

namespace TagsTree.Models
{
	public class FileModel
	{
		public int Id { get; set; }

		public string Name { get; set; } = "";
		public string Path { get; set; } = "";
		
		public bool IsFolder { get; set; }

		public FileModel() { }  //JsonSerializer.Deserialize需要无参数构造函数
		public FileModel(string name, string path, bool isFolder)
		{
			Name = name;
			Path = path;
			IsFolder = isFolder;
		}

		public FileModel(string name, string path, bool isFolder, IEnumerable<string> tags)
		{
			Name = name;
			Path = path;
			IsFolder = isFolder;
			foreach (var tag in tags)
				Tags += " " + tag;
			Tags = Tags[1..];
		}

		public static bool ValidPath(string fullName) => fullName.Contains(Default.LibraryPath);

		[JsonIgnore] public string PartialPath => "..." + Path[Default.LibraryPath.Length..]; //Path必然包含文件路径
		[JsonIgnore] public string FullName => Path + '\\' + Name; //Path必然包含文件路径
		[JsonIgnore] public string Tags { get; } = "";
	}
}


