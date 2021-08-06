using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using static TagsTree.Properties.Settings;

namespace TagsTree.Models
{
	public class FileModel
	{
		public static int Num { get; set; }

		public int Id { get; }
		public string Name { get; private set; }
		public string Path { get; private set; }
		public bool IsFolder { get; }

		[JsonConstructor]
		public FileModel(int id, string name, string path, bool isFolder)
		{
			Id = id;
			Name = name;
			Path = path;
			IsFolder = isFolder;
		}
		public FileModel(string fullName, bool isFolder)
		{
			Id = Num;
			Num++;
			IsFolder = isFolder;
			Name = fullName[(fullName.LastIndexOf('\\') + 1)..];
			Path = fullName[..fullName.LastIndexOf('\\')];
		}
		protected FileModel(FileModel fileModel)
		{
			IsFolder = fileModel.IsFolder;
			Name = fileModel.Name;
			Path = fileModel.Path;
			Id = fileModel.Id;
		}
		public void Reload(string fullName)
		{
			FileSystemInfo info = IsFolder ? new DirectoryInfo(fullName) : new FileInfo(fullName);
			Name = info.Name;
			Path = info.FullName[..^(info.Name.Length + 1)];
		}

		public static bool ValidPath(string path) => path.Contains(Default.LibraryPath) &&
			path.Split('\\', StringSplitOptions.RemoveEmptyEntries).Length > Default.LibraryPath.Split('\\', StringSplitOptions.RemoveEmptyEntries).Length;

		[JsonIgnore] public string Extension => IsFolder ? "文件夹" : Name.Split('.', StringSplitOptions.RemoveEmptyEntries).Last().ToUpper();
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
		[JsonIgnore]
		public string AllTags
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


