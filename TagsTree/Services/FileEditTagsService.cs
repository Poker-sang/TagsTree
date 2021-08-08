using System.Linq;
using System.Windows;
using System.Xml;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
using TagsTree.Views;

namespace TagsTree.Services
{
	public static class FileEditTagsService
	{
		private static FileEditTagsViewModel Vm;
		private static FileEditTags Win;

		public static void Load(FileEditTags window)
		{
			Win = window;
			Vm = (FileEditTagsViewModel)window.DataContext;
		}

		public static void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => Win.TbPath.AutoSuggestBox.Text = App.TagMethods.TvSelectedItemChanged((XmlElement?)e.NewValue) ?? Win.TbPath.AutoSuggestBox.Text;

		public static void AddBClick(object? parameter)
		{
			if (Win.TbPath.AutoSuggestBox.Text.GetTagModel() is not { } pathTagModel)
			{
				App.MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			if (Vm.FileViewModel.VirtualTags.GetTagModels().Contains(pathTagModel))
			{
				App.MessageBoxX.Error("已拥有该标签");
				return;
			}
			if (Vm.FileViewModel.GetRelativeVirtualTag(pathTagModel) is { } relativeTag)
			{
				App.MessageBoxX.Error($"已拥有下级标签「{relativeTag}」");
				return;
			}
			foreach (var tagModel in Vm.FileViewModel.VirtualTags.GetTagModels())
				if (tagModel.HasChildTag(pathTagModel))
				{
					if (App.MessageBoxX.Warning($"将会覆盖上级标签「{tagModel.Name}」，是否继续？"))
						Vm.FileViewModel.VirtualTags = $" {Vm.FileViewModel.VirtualTags} ".Replace($" {tagModel.Name} ", " ").Trim();
					else return;
				}
			Vm.FileViewModel.VirtualTags += (Vm.FileViewModel.VirtualTags is "" ? "" : " ") + pathTagModel.Name;
			Vm.CanSave = true;
		}
		public static void DeleteBClick(object? parameter)
		{
			if (Win.TbPath.AutoSuggestBox.Text.GetTagModel() is not { } pathTagModel)
			{
				App.MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			Vm.FileViewModel.VirtualTags = $" {Vm.FileViewModel.VirtualTags} ".Replace($" {pathTagModel.Name} ", " ").Trim();
			Vm.CanSave = true;
		}
		public static void SaveBClick(object? parameter)
		{
			for (var index = 1; index < App.Relations.Columns.Count; index++)
			{
				var column = App.Relations.Columns[index];
				App.Relations.RowAt(Vm.FileViewModel.GetFileModel)[column] = $" {Vm.FileViewModel.VirtualTags} ".Contains($" {column.ColumnName} ");
			}
			Vm.FileViewModel.TagsUpdated();
			App.SaveRelations();
			App.MessageBoxX.Information("已保存更改");
			Vm.CanSave = false;
		}
	}
}
