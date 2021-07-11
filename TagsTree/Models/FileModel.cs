using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using static TagsTree.Properties.Settings;

namespace TagsTree.Models
{
	[Table(nameof(FileModel))]
	public class FileModel
	{
		[PrimaryKey, AutoIncrement]
		[Column(nameof(Id))]
		public int Id { get; set; }

		[Column(nameof(Name))] public string Name { get; set; } = "";
		[Column(nameof(Path))] public string Path { get; set; } = "";
		[Column(nameof(IsFolder))] public bool IsFolder { get; set; }

		public FileModel() { }
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

		public string PartialPath => "..." + Path[Default.LibraryPath.Length..]; //Path必然包含文件路径
		public string Tags { get; } = "";
	}
}
