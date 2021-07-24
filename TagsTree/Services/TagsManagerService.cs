using ModernWpf.Controls;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TagsTree.ViewModels;
using TagsTree.Views;

namespace TagsTree.Services
{
	public static class TagsManagerService
	{
		public static readonly TagsManagerViewModel Vm = new();
		private static TagsManager Win;
		public static TagsManagerViewModel Load(TagsManager window)
		{
			Win = window;
			return Vm;
		}

		public static void PathComplement(object sender, RoutedEventArgs e)
		{
			var path = App.TagPathComplete(Vm.Path);
			if (path is not null)
				Vm.Path = path;
		}
		public static void NameChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e) => Vm.Name = Regex.Replace(Vm.Name, @"[\\\/\:\*\?\""\<\>\|\s]+", "");
		public static void PathChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
		{
			Vm.Path = Regex.Replace(Vm.Path, @"[\/\:\*\?\""\<\>\|\s]+", "");
			sender.ItemsSource = App.TagSuggest(sender.Text);
		}
		public static void SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e) => sender.Text = e.SelectedItem.ToString();
		public static void TvSelectItemChanged(object? selectElement) => Vm.Path = App.TagsTree_OnSelectedItemChanged((XmlElement?)selectElement) ?? Vm.Path;
		private static XmlElement TvItemGetHeader(object? sender) => (XmlElement)((TreeViewItem)sender!).Header;

		#region 命令

		public static void NewBClick(object? parameter)
		{
			if (!App.NewTagCheck(Vm.Name))
				return;

			NewTag(Vm.Name, App.GetXmlElement(Vm.Path)!);
			Vm.Name = "";
		}
		public static void MoveBClick(object? parameter)
		{
			var element = App.GetXmlElement(Vm.Name);
			if (element is null)
				App.ErrorMessageBox("标签名称不存在！请填写正确的单个标签或完整的路径！");
			else
			{
				MoveTag(element, App.GetXmlElement(Vm.Path)!);
				Vm.Name = "";
			}
		}
		public static void RenameBClick(object? parameter)
		{
			if (!App.NewTagCheck(Vm.Name))
				return;
			RenameTag(Vm.Name, App.GetXmlElement(Vm.Path)!);
			Vm.Name = "";
			Vm.Path = "";
		}
		public static void DeleteBClick(object? parameter)
		{
			if (Vm.Path == string.Empty)
			{
				App.ErrorMessageBox("未输入希望删除的标签");
				return;
			}
			DeleteTag(App.GetXmlElement(Vm.Path)!);
			Vm.Name = "";
		}
		public static void SaveBClick(object? parameter)
		{
			App.SaveXdTags();
			Vm.Changed = false;
		}

		public static void NewCmClick(object? parameter)
		{
			var dialog = new InputName(Win);
			if (dialog.ShowDialog() == false || !App.NewTagCheck(dialog.Message))
				return;

			NewTag(dialog.Message, TvItemGetHeader(parameter)!);
		}
		public static void NewXCmClick(object? parameter)
		{
			var dialog = new InputName(Win);
			if (dialog.ShowDialog() == false || !App.NewTagCheck(dialog.Message))
				return;

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
			var dialog = new InputName(Win);
			if (dialog.ShowDialog() == false || !App.NewTagCheck(dialog.Message))
				return;

			RenameTag(dialog.Message, TvItemGetHeader(parameter)!);
		}
		public static void DeleteCmClick(object? parameter) => DeleteTag(TvItemGetHeader(parameter));

		#endregion

		#region 操作

		private static void NewTag(string name, XmlElement path)
		{
			var element = Vm.Xdp.Document.CreateElement("Tag");
			element.SetAttribute("name", name);
			_ = path.AppendChild(element);
			TagsChanged();
		}
		public static void MoveTag(XmlElement name, XmlElement? path)
		{
			try
			{
				path ??= (XmlElement?)Vm.Xdp.Document.LastChild;
				_ = path!.AppendChild(name); //原位置自动被删除
				TagsChanged();
			}
			catch (ArgumentException)
			{
				App.ErrorMessageBox("禁止将标签移动到自己目录下");
			}
		}
		private static void RenameTag(string name, XmlElement path)
		{
			path.RemoveAllAttributes();
			path.SetAttribute("name", name);
			TagsChanged();
			Vm.Name = "";
			Vm.Path = "";
		}
		private static void DeleteTag(XmlElement path)
		{
			_ = path.ParentNode!.RemoveChild(path);
			TagsChanged();
			Vm.Name = "";
		}
		private static void TagsChanged()
		{
			Vm.Changed = true;
			App.RecursiveLoadTags();
		}

		#endregion
	}
}
