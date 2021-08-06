using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using TagsTree.Models;
using static TagsTree.Properties.Settings;

namespace TagsTree
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static string TagsPath => Default.ConfigPath + @"\TagsTree.xml";
		public static string FilesPath => Default.ConfigPath + @"\Files.json";
		public static string RelationsPath => Default.ConfigPath + @"\Relations.xml";

		/// <summary>
		/// 存储标签结构的Xml文档
		/// </summary>
		public static XmlDocument XdTags { get; } = new();

		/// <summary>
		/// 重新加载标签
		/// </summary>
		public static void XdTagsReload()
		{
			XdTags.Load(TagsPath);
			RecursiveLoadTags();
			Relations = RelationsDataTable.Load()!;
		}

		/// <summary>
		/// XmlDataProvider根元素
		/// </summary>
		public static XmlElement? XdpRoot => (XmlElement?)XdTags.LastChild;

		/// <summary>
		/// 保存标签
		/// </summary>
		public static void SaveXdTags() => XdTags.Save(TagsPath);

		/// <summary>
		/// 保存文件
		/// </summary>
		public static void SaveFiles() => IdFile.Serialize(FilesPath);

		/// <summary>
		/// 保存关系
		/// </summary>
		public static void SaveRelations() => Relations.Save(RelationsPath);

		/// <summary>
		/// 所有标签
		/// </summary>
		public static readonly Dictionary<string, TagModel> Tags = new();

		/// <summary>
		/// 所有标签
		/// </summary>
		public static readonly BidirectionalDictionary<int, FileModel> IdFile = new();

		/// <summary>
		/// 所有关系
		/// </summary>
		public static RelationsDataTable Relations;

		///  <summary>
		///  重新加载新的配置文件
		///  </summary>
		///  <returns>true：已填写正确地址，进入软件；false：打开设置页面；null：关闭软件</returns>
		public static bool? LoadConfig()
		{
			if (!Directory.Exists(Default.ConfigPath))
			{
				if (MessageBoxX.Warning($"路径{Default.ConfigPath}不存在", "修改设置", "关闭软件"))
				{
					Default.IsSet = false;
					return false;
				}
				else
				{
					Default.IsSet = false;
					return null;
				}
			}

			if (!File.Exists(TagsPath))
				new XDocument(new XElement("TagsTree", new XAttribute("name", ""))).Save(TagsPath);
			try
			{
				XdTags.Load(TagsPath);
			}
			catch (Exception)
			{
				File.Delete(TagsPath);
				new XDocument(new XElement("TagsTree", new XAttribute("name", ""))).Save(TagsPath);
			}
			RecursiveLoadTags();

			if (!File.Exists(RelationsPath))
				_ = File.Create(RelationsPath);
			Relations = RelationsDataTable.Load()!; //异常在内部处理

			IdFile.Deserialize(FilesPath);
			FileModel.Num = IdFile.Count is 0 ? 0 : IdFile.Keys.Last() + 1;

			static bool DeleteAll()
			{
				File.Delete(TagsPath);
				File.Delete(FilesPath);
				File.Delete(RelationsPath);
				return false;
			}

			if (Tags.Count != Relations.Columns.Count - 1) //第一列是文件Id 
			{
				if (MessageBoxX.Warning($"路径{Default.ConfigPath}下，TagsTree.xml和Relations.xml存储的标签数不同", "删除标签与文件的配置文件", "直接关闭软件"))
					return DeleteAll();
				return null;
			}
			if (IdFile.Count != Relations.Rows.Count)
			{
				if (MessageBoxX.Warning($"路径{Default.ConfigPath}下，Files.json和Relations.xml存储的文件数不同", "删除标签与文件的配置文件", "直接关闭软件"))
					return DeleteAll();
				return null;
			}
			return true;
		}

		/// <summary>
		/// TreeView控件选择的元素改变时，显示所选项目Xml元素的路径<br/>
		/// 用法：<code>private void treeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs&lt;object&gt; e)<br/>
		/// => textBox.Text = App.TvSelectedItemChanged(treeView) ?? textBox.Text;</code>
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
		/// 补全标签的路径（为空则不补）
		/// </summary>
		/// <param name="name">需要找的单个标签</param>
		/// <returns>找到的标签，若返回null即没找到路径</returns>
		public static TagModel? TagPathComplete(string name)
		{
			var temp = name.Split('\\', StringSplitOptions.RemoveEmptyEntries);
			if (temp.Length == 0)
				return null;
			name = temp.Last();
			return Tags.ContainsKey(name) ? Tags[name] : null;
		}

		/// <summary>
		/// 递归用路径查找标签所在元素
		/// </summary>
		/// <param name="path">需要查找的元素所在路径</param>
		/// <returns>标签所在元素</returns>
		public static XmlElement? GetXmlElement(string path)
		{
			var temp = path.Split('\\', StringSplitOptions.RemoveEmptyEntries);
			if (temp.Length == 0)
				return XdpRoot;
			path = temp.Last();
			return Tags.ContainsKey(path) ? Tags[path].XmlElement : null;
		}

		/// <summary>
		/// 输入标签时的建议列表
		/// </summary>
		/// <param name="name">目前输入的最后一个标签</param>
		/// <returns>建议列表</returns>
		public static IEnumerable<TagModel> TagSuggest(string name)
		{
			var tempName = name.Split('\\', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
			if (tempName is "" or null)
				yield break;
			foreach (var tag in Tags.Values.Where(tag => tag.Name.Contains(tempName)))
				yield return tag;
			foreach (var tag in Tags.Values.Where(tag => tag.Path.Contains(tempName) && !tag.Name.Contains(tempName)))
				yield return tag;
		}
	}
}