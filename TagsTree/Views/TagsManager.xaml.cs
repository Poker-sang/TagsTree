using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using TagsTree.ViewModels;

namespace TagsTree.Views
{
	public partial class TagsManager : Window
	{
		public TagsManager(Window owner)
		{
			Owner = owner;
			InitializeComponent();
			Services.TagsManagerService.Load(this);
			MouseLeftButtonDown += (_, _) => DragMove();
			_ = TvTags.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Mode = BindingMode.TwoWay, Source = ((TagsManagerViewModel)DataContext).Xdp });

			_moveTag = ((TagsManagerViewModel)DataContext).MoveTag;
			TbPath.LostFocus += ((TagsManagerViewModel)DataContext).PathComplement;
			TbPath.SuggestionChosen += ((TagsManagerViewModel)DataContext).SuggestionChosen;
			TbName.TextChanged += ((TagsManagerViewModel)DataContext).NameChanged;
			TbPath.TextChanged += ((TagsManagerViewModel)DataContext).PathChanged;
			TvTags.SelectedItemChanged += (_, _) => ((TagsManagerViewModel)DataContext).TvSelectItemChanged(TvTags.SelectedItem);
		}

		private readonly Action<XmlElement, XmlElement?> _moveTag;
		private static XmlElement TvItemGetHeader(object? sender) => (XmlElement)((TreeViewItem)sender!).Header;

		#region 拖拽

		private void TbTag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => App.Mouse.LastMousePos = e.GetPosition(TvTags);
		private void TbTag_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && App.Mouse.MouseDisplace(4, e.GetPosition(TvTags)))
				_ = DragDrop.DoDragDrop((TreeViewItem)sender, TvItemGetHeader(sender), DragDropEffects.Move);
		}
		private void TbTag_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetData(typeof(XmlElement)) is XmlElement origin)
				if (origin != TvItemGetHeader(sender))
					((TreeViewItem)sender).Foreground = new SolidColorBrush(Colors.LightGray);
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
			if (origin != TvItemGetHeader(sender))
				_moveTag(origin, TvItemGetHeader(sender));
		}
		private void TvTags_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetData(typeof(XmlElement)) is not XmlElement origin)
				return;
			e.Effects = DragDropEffects.None;
			e.Handled = true;
			_moveTag(origin, null);
		}

		#endregion
	}
}