using ModernWpf.Controls;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TagsTree.Models;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
using TagsTree.Views;

namespace TagsTree.Services
{
	public static class TagsManagerService
	{
		private static TagsManagerViewModel Vm;
		private static TagsManager Win;
		public static void Load(TagsManager window)
		{
			Win = window;
			Vm = (TagsManagerViewModel)window.DataContext;
		}

		#region 事件处理

		public static void NameChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
		{
			Vm.Name = Regex.Replace(Vm.Name, $@"[{App.FileX.GetInvalidNameChars}]+", "");
			var textBox = (TextBox)typeof(AutoSuggestBox).GetField("m_textBox", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sender)!;
			textBox.SelectionStart = textBox.Text.Length;
		}

		public static void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => Win.TbPath.Path = App.TagMethods.TvSelectedItemChanged((XmlElement?)e.NewValue) ?? Win.TbPath.Path;

		private static XmlElement TvItemGetHeader(object? sender) => (XmlElement)((TreeViewItem)sender!).Header;
		public static void Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!Vm.Changed) return;
			switch (App.MessageBoxX.Question("是否保存更改？"))
			{
				case true: SaveBClick(null); break;
				case null: e.Cancel = true; break;
			}
		}

		#endregion

		#region 命令

		public static void NewBClick(object? parameter)
		{
			if (Win.TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				App.MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			if (!NewTagCheck(Vm.Name)) return;
			NewTag(Vm.Name, pathTagModel.XmlElement);
			Vm.Name = "";
		}
		public static void MoveBClick(object? parameter)
		{
			if (Win.TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				App.MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			if (Vm.Name.GetTagModel() is not { } nameTagModel)
			{
				App.MessageBoxX.Error("「标签名称」不存在！");
				return;
			}
			MoveTag(nameTagModel.XmlElement, pathTagModel.XmlElement);
			Vm.Name = "";
		}
		public static void RenameBClick(object? parameter)
		{
			if (Win.TbPath.Path is "")
			{
				App.MessageBoxX.Error("未输入希望重命名的标签");
				return;
			}
			if (Win.TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				App.MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			if (!NewTagCheck(Vm.Name)) return;
			RenameTag(Vm.Name, pathTagModel.XmlElement);
			Vm.Name = "";
			Win.TbPath.Path = "";
		}
		public static void DeleteBClick(object? parameter)
		{
			if (Win.TbPath.Path is "")
			{
				App.MessageBoxX.Error("未输入希望删除的标签");
				return;
			}
			if (Win.TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				App.MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			DeleteTag(pathTagModel.XmlElement);
			Vm.Name = "";
		}
		public static void SaveBClick(object? parameter)
		{
			App.SaveXdTags();
			App.SaveRelations();
			Vm.Changed = false;
		}

		public static void NewCmClick(object? parameter)
		{
			var dialog = new InputName(Win, App.FileX.InvalidMode.Name);
			if (dialog.ShowDialog() != false && NewTagCheck(dialog.Message))
				NewTag(dialog.Message, TvItemGetHeader(parameter));
		}
		public static void NewXCmClick(object? parameter)
		{
			var dialog = new InputName(Win, App.FileX.InvalidMode.Name);
			if (dialog.ShowDialog() != false && NewTagCheck(dialog.Message))
				NewTag(dialog.Message, App.XdpRoot!);
		}
		public static void CutCmClick(object? parameter) => Vm.ClipBoard = TvItemGetHeader(parameter);
		public static void PasteCmClick(object? parameter)
		{
			MoveTag(Vm.ClipBoard!, TvItemGetHeader(parameter));
			Vm.ClipBoard = null;
		}
		public static void PasteXCmClick(object? parameter)
		{
			MoveTag(Vm.ClipBoard!, App.XdpRoot!);
			Vm.ClipBoard = null;
		}
		public static void RenameCmClick(object? parameter)
		{
			var dialog = new InputName(Win, App.FileX.InvalidMode.Name);
			if (dialog.ShowDialog() != false && NewTagCheck(dialog.Message))
				RenameTag(dialog.Message, TvItemGetHeader(parameter)!);
		}
		public static void DeleteCmClick(object? parameter) => DeleteTag(TvItemGetHeader(parameter));

		#endregion

		#region 操作

		private static void NewTag(string name, XmlElement path)
		{
			var element = Vm.Xdp.Document.CreateElement("Tag");
			var temp = new TagModel(name, path.GetAttribute("name"), element);
			element.SetAttribute("name", name);
			element.SetAttribute("id", temp.Id.ToString());
			_ = path.AppendChild(element);
			App.Tags[temp.Id, name] = temp;
			App.Relations.NewColumn(temp.Id);
			Vm.Changed = true;
		}
		public static void MoveTag(XmlElement name, XmlElement? path)
		{
			try
			{
				path ??= (XmlElement?)Vm.Xdp.Document.LastChild;
				_ = path!.AppendChild(name); //原位置自动被删除
				App.Tags[name.GetAttribute("name")].Path = path.GetAttribute("name").GetTagModel()!.FullName;
				Vm.Changed = true;
			}
			catch (ArgumentException)
			{
				App.MessageBoxX.Error("禁止将标签移动到自己目录下");
			}
		}
		private static void RenameTag(string name, XmlElement path)
		{
			var temp = App.Tags[path.GetAttribute("name")];
			path.SetAttribute("name", name);
			App.Tags.ChangeKey2(temp.Name, name);
			temp.Name = name;
			Vm.Changed = true;
		}
		private static void DeleteTag(XmlElement path)
		{
			_ = path.ParentNode!.RemoveChild(path);
			var temp = Convert.ToInt32(path.GetAttribute("id"));
			App.Relations.DeleteColumn(temp);
			App.Tags.Remove(temp);
			Vm.Changed = true;
		}
		
		private static bool NewTagCheck(string name)
		{
			if (name is "")
			{
				App.MessageBoxX.Error("标签名称不能为空！");
				return false;
			}
			if (name.GetTagModel() is not null)
			{
				App.MessageBoxX.Error("与现有标签重名！");
				return false;
			}
			return true;
		}

		#endregion
	}
}
