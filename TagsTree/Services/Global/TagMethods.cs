using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TagsTree.Models;

namespace TagsTree
{
	public partial class App
	{
		public static class TagMethods
		{
			/// <summary>
			/// TreeView控件选择的元素改变时，显示所选项目Xml元素的路径<br/>
			/// 用法：<code>private void treeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs&lt;object&gt; e)<br/>
			/// => textBox.Path = App.TvSelectedItemChanged(treeView) ?? textBox.Path;</code>
			/// </summary>
			/// <param name="selectedItem">TreeView控件的SelectedItem</param>
			/// <returns>string类型，显示所选Xml元素的路径，为null则是没有选择项目</returns>
			public static string? TvSelectedItemChanged(XmlElement? selectedItem)
			{
				if (selectedItem is null)
					return null;
				var text = selectedItem.GetAttribute("name");
				var currentElement = (XmlElement?)selectedItem.ParentNode;
				while (currentElement!.Name == "Tag")
				{
					text = currentElement.GetAttribute("name") + @"\" + text;
					currentElement = (XmlElement?)currentElement.ParentNode;
				}
				return text;
			}

			/// <summary>
			/// 清空并重新读取标签
			/// </summary>
			public static void RecursiveLoadTags()
			{
				Tags.Clear();
				RecursiveLoadTags("", XdpRoot);
			}

			/// <summary>
			/// 递归读取标签
			/// </summary>
			/// <param name="path">标签所在路径</param>
			/// <param name="xmlElement">标签所在路径对应的元素</param>
			private static void RecursiveLoadTags(string path, XmlElement? xmlElement)
			{
				if (xmlElement is { HasChildNodes: true })
					foreach (XmlElement? element in xmlElement.ChildNodes)
						if (element!.GetAttribute("name") is { } name)
						{
							Tags[name] = new TagModel(name, path, element);
							RecursiveLoadTags((path is "" ? "" : path + '\\') + name, element);
						}
			}

			/// <summary>
			/// 输入标签时的建议列表
			/// </summary>
			/// <param name="name">目前输入的最后一个标签</param>
			/// <param name="separator">标签间分隔符</param>
			/// <returns>建议列表</returns>
			public static IEnumerable<TagModel> TagSuggest(string name, char separator)
			{
				var tempName = name.Split(separator, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
				if (tempName is "" or null)
					yield break;
				foreach (var tag in Tags.Values.Where(tag => tag.Name.Contains(tempName)))
					yield return tag;
				foreach (var tag in Tags.Values.Where(tag => tag.Path.Contains(tempName) && !tag.Name.Contains(tempName)))
					yield return tag;
			}
		}
	}
}
