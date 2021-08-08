using System;
using System.Collections.Generic;
using System.Linq;
using TagsTree.Models;

namespace TagsTree.Services.ExtensionMethods
{
	public static class TagModelHelper
	{
		/// <summary>
		/// 补全标签的路径（为空则不补）
		/// </summary>
		/// <param name="name">需要找的单个标签</param>
		/// <returns>找到的标签，若返回null即没找到路径</returns>
		public static TagModel? GetTagModel(this string name)
		{
			var temp = name.Split('\\', StringSplitOptions.RemoveEmptyEntries);
			if (temp.Length == 0)
				return new TagModel("", "", App.XdpRoot!);
			return App.Tags.ContainsKey(temp.Last()) ? App.Tags[temp.Last()] : null;
		}
		public static IEnumerable<TagModel> GetTagModels(this string name)
		{
			var temp = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			return temp.Where(tag => App.Tags.ContainsKey(tag)).Select(tag => App.Tags[tag]);
		}
	}
}
