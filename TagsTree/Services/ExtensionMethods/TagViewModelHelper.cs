using System;
using System.Collections.Generic;
using System.Linq;
using TagsTree.Models;
using TagsTree.ViewModels;

namespace TagsTree.Services.ExtensionMethods;

public static class TagViewModelHelper
{
    /// <summary>
    /// 补全标签的路径（为空则为根标签）
    /// </summary>
    /// <param name="name">需要找的单个标签</param>
    /// <param name="range">搜索范围</param>
    /// <returns>找到的标签，若返回<see langword="null"/>即没找到路径</returns>
    public static TagViewModel? GetTagViewModel(this string name, TagsTreeDictionary? range = null)
    {
        range ??= AppContext.Tags;
        var temp = name.Split('\\', StringSplitOptions.RemoveEmptyEntries);
        return temp.Length is 0 ? range.TagsDictionaryRoot : range.TagsDictionary.GetValueOrDefault(temp[^1]);
    }

    /// <summary>
    /// 分隔并获取标签
    /// </summary>
    /// <param name="name"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static IEnumerable<TagViewModel> GetTagViewModels(this string name, TagsTreeDictionary? range = null)
    {
        range ??= AppContext.Tags;
        foreach (var tagName in name.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            if (range.TagsDictionary.GetValueOrDefault(tagName) is { } tagModel)
                yield return tagModel;
    }

    /// <summary>
    /// 输入标签时的建议列表
    /// </summary>
    /// <param name="name">目前输入的最后一个标签</param>
    /// <param name="separator">标签间分隔符</param>
    /// <param name="range">搜索范围</param>
    /// <returns>标签建议列表</returns>
    public static IEnumerable<TagViewModel> TagSuggest(this string name, char separator, TagsTreeDictionary? range = null)
    {
        range ??= AppContext.Tags;
        var tempName = name.Split(separator, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        if (tempName is "" or null)
            yield break;
        foreach (var tag in range.TagsDictionaryValues.Where(tag => tag.Name.Contains(tempName)))
            yield return tag;
        foreach (var tag in range.TagsDictionaryValues.Where(tag => tag.Path.Contains(tempName) && !tag.Name.Contains(tempName)))
            yield return tag;
    }
}
