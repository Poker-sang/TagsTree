using System;
using System.Collections.Generic;
using System.Linq;
using TagsTreeWpf.Models;

namespace TagsTreeWpf.Services.ExtensionMethods
{
	public static class TagMethods
	{
		/// <summary>
		/// TreeView控件选择的元素改变时，显示所选项目Xml元素的路径<br/>
		/// </summary>
		/// <param name="path"></param>
		/// <param name="selectedItem">TreeView控件的SelectedItem</param>
		/// <returns>显示所选标签的全名，如果没选择项目则返回原值</returns>
		public static string TvSelectedItemChanged(this string path, TagModel? selectedItem) => selectedItem?.Name.GetTagModel()!.FullName ?? path;

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
