using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Models
{
    public class TreeDictionary
    {
        public DoubleKeysDictionary<int, string, TagViewModel> TagsDictionary { get; } = new();

        public TagViewModel TagsTree { get; } = new(0, ""); //属于反序列化一部分

        public IEnumerable<TagViewModel> TagsDictionaryValues => TagsDictionary.Values.Skip(1);
        public TagViewModel TagsDictionaryRoot => TagsDictionary[0]; //或TagsDictionary[""]

        public void AddTag(TagViewModel path, string name)
        {
            var temp = new TagViewModel(name, path.FullName);
            path.SubTags.Add(temp);

            TagsDictionary[temp.Id, name] = temp;
        }
        public void MoveTag(TagViewModel tag, TagViewModel newPath)
        {
            _ = TagsDictionary[tag.Id].GetParentTag().SubTags.Remove(tag);
            newPath.SubTags.Add(tag);

            TagsDictionary[tag.Id].Path = TagsDictionary[newPath.Id].FullName;
        }
        public void RenameTag(TagViewModel tag, string newName)
        {
            TagsDictionary.ChangeKey2(tag.Name, newName);

            tag.Name = newName;
        }
        public void DeleteTag(TagViewModel tag)
        {
            TagsDictionary[tag.Id].GetParentTag().SubTags.Remove(tag);

            TagsDictionary.Remove(tag.Id);
        }


        /// <summary>
        /// 递归读取标签
        /// </summary>
        /// <param name="path">标签所在路径</param>
        /// <param name="tag">标签所在路径的标签</param>
        private void RecursiveLoadTags(string path, TagViewModel tag)
        {
            tag.Path = path;
            TagsDictionary[tag.Id, tag.Name] = tag;
            foreach (var subTag in tag.SubTags)
                RecursiveLoadTags((path is "" ? "" : path + '\\') + tag.Name, subTag);
        }

        public void DeserializeTree(string path) => TagsTree.SubTags = Serialization.Deserialize<ObservableCollection<TagViewModel>>(path);

        public void LoadDictionary()
        {
            TagsDictionary.Clear();
            RecursiveLoadTags("", TagsTree);
        }
    }
}
