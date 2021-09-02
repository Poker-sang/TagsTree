using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Models
{
	public class FileModel
	{
		private static int Num { get; set; }

		public int Id { get; }
		public string Name { get; private set; }
		public string Path { get; private set; }

		protected FileModel(FileModel fileModel)
		{
			Id = fileModel.Id;
			Name = fileModel.Name;
			Path = fileModel.Path;
		}
		protected FileModel(string fullName)
		{
			Id = -1;
			Name = fullName.GetName();
			Path = fullName.GetPath();
		}
		/// <summary>
		/// 反序列化专用，不要调用该构造器
		/// </summary>
		[JsonConstructor]
		public FileModel(int id, string name, string path)
		{
			Num = Math.Max(Num, id + 1);
			Id = id;
			Name = name;
			Path = path;
		}
		public FileModel(FileViewModel fileViewModel)
		{
			Id = Num;
			Num++;
			Name = fileViewModel.Name;
			Path = fileViewModel.Path;
		}
		public void Reload(string fullName)
		{
			FileSystemInfo info = IsFolder ? new DirectoryInfo(fullName) : new FileInfo(fullName);
			var index =
			Name = info.Name;
			Path = fullName.GetPath();
		}

		protected static bool ValidPath(string path) => path.Contains(App.AppConfigurations.LibraryPath);

		protected bool? HasTag(TagViewModel tag) //null表示拥有标签的上级标签存在本标签
		{
			foreach (var tagPossessed in Tags.GetTagViewModels())
				if (tag == tagPossessed)
					return true;
				else if (tag.HasChildTag(tagPossessed))
					return null;
			return false;
		}
		public IEnumerable<TagViewModel> GetRelativeTags(TagViewModel parentTag) => Tags.GetTagViewModels().Where(parentTag.HasChildTag);

		[JsonIgnore] public string Extension => IsFolder ? "文件夹" : Name.Split('.', StringSplitOptions.RemoveEmptyEntries).Last().ToUpper(CultureInfo.CurrentCulture);
		[JsonIgnore] protected string PartialPath => "..." + Path[App.AppConfigurations.LibraryPath.Length..]; //Path必然包含文件路径
		[JsonIgnore] public string FullName => Path + '\\' + Name; //Path必然包含文件路径
		[JsonIgnore] public string UniqueName => IsFolder + FullName;
		[JsonIgnore] public bool IsFolder => Directory.Exists(FullName);
		[JsonIgnore]
		protected string Tags
		{
			get
			{
				var tags = App.Relations.GetTags(this).Select(tag => tag.Name).Aggregate("", (current, tag) => current + " " + tag);
				return tags is "" ? "" : tags[1..];
			}
		}
		[JsonIgnore] public IEnumerable<string> PathTags => PartialPath is "..." ? Enumerable.Empty<string>() : PartialPath[4..].Split('\\', StringSplitOptions.RemoveEmptyEntries); //PartialPath不会是空串
	}
}


