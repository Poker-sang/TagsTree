using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Linq;
using TagsTree.Services;
using System.Collections.Generic;
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
		public static string FilesPath => Default.ConfigPath + @"\Files.bin";
		public static string RelationPath => Default.ConfigPath + @"\Relation.bin";

		/// <summary>
		/// 鼠标上一次的位置
		/// </summary>
		public static Point LastMousePos { get; set; }

		/// <summary>
		/// 鼠标位移是否超过一定距离
		/// </summary>
		/// <param name="distance">位移阈值</param>
		/// <param name="currentPos">现在鼠标位置</param>
		/// <returns>是否超过阈值</returns>
		public static bool MouseDisplace(double distance, Point currentPos) => Math.Abs(currentPos.X - LastMousePos.X) > distance || Math.Abs(currentPos.Y - LastMousePos.Y) > distance;

		/// <summary>
		/// 显示一条错误信息
		/// </summary>
		public static void ErrorMessageBox(string message) => _ = MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);

		/// <summary>
		/// 存储标签结构的Xml文档
		/// </summary>
		public static XmlDocument XdTags { get; } = new();

		static App()
		{
			XdTags.Load(TagsPath);
			RecursiveLoadTags();
		}

		/// <summary>
		/// XmlDataProvider根元素
		/// </summary>
		public static XmlElement? XdpRoot => (XmlElement?)TagsManagerService.Vm.Xdp.Document.LastChild;

		/// <summary>
		/// 保存文件
		/// </summary>
		public static void SaveXdTags() => XdTags.Save(TagsPath);

		/// <summary>
		/// 所有标签
		/// </summary>
		public static readonly List<Tag> TagsList = new();

		/// <summary>
		/// 检查新的标签名语法和与已有标签是否重复（已被删除空白字符）
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool NewTagCheck(string name)
		{
			if (name == string.Empty)
			{
				ErrorMessageBox("标签名称不能为空！）");
				return false;
			}
			if (TagPathComplete(name) is not null)
			{
				ErrorMessageBox("与现有标签重名！");
				return false;
			}

			return true;
		}

		/// <summary>
		/// 重新加载新的配置文件
		/// </summary>
		///<returns>是否已经处理</returns>
		public static bool LoadConfig(string configPath)
		{
			try
			{
				XdTags.Load(configPath + @"\TagsTree.xml");
			}
			catch (FileNotFoundException)
			{
				var result = MessageBox.Show("未检测到TagsTree.xml\n是否自动创建新的文件", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
					new XDocument(new XElement("TagsTree", new XAttribute("name", ""))).Save(configPath + @"\TagsTree.xml");
				else return false;
			}
			catch (XmlException)
			{
				var result = MessageBox.Show("TagsTree.xml文件损坏，\n是否自动创建新的文件覆盖", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					File.Delete(configPath + @"\TagsTree.xml");
					new XDocument(new XElement("TagsTree", new XAttribute("name", ""))).Save(configPath + @"\TagsTree.xml");
				}
				else return false;
			}
			catch (Exception)
			{
				var result = MessageBox.Show("TagsTree.xml文件未知错误，\n是否自动创建新的文件覆盖", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					File.Delete(configPath + @"\TagsTree.xml");
					new XDocument(new XElement("TagsTree", new XAttribute("name", ""))).Save(configPath + @"\TagsTree.xml");
				}
				else return false;
			}
			finally
			{
				Default.IsSet = false;
			}
			return true;
		}

		/// <summary>
		/// TreeView控件选择的元素改变时，显示所选项目Xml元素的路径<br/>
		/// 用法：<code>private void treeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs&lt;object&gt; e)<br/>
		/// => textBox.Text = TagsTreeStatic.TagsTree_OnSelectedItemChanged(treeView) ?? textBox.Text;</code>
		/// </summary>
		/// <param name="selectedItem">TreeView控件的SelectedItem</param>
		/// <returns>string类型，显示所选Xml元素的路径，为null则是没有选择项目</returns>
		public static string? TagsTree_OnSelectedItemChanged(XmlElement? selectedItem)
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
			TagsList.Clear();
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
					if (element!.GetAttribute("name") is { } attribute)
					{
						TagsList.Add(new Tag(attribute, @$"{path}\{attribute}"[1..], element));
						RecursiveLoadTags(@$"{path}\{attribute}", element);
					}
		}

		/// <summary>
		/// 补全标签的路径（为空则不补）
		/// </summary>
		/// <param name="name">需要找的单个标签</param>
		/// <returns>找到的完整路径，若返回null即没找到路径</returns>
		public static string? TagPathComplete(string name)
		{
			var temp = name.Split('\\', StringSplitOptions.RemoveEmptyEntries);
			if (temp.Length == 0)
				return name;
			name = temp.Last();
			var legal = new Regex(@"^[^\\\/\:\*\?\""\<\>\|\s]+$");
			if (!legal.IsMatch(name))
				throw new InvalidDataException();
			return TagsList.Where(tag => tag.Name == name).Select(tag => tag.Path).FirstOrDefault();
		}

		/// <summary>
		/// 递归用路径查找标签所在元素
		/// </summary>
		/// <param name="path">需要查找的元素所在路径（已由TagPathComplete()补全）</param>
		/// <returns>标签所在元素</returns>
		public static XmlElement? GetXmlElement(string path) => TagsList.Where(tag => tag.Path == path).Select(tag => tag.XmlElement).FirstOrDefault();

		public static IEnumerable<Tag> TagSuggest(string? name)
		{
			if (name is "" or null)
				return new List<Tag>();
			var temp = TagsList.Where(tag => tag.Name.Contains(name)).ToList();
			temp.AddRange(TagsList.Where(tag => tag.Path.Contains(name) && !tag.Name.Contains(name)));
			return temp;
		}


	}
}