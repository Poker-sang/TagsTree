using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TagsTreeWinUI3.Services;

namespace TagsTreeWinUI3.Models
{
	public class TreeDictionary
	{
		public DoubleKeysDictionary<int, string, TagModel> TagsDictionary { get; } = new();

		public TagModel TagsTree { get; } = new(0, "", ""); //属于反序列化一部分

		public IEnumerable<TagModel> TagsDictionaryValues => TagsDictionary.Values.Where(value => value.Id != 0);
		public TagModel TagsDictionaryRoot => TagsDictionary[0]; //或TagsDictionary[""]

		public int AddTag(TagModel path, string name)
		{
			var temp = new TagModel(name, path.FullName);
			path.SubTags.Add(temp);

			TagsDictionary[temp.Id, name] = temp;
			return temp.Id;
		}
		public void MoveTag(TagModel tag, TagModel newPath)
		{
			_ = TagsDictionary[tag.Id].GetParentTag.SubTags.Remove(tag);
			newPath.SubTags.Add(tag);

			TagsDictionary[tag.Id].Path = TagsDictionary[newPath.Id].FullName;
		}
		public void RenameTag(TagModel tag, string newName)
		{
			TagsDictionary.ChangeKey2(tag.Name, newName);

			tag.Name = newName;
		}
		public void DeleteTag(TagModel tag)
		{
			TagsDictionary[tag.Id].GetParentTag.SubTags.Remove(tag);

			TagsDictionary.Remove(tag.Id);
		}


		/// <summary>
		/// 递归读取标签
		/// </summary>
		/// <param name="path">标签所在路径</param>
		/// <param name="tag">标签所在路径的标签</param>
		private void RecursiveLoadTags(string path, TagModel tag)
		{
			tag.Path = path;
			TagsDictionary[tag.Id, tag.Name] = tag;
			foreach (var subTag in tag.SubTags)
				RecursiveLoadTags((path is "" ? "" : path + '\\') + tag.Name, subTag);
		}

		public void LoadTree(string path) => TagsTree.SubTags = Serialization.Deserialize<ObservableCollection<TagModel>>(path);

		public void LoadDictionary()
		{
			TagsDictionary.Clear();
			RecursiveLoadTags("", TagsTree);
		}
	}
}
