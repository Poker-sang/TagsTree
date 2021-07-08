using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using ModernWpf.Controls;
using TagsTree.ViewModels;
using TreeView = System.Windows.Controls.TreeView;

namespace TagsTree.Services
{
	internal static class TagsManagerServices
	{
		private static readonly TagsManagerViewModel Vm = new();

		public static TagsManagerViewModel Load(TreeView treeView)
		{
			TagsTreeStatic.XdpLoad(treeView);
			return Vm;
		}

		#region 补全

		public static void NameComplement()
		{
			Vm.Name = Regex.Replace(Vm.Name, @"\s", "");
		}
		public static void PathComplement()
		{
			Vm.Path = Regex.Replace(Vm.Path, @"\s", "");
			var path = TagsTreeStatic.TagPathComplete(Vm.Path);
			if (path is null)
				TagsTreeStatic.ErrorMessageBox("标签路径不存在！请填写正确的单个标签或完整的路径！\n" + @"（不包含/:*?\""<>|,和空白字符）");
			else Vm.Path = path;
		}
		public static void TreeViewComplement(TreeView treeView)
		{
			Vm.Path = TagsTreeStatic.TagsTree_OnSelectedItemChanged(treeView) ?? Vm.Path;
		}

		#endregion

		#region 命令

		public static void NewBClick(object? parameter)
		{
			if (!TagsTreeStatic.NewTagCheck(Vm.Name))
				return;

			NewTag(Vm.Name, TagsTreeStatic.RecursiveSearchXmlElement(Vm.Path)!);
			Vm.Name = "";
		}
		public static void MoveBClick(object? parameter)
		{
			var element = TagsTreeStatic.RecursiveSearchXmlElement(Vm.Name);
			if (element is null)
				TagsTreeStatic.ErrorMessageBox("标签名称不存在！请填写正确的单个标签或完整的路径！");
			else
			{
				MoveTag(element, TagsTreeStatic.RecursiveSearchXmlElement(Vm.Path)!);
				Vm.Name = "";
			}
		}
		public static void RenameBClick(object? parameter)
		{
			if (!TagsTreeStatic.NewTagCheck(Vm.Name))
				return;
			RenameTag(Vm.Name, TagsTreeStatic.RecursiveSearchXmlElement(Vm.Path)!);
			Vm.Name = "";
			Vm.Path = "";
		}
		public static void DeleteBClick(object? parameter)
		{
			if (Vm.Path == string.Empty)
			{
				TagsTreeStatic.ErrorMessageBox("未输入希望删除的标签");
				return;
			}
			DeleteTag(TagsTreeStatic.RecursiveSearchXmlElement(Vm.Path)!);
			Vm.Name = "";
		}
		public static void SaveBClick(object? parameter)
		{
			TagsTreeStatic.XdpSave();
			Vm.Changed = false;
		}

		public static void NewCmClick(object? parameter)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			NewTag(dialog.Message, TagsTreeStatic.TvItemGetHeader(parameter)!);
		}
		public static void NewXCmClick(object? parameter)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			NewTag(dialog.Message, TagsTreeStatic.XdpRoot!);
		}
		public static void CutCmClick(object? parameter) => Vm.ClipBoard = TagsTreeStatic.TvItemGetHeader(parameter);
		public static void PasteCmClick(object? parameter)
		{
			MoveTag(Vm.ClipBoard!, TagsTreeStatic.TvItemGetHeader(parameter));
			Vm.ClipBoard = null;
		}
		public static void PasteXCmClick(object? parameter)
		{
			MoveTag(Vm.ClipBoard!, TagsTreeStatic.XdpRoot!);
			Vm.ClipBoard = null;
		}
		public static void RenameCmClick(object? parameter)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			RenameTag(dialog.Message, TagsTreeStatic.TvItemGetHeader(parameter)!);
		}
		public static void DeleteCmClick(object? parameter) => DeleteTag(TagsTreeStatic.TvItemGetHeader(parameter));

		#endregion

		#region 操作

		public static void NewTag(string name, XmlElement path)
		{
			var element = TagsTreeStatic.XdpDocument.CreateElement("Tag");
			element.SetAttribute("name", name);
			_ = path.AppendChild(element);
			Vm.Changed = true;
		}
		public static void MoveTag(XmlElement name, XmlElement path)
		{
			try
			{
				_ = path.AppendChild(name); //原位置自动被删除
				Vm.Changed = true;
			}
			catch (ArgumentException)
			{
				TagsTreeStatic.ErrorMessageBox("禁止将标签移动到自己目录下");
			}
		}
		public static void RenameTag(string name, XmlElement path)
		{
			path.RemoveAllAttributes();
			path.SetAttribute("name", name);
			Vm.Changed = true;
			Vm.Name = "";
			Vm.Path = "";
		}
		public static void DeleteTag(XmlElement path)
		{
			_ = path.ParentNode!.RemoveChild(path);
			Vm.Changed = true;
			Vm.Name = "";
		}

		#endregion
	}
}
