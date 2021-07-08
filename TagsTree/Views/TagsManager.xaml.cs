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
using Service = TagsTree.Services.TagsManagerServices;
using TagsTree.ViewModels;
using static TagsTree.Properties.Settings;

namespace TagsTree
{
	public partial class TagsManager : Window
	{
		public TagsManager()
		{
			InitializeComponent();
			MouseLeftButtonDown += (_, _) => DragMove();
			DataContext = Service.Load(TvTags);
		}

		#region 被动反应

		private void TbName_OnLostFocus(object sender, RoutedEventArgs e) => Service.NameComplement();
		private void TbPath_OnLostFocus(object sender, RoutedEventArgs e) => Service.PathComplement();
		private void TvTags_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => Service.TreeViewComplement(TvTags);

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
				Service.MoveTag(origin,TagsTreeStatic.TvItemGetHeader(sender));
		}
		private void TvTags_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetData(typeof(XmlElement)) is not XmlElement origin)
				return;
			e.Effects = DragDropEffects.None;
			e.Handled = true;
			Service.MoveTag(origin, TagsTreeStatic.XdpRoot!);
		}

		#endregion

		#region 按钮
		

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

			Service.NewTag(dialog.Message, TagsTreeStatic.TvItemGetHeader(e.Parameter)!);
		}
		private void NewX_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			Service.NewTag(dialog.Message, TagsTreeStatic.XdpRoot!);
		}
		private void Cut_Execute(object sender, ExecutedRoutedEventArgs e) => ClipBoard = TagsTreeStatic.TvItemGetHeader(e.Parameter);
		private void Paste_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			Service.MoveTag(ClipBoard!, TagsTreeStatic.TvItemGetHeader(e.Parameter));
			ClipBoard = null;
		}
		private void PasteX_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			Service.MoveTag(ClipBoard!, TagsTreeStatic.XdpRoot!);
			ClipBoard = null;
		}
		private void Rename_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			Service.RenameTag(dialog.Message, TagsTreeStatic.TvItemGetHeader(e.Parameter)!);
		}
		private void Delete_Execute(object sender, ExecutedRoutedEventArgs e) => Service.DeleteTag(TagsTreeStatic.TvItemGetHeader(e.Parameter));

		#endregion
	}
}
