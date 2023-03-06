using System.Collections.Generic;
using System.Linq;
using TagsTree.Algorithm;
using TagsTree.Services;
using TagsTree.Views.ViewModels;

namespace TagsTree.Models;

public class TagsTreeDictionary
{
    public DoubleKeysDictionary<int, string, TagViewModel> TagsDictionary { get; } = new();

    /// <remarks>
    /// 属于反序列化一部分
    /// </remarks>
    public TagViewModel TagsTree { get; } = new(0, "");

    public IEnumerable<TagViewModel> TagsDictionaryValues => TagsDictionary.Values.Skip(1);

    /// <remarks>
    /// 或<see cref="TagsDictionary"/>[""]
    /// </remarks>
    public TagViewModel TagsDictionaryRoot => TagsDictionary[0];

    public TagViewModel AddTag(TagViewModel path, string name)
    {
        var temp = new TagViewModel(name, path);
        path.SubTags.Add(temp);

        TagsDictionary[temp.Id, name] = temp;
        return temp;
    }

    public void MoveTag(TagViewModel tag, TagViewModel newPath)
    {
        _ = TagsDictionary[tag.Id].Parent!.SubTags.Remove(tag);
        newPath.SubTags.Add(tag);
    }

    public void RenameTag(TagViewModel tag, string newName)
    {
        TagsDictionary.ChangeKey2(tag.Name, newName);

        tag.Name = newName;
    }

    public void DeleteTag(TagViewModel tag)
    {
        _ = TagsDictionary[tag.Id].Parent!.SubTags.Remove(tag);

        _ = TagsDictionary.Remove(tag.Id);
    }

    /// <summary>
    /// 递归读取标签到<see cref="TagsDictionary"/>
    /// </summary>
    /// <param name="tag">标签所在路径的标签</param>
    private void RecursiveLoadTags(TagViewModel tag)
    {
        TagsDictionary[tag.Id, tag.Name] = tag;
        foreach (var subTag in tag.SubTags)
            RecursiveLoadTags(subTag);
    }

    public void DeserializeTree(string path)
    {
        // 为了触发TagsTree.SubTags.CollectionChanged
        foreach (var subTag in Serialization.Deserialize<List<TagViewModel>>(path))
            TagsTree.SubTags.Add(subTag);
    }

    public void LoadDictionary()
    {
        TagsDictionary.Clear();
        RecursiveLoadTags(TagsTree);
    }

    public void Serialize(string path) => Serialization.Serialize(path, TagsTree.SubTags);
}
