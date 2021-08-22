using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using TagsTree.Models;
using TagsTree.ViewModels;
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
			TagMethods.RecursiveLoadTags();
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
		public static readonly DoubleKeysDictionary<int, string, TagModel> Tags = new();

		/// <summary>
		/// 所有标签
		/// </summary>
		public static readonly BidirectionalDictionary<int, FileModel> IdFile = new();

		/// <summary>
		/// 所有关系
		/// </summary>
		public static readonly RelationsDataTable Relations = new();

		public static bool TryRemoveFileModel(FileViewModel fileViewModel)
		{
			var fileModel = fileViewModel.GetFileModel;
			if (!IdFile.Contains(fileModel)) return false;
			_ = IdFile.Remove(fileModel);
			Relations.Rows.Remove(Relations.RowAt(fileModel));
			Relations.RefreshRowsDict();
			SaveFiles();
			SaveRelations();
			return true;
		}

		///  <summary>
		///  重新加载新的配置文件
		///  </summary>
		///  <returns>true：已填写正确地址，进入软件；false：打开设置页面；null：关闭软件</returns>
		public static bool? LoadConfig()
		{
			//总体设置
			if (!Directory.Exists(Default.ConfigPath))
			{
				if (MessageBoxX.Warning($"路径「{Default.ConfigPath}」不存在", "修改设置", "关闭软件"))
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
			//标签
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

			TagMethods.RecursiveLoadTags();
			//文件
			IdFile.Deserialize(FilesPath);
			FileModel.Num = IdFile.Count is 0 ? 0 : IdFile.Keys.Last() + 1; //或 IdFile.Values.Last().Id + 1
			
			//关系
			if (!File.Exists(RelationsPath))
				_ = File.Create(RelationsPath);
			Relations.Load(); //异常在内部处理
			//检查
			if (Tags.Count != Relations.Columns.Count - 1) //第一列是文件Id 
			{
				if (MessageBoxX.Warning($"路径「{Default.ConfigPath}」下，TagsTree.xml和Relations.xml存储的标签数不同", "删除关系文件Relations.xml并关闭软件", "直接关闭软件"))
					File.Delete(RelationsPath);
				return null;
			}
			if (IdFile.Count != Relations.Rows.Count)
			{
				if (MessageBoxX.Warning($"「路径{Default.ConfigPath}」下，Files.json和Relations.xml存储的文件数不同", "删除关系文件Relations.xml并关闭软件", "直接关闭软件"))
					File.Delete(RelationsPath);
				return null;
			}
			return true;
		}
	}
}