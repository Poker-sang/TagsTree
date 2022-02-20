using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualBasic.FileIO;
using TagsTree.Interfaces;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views;

/// <summary>
/// FilePropertiesPage.xaml 的交互逻辑
/// </summary>
[INotifyPropertyChanged]
public partial class FilePropertiesPage : Page, ITypeGetter
{
    public FilePropertiesPage() => InitializeComponent();
    public static Type TypeGetter => typeof(FilePropertiesPage);

    private static readonly FileViewModel ConstFileViewModel = new(App.IdFile[0]);

    public FileViewModel FileViewModel { get; private set; } = ConstFileViewModel;

    #region 事件处理

    protected override void OnNavigatedTo(NavigationEventArgs e) => Load((FileViewModel)e.Parameter);

    private void OpenBClick(object sender, RoutedEventArgs e) => FileViewModel.Open();
    private void OpenExplorerBClick(object sender, RoutedEventArgs e) => FileViewModel.OpenDirectory();
    private void EditTagsBClick(object sender, RoutedEventArgs e) => App.RootFrame.Navigate(typeof(FileEditTagsPage), FileViewModel);
    private async void RemoveBClick(object sender, RoutedEventArgs e)
    {
        if (!await ShowMessageDialog.Warning("是否从软件移除该文件？")) return;
        Remove(FileViewModel);
    }
    private async void RenameBClick(object sender, RoutedEventArgs e)
    {
        InputName.Load($"文件重命名 {FileViewModel.Name}", () =>
        {
            if (FileViewModel.Name == InputName.Text)
            {
                return "新文件名与原文件名一致！";
            }
            var newFullName = FileViewModel.Path + @"\" + InputName.Text;
            if (FileViewModel.IsFolder ? FileSystem.DirectoryExists(newFullName) : FileSystem.FileExists(newFullName))
            {
                var isFolder = FileViewModel.IsFolder ? "夹" : "";
                return $"新文件{isFolder}名与目录中其他文件{isFolder}同名！";
            }
            return null;
        }, FileSystemHelper.InvalidMode.Name, FileViewModel.Name);
        if (await InputName.ShowAsync()) return;
        var newFullName = FileViewModel.Path + @"\" + InputName.Text;
        FileViewModel.Rename(newFullName);
        FileViewModel.MoveOrRenameAndSave(newFullName);
        Load(FileViewModel);
    }
    private async void MoveBClick(object sender, RoutedEventArgs e)
    {
        if (await FileSystemHelper.GetStorageFolder() is not { } folder) return;
        if (FileViewModel.Path == folder.Path)
        {
            await ShowMessageDialog.Information(true, "新目录与原目录一致！");
            return;
        }
        if (folder.Path.Contains(FileViewModel.Path))
        {
            await ShowMessageDialog.Information(true, "不能将其移动到原目录下！");
            return;
        }
        var newFullName = folder.Path + @"\" + FileViewModel.Name;
        if (newFullName.Exists())
        {
            await ShowMessageDialog.Information(true, "新名称与目录下其他文件（夹）同名！");
            return;
        }
        FileViewModel.Move(newFullName);
        FileViewModel.MoveOrRenameAndSave(newFullName);
        Load(FileViewModel);
    }
    private async void DeleteBClick(object sender, RoutedEventArgs e)
    {
        if (!await ShowMessageDialog.Warning("是否删除该文件？")) return;
        FileViewModel.Delete();
        Remove(FileViewModel);
    }

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
        App.RootFrame.GoBack(new SlideNavigationTransitionInfo());
        fileViewModel.RemoveAndSave();
        TagSearchFilesPage.FileRemoved(fileViewModel);
    }

    #endregion
}