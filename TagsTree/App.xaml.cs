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
		/// <param name="message">错误信息</param>
		public static void ErrorMessageBox(string message) => _ = MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);

		/// <summary>
		/// 显示一条可选择的警告信息
		/// </summary>
		/// <param name="message">警告信息</param>
		/// <param name="okHint">选择确认结果</param>
		/// <param name="cancelHint">选择取消结果</param>
		/// <returns>只可能有Ok或Cancel两种结果（直接关闭结果为Cancel）</returns>
		private static MessageBoxResult WarningMessageBox(string message, string okHint = "", string cancelHint = "")
		{
			var ok = okHint is "" ? "" : $"\n按“确认”{okHint}";
			var cancel = okHint is "" ? "" : $"\n按“取消”{cancelHint}";
			return MessageBox.Show(message + ok + cancel, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
		}

		/// <summary>
		/// 显示一条信息
		/// </summary>
		/// <param name="message">信息</param>
		public static void InformationMessageBox(string message) => _ = MessageBox.Show(message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);

		/// <summary>
		/// 存储标签结构的Xml文档
		/// </summary>
		public static XmlDocument XdTags { get; } = new();

		/// <summary>
		/// XmlDataProvider根元素
		/// </summary>
		public static XmlElement? XdpRoot => (XmlElement?)TagsManagerService.Vm.Xdp.Document.LastChild;

		/// <summary>
		/// 保存标签
		/// </summary>
		public static void SaveXdTags() => XdTags.Save(TagsPath);

		/// <summary>
		/// 保存文件
		/// </summary>
		public static void SaveFiles() => Serialize(FilesPath, IdToFile);

		/// <summary>
		/// 保存关系
		/// </summary>
		public static void SaveRelations() => Relations.Save();

		/// <summary>
		/// 所有标签
		/// </summary>
		public static readonly Dictionary<string, TagModel> Tags = new();

		/// <summary>
		/// 所有标签
		/// </summary>
		public static readonly Dictionary<int, FileModel> IdToFile = new();

		/// <summary>
		/// 所有关系
		/// </summary>
		public static RelationsDataTable Relations;

		/// <summary>
		/// 重新加载新的配置文件
		/// </summary>
		///<returns>true：已填写正确地址，进入软件；false：打开设置页面；null：关闭软件</returns>
		public static bool? LoadConfig(string configPath)
		{
			var fullPath = configPath + @"\TagsTree.xml";

			if (!Directory.Exists(configPath))
				switch (WarningMessageBox($"路径{configPath}不存在", "修改设置", "关闭软件"))
				{
					case MessageBoxResult.OK: Default.IsSet = false; return false;
					case MessageBoxResult.Cancel: Default.IsSet = false; return null;
					default: throw new ArgumentOutOfRangeException();
				}

			if (!File.Exists(configPath + @"\TagsTree.xml"))
				new XDocument(new XElement("TagsTree", new XAttribute("name", ""))).Save(fullPath);
			try
			{
				XdTags.Load(configPath + @"\TagsTree.xml");
			}
			catch (Exception)
			{
				File.Delete(fullPath);
				new XDocument(new XElement("TagsTree", new XAttribute("name", ""))).Save(fullPath);
			}
			RecursiveLoadTags();

			if (!File.Exists(configPath + @"\Relations.xml"))
				_ = File.Create(configPath + @"\Relations.xml");
			Relations = RelationsDataTable.Load()!; //异常在内部处理

			foreach (var (key, file) in Task.Run(async () => await Deserialize<Dictionary<int, FileModel>>(FilesPath)).GetAwaiter().GetResult())
				IdToFile[key] = file;
			FileModel.Num = IdToFile.Keys.LastOrDefault() + 1;

			if (Tags.Count != Relations.Columns.Count - 1) //第一列是文件Id
				return WarningMessageBox($"路径{configPath}下，TagsTree.xml和Relations.xml存储的标签数不同", "删除标签与文件的配置文件", "直接关闭软件") switch
				{
					MessageBoxResult.OK => false,
					MessageBoxResult.Cancel => null,
					_ => throw new ArgumentOutOfRangeException()
				};
			if (IdToFile.Count != Relations.Rows.Count)
				return WarningMessageBox($"路径{configPath}下，Files.json和Relations.xml存储的文件数不同", "删除标签与文件的配置文件", "直接关闭软件") switch
				{
					MessageBoxResult.OK => false,
					MessageBoxResult.Cancel => null,
					_ => throw new ArgumentOutOfRangeException()
				};
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
			if (!new Regex(@"^[^\\\/\:\*\?\""\<\>\|\s]+$").IsMatch(name))
				throw new InvalidDataException();
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

		/// <summary>
		/// 异步将Json文件反序列化为某个类
		/// </summary>
		/// <typeparam name="T">带无参构造的类</typeparam>
		/// <param name="path">Json文件位置</param>
		/// <returns>返回文件中的数据，如果没有则返回新实例</returns>
		private static async ValueTask<T> Deserialize<T>(string path) where T : new()
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
		private static async void Serialize<T>(string path, T objectItem) => await JsonSerializer.SerializeAsync(File.Create(path), objectItem);
	}
}