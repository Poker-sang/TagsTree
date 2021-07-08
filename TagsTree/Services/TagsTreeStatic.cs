using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;
using static TagsTree.Properties.Settings;

namespace TagsTree.Services
{
	public static class TagsTreeStatic
	{
		public static XmlDocument TagsTreeDocument { get; } = new();
		public static XmlDocument XdpDocument { get; } = new();

		/// <summary>
		/// XmlDataProvider根元素
		/// </summary>
		public static XmlElement? XdpRoot => (XmlElement?)XdpDocument!.LastChild;

		/// <summary>
		/// 加载XmlDataProvider并绑定TreeView
		/// </summary>
		public static void XdpLoad(TreeView treeView)
		{
			XdpDocument.Load(Default.ConfigPath + @"\TagsTree.xml");
			treeView.DataContext = new XmlDataProvider { Document = XdpDocument, XPath = @"TagsTree/Tag" };
			_ = treeView.SetBinding(ItemsControl.ItemsSourceProperty, new Binding());
		}

		/// <summary>
		/// 保存文件
		/// </summary>
		public static void Save() => TagsTreeDocument.Save(Default.ConfigPath + @"\TagsTree.xml");

		/// <summary>
		/// 保存文件
		/// </summary>
		public static void XdpSave() => XdpDocument.Save(Default.ConfigPath + @"\TagsTree.xml");

		/// <summary>
		/// 显示一条错误信息
		/// </summary>
		public static void ErrorMessageBox(string message) => _ = MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);

		/// <summary>
		/// 上次鼠标点击位置
		/// </summary>
		public static Point LastMousePos { get; set; }

		/// <summary>
		/// 由TreeViewItem的sender得到其中的XmlElement
		/// </summary>
		public static XmlElement TvItemGetHeader(object? sender) => (XmlElement)((TreeViewItem)sender!).Header;

		/// <summary>
		/// 是否鼠标位移超过一定距离
		/// </summary>
		public static bool MouseDisplace(double distance, Point currentPos)
			=> Math.Abs(currentPos.X - LastMousePos.X) > distance ||
			   Math.Abs(currentPos.Y - LastMousePos.Y) > distance;

		/// <summary>
		/// 检查新的标签名语法和是否重复（已被删除空白字符）
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool NewTagCheck(string name)
		{
			if (!new Regex(@"^[^\\\/\:\*\?\""\<\>\|\s]+$").IsMatch(name))
			{
				TagsTreeStatic.ErrorMessageBox("标签名称错误！请填写正确的名称！\n" + @"（不包含\/:*?\""<>|,和空白字符且不为空）");
				return false;
			}

			if (TagsTreeStatic.TagPathComplete(name) is not null)
			{
				TagsTreeStatic.ErrorMessageBox("与现有标签重名！");
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
				TagsTreeDocument.Load(configPath + @"\TagsTree.xml");
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
		/// <param name="treeView">TreeView控件</param>
		/// <returns>string类型，显示所选Xml元素的路径，为null则是没有选择项目</returns>
		public static string? TagsTree_OnSelectedItemChanged(TreeView treeView)
		{
			var xmlElement = (XmlElement?)treeView.SelectedItem;
			if (xmlElement is null)
				return null;
			var text = xmlElement.GetAttribute("name");
			var currentElement = (XmlElement?)xmlElement.ParentNode;
			while (currentElement!.Name == "Tag")
			{
				text = currentElement.GetAttribute("name") + @"\" + text;
				currentElement = (XmlElement?)currentElement.ParentNode;
			}
			return text;
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
			var xmlElement = XdpRoot;
			var path = RecursiveSearchTags(xmlElement, name);
			return path?[1..];
		}

		/// <summary>
		/// 递归查找标签所在路径
		/// </summary>
		private static string? RecursiveSearchTags(XmlElement? xmlElement, string name, string path = "")
		{
			if (xmlElement is { HasChildNodes: true })
				foreach (XmlElement? element in xmlElement.ChildNodes)
					if (element!.GetAttribute("name") is { } attribute)
					{
						if (attribute == name)
							return @$"{path}\{attribute}";
						var result = RecursiveSearchTags(element, name, @$"{path}\{attribute}");
						if (result is not null)
							return result;
					}
			return null;
		}

		/// <summary>
		/// 递归用路径查找标签所在元素
		/// </summary>
		/// <param name="path">需要查找的元素所在路径（已由TagPathComplete()补全）</param>
		/// <param name="xmlElement">在该元素下查找，默认为根元素</param>
		/// <param name="series">递归层数（外部不需要传值）</param>
		/// <returns></returns>
		public static XmlElement? RecursiveSearchXmlElement(string path, XmlElement? xmlElement = null, int series = 0)
		{
			xmlElement ??= XdpRoot;//默认为根元素
			if (series == path.Split('\\', StringSplitOptions.RemoveEmptyEntries).Length)
				return xmlElement;
			else if (xmlElement is { HasChildNodes: true })
				foreach (XmlElement? element in xmlElement.ChildNodes)
					if (element?.GetAttribute("name") == path.Split('\\', StringSplitOptions.RemoveEmptyEntries)[series])
						return RecursiveSearchXmlElement(path, element, series + 1);
			return null;//理论上不会到达此代码
		}
	}
}