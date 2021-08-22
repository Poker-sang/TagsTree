﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
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

		protected FileModel(FileModel fileModel)
		{
			IsFolder = fileModel.IsFolder;
			Name = fileModel.Name;
			Path = fileModel.Path;
			Id = fileModel.Id;
		}
		protected FileModel(string fullName, bool isFolder)
		{
			Id = Num;
			Num++;
			IsFolder = isFolder;
			Name = fullName[(fullName.LastIndexOf('\\') + 1)..];
			Path = fullName[..fullName.LastIndexOf('\\')];
		}
		[JsonConstructor]
		public FileModel(int id, string name, string path, bool isFolder)
		{
			Id = id;
			Name = name;
			Path = path;
			IsFolder = isFolder;
		}
		public FileModel(FileViewModel fileViewModel)
		{
			Id = fileViewModel.Id;
			Name = fileViewModel.Name;
			Path = fileViewModel.Path;
			IsFolder = fileViewModel.IsFolder;
		}
		public void Reload(string fullName)
		{
			FileSystemInfo info = IsFolder ? new DirectoryInfo(fullName) : new FileInfo(fullName);
			Name = info.Name;
			Path = info.FullName[..^(info.Name.Length + 1)];
		}

		protected static bool ValidPath(string path) => path.Contains(Default.LibraryPath);

		public bool? HasTag(TagModel tag) //null表示拥有标签的上级标签存在本标签
		{
			foreach (var tagPossessed in Tags.GetTagModels())
				if (tag == tagPossessed)
					return true;
				else if (tag.HasChildTag(tagPossessed))
					return null;
			return false;
		}
		public IEnumerable<TagModel> GetRelativeTags(TagModel parentTag) => Tags.GetTagModels().Where(parentTag.HasChildTag);

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
		public string PathTags //如果设置没有启用PathTags则返回空串
		{
			get
			{
				var tags = "";
				if (Default.PathTags)
					tags = PartialPath[4..].Split('\\', StringSplitOptions.RemoveEmptyEntries).Aggregate(tags, (current, tag) => current + " " + tag);
				return tags is "" ? "" : tags[1..];
			}
		}
		[JsonIgnore]
		public string AllTags => Tags + PathTags is "" ? "" : " " + PathTags;
	}
}


