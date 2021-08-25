using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using TagsTreeWpf.Services;
using TagsTreeWpf.Services.ExtensionMethods;
using TagsTreeWpf.ViewModels;

namespace TagsTreeWpf.Views
{
	/// <summary>
	/// FileEditTags.xaml 的交互逻辑
	/// </summary>
	public partial class FileEditTags : Window
	{
		public FileEditTags(Window owner, FileViewModel fileViewModel)
		{
			Owner = owner;
			fileViewModel.VirtualTagsInitialize();
			DataContext = _vm = new FileEditTagsViewModel(fileViewModel);
			InitializeComponent();
			MouseLeftButtonDown += (_, _) => DragMove();
			_ = Tags.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Mode = BindingMode.TwoWay, Source = ((FileEditTagsViewModel)DataContext).Xdp });
		}

		private readonly FileEditTagsViewModel _vm;
		private void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => TbPath.Path = TagMethods.TvSelectedItemChanged((XmlElement?)e.NewValue) ?? TbPath.Path;

		private void AddBClick(object parameter, RoutedEventArgs e)
		{
			if (TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			if (_vm.FileViewModel.VirtualTags.GetTagModels().Contains(pathTagModel))
			{
				MessageBoxX.Error("已拥有该标签");
				return;
			}
			if (_vm.FileViewModel.GetRelativeVirtualTag(pathTagModel) is { } relativeTag)
			{
				MessageBoxX.Error($"已拥有下级标签「{relativeTag}」");
				return;
			}
			foreach (var tagModel in _vm.FileViewModel.VirtualTags.GetTagModels())
				if (tagModel.HasChildTag(pathTagModel))
				{
					if (MessageBoxX.Warning($"将会覆盖上级标签「{tagModel.Name}」，是否继续？"))
						_vm.FileViewModel.VirtualTags = $" {_vm.FileViewModel.VirtualTags} ".Replace($" {tagModel.Name} ", " ").Trim();
					else return;
				}
			_vm.FileViewModel.VirtualTags += (_vm.FileViewModel.VirtualTags is "" ? "" : " ") + pathTagModel.Name;
			BSave.IsEnabled = true;
		}

		private void DeleteBClick(object parameter, RoutedEventArgs e)
		{
			if (TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			_vm.FileViewModel.VirtualTags = $" {_vm.FileViewModel.VirtualTags} ".Replace($" {pathTagModel.Name} ", " ").Trim();
			BSave.IsEnabled = true;
		}

		private void SaveBClick(object parameter, RoutedEventArgs e)
		{
			for (var index = 1; index < App.Relations.Columns.Count; index++)
			{
				var column = App.Relations.Columns[index];
				App.Relations.RowAt(_vm.FileViewModel.GetFileModel)[column] = $" {_vm.FileViewModel.VirtualTags} ".Contains($" {App.Tags[Convert.ToInt32(column.ColumnName)].Name} ");
			}
			_vm.FileViewModel.TagsUpdated();
			App.SaveRelations();
			MessageBoxX.Information("已保存更改");
			BSave.IsEnabled = false;
		}
	}
}
