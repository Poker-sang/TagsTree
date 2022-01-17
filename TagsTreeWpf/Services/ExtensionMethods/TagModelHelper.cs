using System;
using System.Collections.Generic;
using System.Linq;
using TagsTreeWpf.Models;

namespace TagsTreeWpf.Services.ExtensionMethods
{
    public static class TagModelHelper
    {
        /// <summary>
        /// 补全标签的路径（为空则为根标签）
        /// </summary>
        /// <param name="name">需要找的单个标签</param>
        /// <returns>找到的标签，若返回null即没找到路径</returns>
        public static TagModel? GetTagModel(this string name)
        {
            var temp = name.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length == 0)
                return App.Tags.TagsDictionaryRoot;
            return App.Tags.TagsDictionary.ContainsKey(temp.Last()) ? App.Tags.TagsDictionary[temp.Last()] : null;
        }
        public static TagModel? GetTagModel(this int id) => App.Tags.TagsDictionary.ContainsKey(id) ? App.Tags.TagsDictionary[id] : null;

        public static IEnumerable<TagModel> GetTagModels(this string name)
        {
            var temp = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return temp.Where(tag => App.Tags.TagsDictionary.ContainsKey(tag)).Select(tag => App.Tags.TagsDictionary[tag]);
        }
        public static IEnumerable<PathTagModel> GetTagsFiles(this string name)
        {
            var temp = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string tag in temp)
                if (App.Tags.TagsDictionary.ContainsKey(tag))
                    yield return App.Tags.TagsDictionary[tag];
                else yield return new PathTagModel(tag);
        }
    }
}
