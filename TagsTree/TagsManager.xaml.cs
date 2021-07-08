using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using ModernWpf.Controls;
using static TagsTree.Properties.Settings;

namespace TagsTree
{
	public partial class TagsManager : Window
	{
		public TagsManager()
		{
			InitializeComponent();
			MouseLeftButtonDown += (_, _) => DragMove();
			TagsTreeStatic.XdpLoad(TvTags);
		}

		#region 被动反应

		private void TbName_OnLostFocus(object sender, RoutedEventArgs e) => ((AutoSuggestBox)sender).Text = Regex.Replace(((AutoSuggestBox)sender).Text, @"\s", "");
		private void TbPath_OnLostFocus(object sender, RoutedEventArgs e)
		{
			((AutoSuggestBox)sender).Text = Regex.Replace(((AutoSuggestBox)sender).Text, @"\s", "");
			var path = TagsTreeStatic.TagPathComplete(((AutoSuggestBox)sender).Text);
			if (path is null)
				TagsTreeStatic.ErrorMessageBox("标签路径不存在！请填写正确的单个标签或完整的路径！\n" + @"（不包含/:*?\""<>|,和空白字符）");
			else ((AutoSuggestBox)sender).Text = path;
		}
		private void TbName_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				TbPath.Focus();
		}
		private void TvTags_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => TbPath.Text = TagsTreeStatic.TagsTree_OnSelectedItemChanged(TvTags) ?? TbPath.Text;

		#endregion

		#region 拖拽

		private void TbTag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			TagsTreeStatic.LastMousePos = e.GetPosition(TvTags);
		}
		private void TbTag_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && TagsTreeStatic.MouseDisplace(4, e.GetPosition(TvTags)))
				_ = DragDrop.DoDragDrop((TreeViewItem)sender, TagsTreeStatic.TvItemGetHeader(sender), DragDropEffects.Move);
		}
		private void TbTag_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetData(typeof(XmlElement)) is XmlElement origin)
				if (origin != TagsTreeStatic.TvItemGetHeader(sender))
					((TreeViewItem)sender).Foreground = new SolidColorBrush(Colors.Orange);
			e.Handled = true;
		}
		private void TbTag_DragLeave(object sender, DragEventArgs dragEventArgs)
		{
			((TreeViewItem)sender).Foreground = new SolidColorBrush(Colors.Black);
		}
		private void TbTag_Drop(object sender, DragEventArgs e)
		{
			((TreeViewItem)sender).Foreground = new SolidColorBrush(Colors.Black);
			if (e.Data.GetData(typeof(XmlElement)) is not XmlElement origin)
				return;
			e.Effects = DragDropEffects.None;
			e.Handled = true;
			if (origin != TagsTreeStatic.TvItemGetHeader(sender))
				MoveTag(origin,TagsTreeStatic.TvItemGetHeader(sender));
		}
		private void TvTags_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetData(typeof(XmlElement)) is not XmlElement origin)
				return;
			e.Effects = DragDropEffects.None;
			e.Handled = true;
			MoveTag(origin, TagsTreeStatic.XdpRoot!);
		}

		#endregion

		#region 按钮

		private void BNew_Click(object sender, RoutedEventArgs e)
		{
			if (!TagsTreeStatic.NewTagCheck(TbName.Text))
				return;

			NewTag(TbName.Text, TagsTreeStatic.RecursiveSearchXmlElement(TbPath.Text)!);
			TbName.Text = "";
		}
		private void BMove_Click(object sender, RoutedEventArgs e)
		{
			var element = TagsTreeStatic.RecursiveSearchXmlElement(TbName.Text);
			if (element is null)
				TagsTreeStatic.ErrorMessageBox("标签名称不存在！请填写正确的单个标签或完整的路径！");
			else
			{
				MoveTag(element, TagsTreeStatic.RecursiveSearchXmlElement(TbPath.Text)!);
				TbName.Text = "";
			}
		}
		private void BRename_Click(object sender, RoutedEventArgs e)
		{
			if (!TagsTreeStatic.NewTagCheck(TbName.Text))
				return;
			RenameTag(TbName.Text, TagsTreeStatic.RecursiveSearchXmlElement(TbPath.Text)!);
			TbName.Text = "";
			TbPath.Text = "";
		}
		private void BDelete_Click(object sender, RoutedEventArgs e)
		{
			DeleteTag(TagsTreeStatic.RecursiveSearchXmlElement(TbPath.Text)!);
			TbName.Text = "";
		}

		#endregion

		#region 命令

		private void True_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
		private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ClipBoard is not null;

		private XmlElement? ClipBoard { get; set; }

		private void New_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			NewTag(dialog.Message, TagsTreeStatic.TvItemGetHeader(e.Parameter)!);
		}
		private void NewX_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			NewTag(dialog.Message, TagsTreeStatic.XdpRoot!);
		}
		private void Cut_Execute(object sender, ExecutedRoutedEventArgs e) => ClipBoard = TagsTreeStatic.TvItemGetHeader(e.Parameter);
		private void Paste_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			MoveTag(ClipBoard!, TagsTreeStatic.TvItemGetHeader(e.Parameter));
			ClipBoard = null;
		}
		private void PasteX_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			MoveTag(ClipBoard!, TagsTreeStatic.XdpRoot!);
			ClipBoard = null;
		}
		private void Rename_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			RenameTag(dialog.Message, TagsTreeStatic.TvItemGetHeader(e.Parameter)!);
		}
		private void Delete_Execute(object sender, ExecutedRoutedEventArgs e) => DeleteTag(TagsTreeStatic.TvItemGetHeader(e.Parameter));

		#endregion

		#region 执行

		private bool TagsTreeChanged { get; set; } = false;
		private void NewTag(string name, XmlElement path)
		{
			var element = TagsTreeStatic.XdpDocument.CreateElement("Tag");
			element.SetAttribute("name", name);
			_ = path.AppendChild(element);
			TagsTreeChanged = true;
		}
		private void MoveTag(XmlElement name, XmlElement path)
		{
			try
			{
				_ =path.AppendChild(name); //原位置自动被删除
				TagsTreeChanged = true;
			}
			catch (ArgumentException)
			{
				TagsTreeStatic.ErrorMessageBox("禁止将标签移动到自己目录下");
			}
		}
		private void RenameTag(string name, XmlElement path)
		{
			path.RemoveAllAttributes();
			path.SetAttribute("name", name);
			TagsTreeChanged = true;
		}
		private void DeleteTag(XmlElement path)
		{
			_ = path.ParentNode!.RemoveChild(path);
			TagsTreeChanged = true;
		}

		#endregion

		private void BSave_Click(object sender, RoutedEventArgs e)
		{
			TagsTreeStatic.XdpSave();
			TagsTreeChanged = false;
		}
		private void TagsManager_OnClosing(object sender, CancelEventArgs e)
		{
			if (!TagsTreeChanged)
				return;
			var result = MessageBox.Show("是否保存配置更改", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
			switch (result)
			{
				case MessageBoxResult.Yes:
					TagsTreeStatic.XdpSave();
					break;
				case MessageBoxResult.No:
					Close();
					break;
				default:
					e.Cancel = true;
					break;
			}
		}
	}
}
