using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace TagsTree.Views
{
	public partial class TagsManager : Window
	{
		public TagsManager(Window owner)
		{
			Owner = owner;
			InitializeComponent();
			MouseLeftButtonDown += (_, _) => DragMove();
			var vm = Services.TagsManagerService.Load(this);
			_ = TvTags.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Mode = BindingMode.TwoWay, Source = vm.Xdp });
			DataContext = vm;

			_moveTag = vm.MoveTag;
			TbPath.LostFocus += vm.PathComplement;
			TbPath.SuggestionChosen += vm.SuggestionChosen;
			TbName.TextChanged += vm.NameChanged;
			TbPath.TextChanged += vm.PathChanged;
			TvTags.SelectedItemChanged += (_, _) => vm.TvSelectItemChanged(TvTags.SelectedItem);
		}

		private readonly Action<XmlElement, XmlElement?> _moveTag;
		private static XmlElement TvItemGetHeader(object? sender) => (XmlElement)((TreeViewItem)sender!).Header;

		#region 拖拽

		private void TbTag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => App.LastMousePos = e.GetPosition(TvTags);
		private void TbTag_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && App.MouseDisplace(4, e.GetPosition(TvTags)))
				_ = DragDrop.DoDragDrop((TreeViewItem)sender, TvItemGetHeader(sender), DragDropEffects.Move);
		}
		private void TbTag_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetData(typeof(XmlElement)) is XmlElement origin)
				if (origin != TvItemGetHeader(sender))
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
			if (origin != TvItemGetHeader(sender))
				_moveTag(origin,TvItemGetHeader(sender));
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