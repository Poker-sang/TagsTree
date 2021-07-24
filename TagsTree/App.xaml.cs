using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using TagsTree.Models;
using TagsTree.Services;
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
		public static readonly Dictionary<string, Tag> TagsList = new();

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
		///<returns>true：已填写正确地址，进入软件；false：打开设置页面编辑；null：关闭软件</returns>
		public static bool? LoadConfig(string configPath)
		{
			var fullpath = configPath + @"\TagsTree.xml";
			try
			{
				XdTags.Load(fullpath);
				RecursiveLoadTags();
			}
			catch (FileNotFoundException)
			{
				var result = MessageBox.Show($"未检测到{fullpath}\n按“是”自动创建新的文件\n按“否”修改设置\n按“取消”关闭软件", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
				switch (result)
				{
					case MessageBoxResult.Yes: new XDocument(new XElement("TagsTree", new XAttribute("name", ""))).Save(fullpath); break;
					case MessageBoxResult.No: return false;
					case MessageBoxResult.Cancel: return null;
				}
			}
			catch (XmlException)
			{
				var result = MessageBox.Show($"{fullpath}文件损坏，\n按“是”自动覆盖新的文件\n按“否”修改设置\n按“取消”关闭软件", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
				switch (result)
				{
					case MessageBoxResult.Yes:
						File.Delete(fullpath);
						new XDocument(new XElement("TagsTree", new XAttribute("name", ""))).Save(fullpath);
						break;
					case MessageBoxResult.No: return false;
					case MessageBoxResult.Cancel: return null;
				}
			}
			catch (DirectoryNotFoundException)
			{
				var result = MessageBox.Show($"TagsTree.xml所在路径{configPath}未找到，\n按“确认”修改设置\n按“取消”关闭软件", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
				switch (result)
				{
					case MessageBoxResult.Yes: return false;
					case MessageBoxResult.Cancel: return null;
				}
			}
			catch (Exception)
			{
				var result = MessageBox.Show($"{fullpath}文件发生其他错误，\n按“是”自动覆盖新的文件\n按“否”修改设置\n按“取消”关闭软件", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
				switch (result)
				{
					case MessageBoxResult.Yes:
						File.Delete(fullpath);
						new XDocument(new XElement("TagsTree", new XAttribute("name", ""))).Save(fullpath);
						break;
					case MessageBoxResult.No: return false;
					case MessageBoxResult.Cancel: return null;
				}
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
					if (element!.GetAttribute("name") is { } name)
					{
						TagsList[name] = new Tag(name, path, element);
						RecursiveLoadTags((path is "" ? "" : path + '\\') + name, element);
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
			if (!new Regex(@"^[^\\\/\:\*\?\""\<\>\|\s]+$").IsMatch(name))
				throw new InvalidDataException();
			try
			{
				return TagsList[name].FullName;
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
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
				return null;
			path = temp.Last();
			try
			{
				return TagsList[path].XmlElement;
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}

		/// <summary>
		/// 输入标签时的建议列表
		/// </summary>
		/// <param name="name">目前输入的最后一个标签</param>
		/// <returns>建议列表</returns>
		public static IEnumerable<Tag> TagSuggest(string name)
		{
			var tempName = name.Split('\\', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
			if (tempName is "" or null)
				return new List<Tag>();
			var temp = TagsList.Values.Where(tag => tag.Name.Contains(tempName)).ToList();
			temp.AddRange(TagsList.Values.Where(tag => tag.Path.Contains(tempName) && !tag.Name.Contains(tempName)));
			return temp;
		}

		/// <summary>
		/// 异步将Json文件反序列化为某个类
		/// </summary>
		/// <typeparam name="T">带无参构造的类</typeparam>
		/// <param name="path">Json文件位置</param>
		/// <returns>返回文件中的数据，如果没有则返回新实例</returns>
		public static async ValueTask<T> Deserialize<T>(string path) where T : new()
		{
			try
			{
				await using var fileStream = File.OpenRead(path);
				return await JsonSerializer.DeserializeAsync<T>(fileStream) ?? new T();
			}
			catch (Exception)
			{
				return new T();
			}
		}

		/// <summary>
		/// 异步将某个类序列化为Json文件
		/// </summary>
		/// <typeparam name="T">泛型；类</typeparam>
		/// <param name="path">Json文件路径</param>
		/// <param name="objectItem">需要转化的对象</param>
		/// <returns></returns>
		public static async void Serialize<T>(string path, T objectItem) => await JsonSerializer.SerializeAsync(File.Create(path), objectItem);
	}
}