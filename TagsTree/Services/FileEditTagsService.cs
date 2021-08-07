using System;
using System.Linq;
using System.Windows;
using System.Xml;
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

		public static void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => Win.TbPath.AutoSuggestBox.Text = App.TvSelectedItemChanged((XmlElement?)e.NewValue) ?? Win.TbPath.AutoSuggestBox.Text;

		public static void AddBClick(object? parameter)
		{
			if (App.IsTagNotExists(Win.TbPath.AutoSuggestBox.Text)) return;
			//if(Vm.FileViewModel.t)hasTags 加标签时查看有无上级或下级标签
			Vm.FileViewModel.VirtualTags += " " + Win.TbPath.AutoSuggestBox.Text.Split('\\', StringSplitOptions.RemoveEmptyEntries).Last();
			Vm.CanSave = true;
		}
		public static void DeleteBClick(object? parameter)
		{
			if (App.IsTagNotExists(Win.TbPath.AutoSuggestBox.Text)) return;
			var tag = Win.TbPath.AutoSuggestBox.Text.Split('\\', StringSplitOptions.RemoveEmptyEntries).Last();
			Vm.FileViewModel.VirtualTags = $" {Vm.FileViewModel.VirtualTags} ".Replace($" {tag} ", " ")[1..^1];
			Vm.CanSave = true;
		}
		public static void SaveBClick(object? parameter)
		{
			throw new System.NotImplementedException();
			App.MessageBoxX.Information("已保存更改");
			Vm.CanSave = false;
		}
	}
}
