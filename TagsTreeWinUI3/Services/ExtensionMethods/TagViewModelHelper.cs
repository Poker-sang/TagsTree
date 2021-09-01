using System;
using System.Collections.Generic;
using System.Linq;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Services.ExtensionMethods
{
	public static class TagViewModelHelper
	{
		/// <summary>
		/// 补全标签的路径（为空则为根标签）
		/// </summary>
		/// <param name="name">需要找的单个标签</param>
		/// <returns>找到的标签，若返回null即没找到路径</returns>
		public static TagViewModel? GetTagViewModel(this string name)
		{
			var temp = name.Split('\\', StringSplitOptions.RemoveEmptyEntries);
			return temp.Length == 0 ? App.Tags.TagsDictionaryRoot : App.Tags.TagsDictionary.GetValueOrDefault(temp.Last());
		}
		public static IEnumerable<TagViewModel> GetTagViewModels(this string name)
		{
			foreach (string tagName in name.Split(' ', StringSplitOptions.RemoveEmptyEntries))
				if (App.Tags.TagsDictionary.GetValueOrDefault(tagName) is { } tagModel)
					yield return tagModel;
		}

		/// <summary>
		/// 输入标签时的建议列表
		/// </summary>
		/// <param name="name">目前输入的最后一个标签</param>
		/// <param name="separator">标签间分隔符</param>
		/// <returns>建议列表</returns>
		public static IEnumerable<TagViewModel> TagSuggest(this string name, char separator)
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
