using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using TagsTreeWinUI3.Delegates;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Views.Controls
{
	/// <summary>
	/// FileProperties.xaml 的交互逻辑
	/// </summary>
	public partial class FileProperties : ContentDialog, INotifyPropertyChanged
	{
		public event FileRemovedEventHandler FileRemoved = null!;
		public event FileEditTagsRaisedEventHandler FileEditTagsRaised = null!;
		public event PropertyChangedEventHandler? PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public FileProperties() => InitializeComponent();

		public void Load(FileViewModel fileViewModel)
		{
			FileViewModel = fileViewModel;
			OnPropertyChanged(nameof(FileViewModel));
			BOpen.IsEnabled = BRename.IsEnabled = BMove.IsEnabled = BDelete.IsEnabled = FileViewModel.Exists;
		}

		public FileViewModel FileViewModel { get; private set; } = null!;

		#region 事件

		private void OpenBClick(object sender, RoutedEventArgs e) => FileViewModel.FullName.Open();
		private void OpenExplorerBClick(object sender, RoutedEventArgs e) => FileViewModel.Path.Open();
		private void EditTagsBClick(ContentDialog sender, ContentDialogButtonClickEventArgs e) => FileEditTagsRaised.Invoke(FileViewModel);
		private async void RemoveBClick(object sender, RoutedEventArgs e)
		{
			if (await MessageDialogX.Warning("是否从软件移除该文件？")) return;
			Remove(FileViewModel);
		}
		private async void RenameBClick(object sender, RoutedEventArgs e)
		{
			//FileProperties.
			//InputName.Load(FileX.InvalidMode.Name);
			//await InputName.ShowAsync();
			//if (!InputName.Canceled && NewTagCheck(InputName.AsBox.Text))
			//	NewTag(InputName.AsBox.Text, MItemGetHeader(sender));
			//var dialog = new InputName(FileX.InvalidMode.Name, FileViewModel.Name);
			//if (dialog.ShowDialog() == false) return;
			//if (FileViewModel.Name == dialog.Message)
			//{
			//	MessageDialogX.Information(true, "新文件名与原文件名一致！");
			//	return;
			//}
			//var newFullName = FileViewModel.Path + @"\" + dialog.Message;
			//if (FileViewModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName))
			//{
			//	MessageDialogX.Information(true, "新文件名与文件夹中其他文件同名！");
			//	return;
			//}
			//new FileInfo(FileViewModel.FullName).MoveTo(newFullName);
			//FileViewModel.Reload(newFullName);
			//Load(FileViewModel);
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
			FileViewModel.Reload(newFullName);
			Load(FileViewModel);
		}
		private async void DeleteBClick(object sender, RoutedEventArgs e)
		{
			if (await MessageDialogX.Warning("是否删除该文件？")) return;
			File.Delete(FileViewModel.FullName);
			Remove(FileViewModel);
		}

		#endregion

		#region 操作

		private void Remove(FileViewModel fileViewModel)
		{
			Hide();
			if (App.TryRemoveFileModel(fileViewModel))
				FileRemoved.Invoke(fileViewModel);
		}

		#endregion

	}
}
