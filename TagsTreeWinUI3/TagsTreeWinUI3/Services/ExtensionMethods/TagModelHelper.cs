using System;
using System.Collections.Generic;
using System.Linq;
using TagsTreeWinUI3.Models;

namespace TagsTreeWinUI3.Services.ExtensionMethods
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

		/// <summary>
		/// 输入标签时的建议列表
		/// </summary>
		/// <param name="name">目前输入的最后一个标签</param>
		/// <param name="separator">标签间分隔符</param>
		/// <returns>建议列表</returns>
		public static IEnumerable<TagModel> TagSuggest(this string name, char separator)
		{
			var tempName = name.Split(separator, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
			if (tempName is "" or null)
				yield break;
			foreach (var tag in App.Tags.TagsDictionaryValues.Where(tag => tag.Name.Contains(tempName)))
				yield return tag;
			foreach (var tag in App.Tags.TagsDictionaryValues.Where(tag => tag.Path.Contains(tempName) && !tag.Name.Contains(tempName)))
				yield return tag;
		}
	}
}
