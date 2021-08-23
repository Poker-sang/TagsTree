using JetBrains.Annotations;
using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf.Controls;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using TagsTreeWpf.Delegates;
using TagsTreeWpf.Services;
using TagsTreeWpf.Services.ExtensionMethods;
using TagsTreeWpf.ViewModels;
using static TagsTreeWpf.Properties.Settings;

namespace TagsTreeWpf.Views.Controls
{
	/// <summary>
	/// FileProperties.xaml 的交互逻辑
	/// </summary>
	public partial class FileProperties : ContentDialog, INotifyPropertyChanged
	{
		public event FileRemovedEventHandler FileRemoved;
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


		public FileProperties() => InitializeComponent();

		public void Load(Main win, FileViewModel fileViewModel)
		{
			_win = win;
			FileViewModel = fileViewModel;
			OnPropertyChanged(nameof(FileViewModel));
			BOpen.IsEnabled = BRename.IsEnabled = BMove.IsEnabled = BDelete.IsEnabled = FileViewModel.Exists;
		}

		private Main _win;
		public FileViewModel FileViewModel { get; private set; }

		#region 事件

		private void OpenBClick(object sender, RoutedEventArgs e) => FileViewModel.FullName.Open();
		private void OpenExplorerBClick(object sender, RoutedEventArgs e) => FileViewModel.Path.Open();
		private void EditTagsBClick(object sender, RoutedEventArgs e) => new FileEditTags(_win, FileViewModel).ShowDialog();
		private void RemoveBClick(object sender, RoutedEventArgs e)
		{
			if (!MessageBoxX.Warning("是否从软件移除该文件？")) return;
			Remove(FileViewModel);
		}
		private void RenameBClick(object sender, RoutedEventArgs e)
		{
			var dialog = new InputName(_win, FileX.InvalidMode.Name, FileViewModel.Name);
			if (dialog.ShowDialog() == false) return;
			if (FileViewModel.Name == dialog.Message)
			{
				MessageBoxX.Error("新文件名与原文件名一致！");
				return;
			}
			var newFullName = FileViewModel.Path + @"\" + dialog.Message;
			if (FileViewModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName))
			{
				MessageBoxX.Error("新文件名与文件夹中其他文件同名！");
				return;
			}
			new FileInfo(FileViewModel.FullName).MoveTo(newFullName);
			FileViewModel.Reload(newFullName);
			Load(_win, FileViewModel);
		}
		private void MoveBClick(object sender, RoutedEventArgs e)
		{
			var dialog = new CommonOpenFileDialog
			{
				Multiselect = false,
				EnsurePathExists = true,
				IsFolderPicker = true,
				InitialDirectory = Default.LibraryPath,
				Title = "选择你要将文件移动到的位置"
			};
			if (dialog.ShowDialog(_win) != CommonFileDialogResult.Ok) return; //仅用于主窗口所以直接调用MainService
			if (FileViewModel.Path == dialog.FileName)
			{
				MessageBoxX.Error("新目录与原目录一致！");
				return;
			}
			if (dialog.FileName.Contains(FileViewModel.Path))
			{
				MessageBoxX.Error("不能将其移动到原目录下！");
				return;
			}
			var newFullName = dialog.FileName + @"\" + FileViewModel.Name;
			if (FileViewModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName))
			{
				MessageBoxX.Error("新文件名与文件夹中其他文件同名！");
				return;
			}
			new FileInfo(FileViewModel.FullName).MoveTo(newFullName);
			FileViewModel.Reload(newFullName);
			Load(_win, FileViewModel);
		}
		private void DeleteBClick(object sender, RoutedEventArgs e)
		{
			if (!MessageBoxX.Warning("是否删除该文件？")) return;
			File.Delete(FileViewModel.FullName);
			Remove(FileViewModel);
		}

		#endregion

		#region 操作

		private void Remove(FileViewModel fileViewModel)
		{
			Hide();
			if (App.TryRemoveFileModel(fileViewModel))
				FileRemoved.Invoke(this, new FileRemovedEventArgs(fileViewModel));
		}

		#endregion

	}
}
