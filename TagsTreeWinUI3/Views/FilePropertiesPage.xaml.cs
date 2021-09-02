using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Views
{
	/// <summary>
	/// FilePropertiesPage.xaml 的交互逻辑
	/// </summary>
	public partial class FilePropertiesPage : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public FilePropertiesPage() => InitializeComponent();

		public FileViewModel FileViewModel { get; private set; } = null!;

		#region 事件处理

		protected override void OnNavigatedTo(NavigationEventArgs e) => Load((FileViewModel)e.Parameter);

		private void OpenBClick(object sender, RoutedEventArgs e) => FileViewModel.FullName.Open();
		private void OpenExplorerBClick(object sender, RoutedEventArgs e) => FileViewModel.Path.Open();
		private void EditTagsBClick(object sender, RoutedEventArgs e) => App.RootFrame.Navigate(typeof(FileEditTagsPage), FileViewModel);
		private async void RemoveBClick(object sender, RoutedEventArgs e)
		{
			if (await MessageDialogX.Warning("是否从软件移除该文件？")) return;
			Remove(FileViewModel);
		}
		private async void RenameBClick(object sender, RoutedEventArgs e)
		{
			InputName.Load(FileX.InvalidMode.Name, FileViewModel.Name);
			await InputName.ShowAsync();
			if (InputName.Canceled) return;
			if (FileViewModel.Name == InputName.AsBox.Text)
			{
				MessageDialogX.Information(true, "新文件名与原文件名一致！");
				return;
			}
			var newFullName = FileViewModel.Path + @"\" + InputName.AsBox.Text;
			if (FileViewModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName))
			{
				var isFolder = FileViewModel.IsFolder ? "夹" : "";
				MessageDialogX.Information(true, $"新文件{ isFolder }名与目录中其他文件{ isFolder }同名！");
				return;
			}
			new FileInfo(FileViewModel.FullName).MoveTo(newFullName);
			FileViewModel.MoveOrRenameAndSave(newFullName);
			Load(FileViewModel);
		}
		private async void MoveBClick(object sender, RoutedEventArgs e)
		{
			if (await FileX.GetStorageFolder() is not { } folder) return;
			if (FileViewModel.Path == folder.Path)
			{
				MessageDialogX.Information(true, "新目录与原目录一致！");
				return;
			}
			if (folder.Path.Contains(FileViewModel.Path))
			{
				MessageDialogX.Information(true, "不能将其移动到原目录下！");
				return;
			}
			var newFullName = folder.Path + @"\" + FileViewModel.Name;
			if (FileViewModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName))
			{
				MessageDialogX.Information(true, "新文件名与文件夹中其他文件同名！");
				return;
			}
			new FileInfo(FileViewModel.FullName).MoveTo(newFullName);
			FileViewModel.MoveOrRenameAndSave(newFullName);
			Load(FileViewModel);
		}
		private async void DeleteBClick(object sender, RoutedEventArgs e)
		{
			if (await MessageDialogX.Warning("是否删除该文件？")) return;
			File.Delete(FileViewModel.FullName);
			Remove(FileViewModel);
		}
		private void BackBClick(object sender, RoutedEventArgs e) => GoBack();

		#endregion

		#region 操作

		private void Load(FileViewModel fileViewModel)
		{
			FileViewModel = fileViewModel;
			OnPropertyChanged(nameof(FileViewModel));
			BOpen.IsEnabled = BRename.IsEnabled = BMove.IsEnabled = BDelete.IsEnabled = FileViewModel.Exists;
		}

		private static void Remove(FileViewModel fileViewModel)
		{
			GoBack();
			fileViewModel.RemoveAndSave();
			IndexPage.FileRemoved(fileViewModel);
		}

		private static void GoBack() => App.RootFrame.GoBack(new SlideNavigationTransitionInfo());

		#endregion
	}
}
