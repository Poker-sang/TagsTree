using System.IO;
using CommunityToolkit.Labs.WinUI;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using WinUI3Utilities;

namespace TagsTree.Views;

[INotifyPropertyChanged]
public partial class FilePropertiesPage : Page
{
    public FilePropertiesPage() => InitializeComponent();

    private static readonly FileViewModel _constFileViewModel = new("");

    public FileViewModel FileViewModel { get; private set; } = _constFileViewModel;

    #region 事件处理

    protected override void OnNavigatedTo(NavigationEventArgs e) => Load((FileViewModel)e.Parameter);

    private void OpenClick(object sender, RoutedEventArgs e) => FileViewModel.Open();
    private void OpenExplorerClick(object sender, RoutedEventArgs e) => FileViewModel.OpenDirectory();
    private void EditTagsClick(object sender, RoutedEventArgs e) => App.GotoPage<FileEditTagsPage>(FileViewModel);
    private async void RemoveClick(object sender, RoutedEventArgs e)
    {
        if (!await ShowMessageDialog.Warning("是否从软件移除该文件？"))
            return;
        Remove(FileViewModel);
    }
    private async void RenameClick(object sender, RoutedEventArgs e)
    {
        InputName.Load($"文件重命名 {FileViewModel.Name}", cd =>
        {
            if (FileViewModel.Name == cd.Text)
            {
                return "新文件名与原文件名一致！";
            }

            var newFullName = FileViewModel.Path + @"\" + cd.Text;
            if (FileViewModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName))
            {
                var isFolder = FileViewModel.IsFolder ? "夹" : "";
                return $"新文件{isFolder}名与目录中其他文件{isFolder}同名！";
            }

            return null;
        }, FileSystemHelper.InvalidMode.Name, FileViewModel.Name);
        if (await InputName.ShowAsync())
            return;
        var newFullName = FileViewModel.Path + @"\" + InputName.Text;
        FileViewModel.FileModel.Rename(newFullName);
        FileViewModel.MoveOrRenameAndSave(newFullName);
        // 相当于对FileViewModel的所有属性OnPropertyChanged
        OnPropertyChanged(nameof(FileViewModel));
    }
    private async void MoveClick(object sender, RoutedEventArgs e)
    {
        if (await PickerHelper.PickFolderAsync() is not { } folder)
            return;
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

        FileViewModel.FileModel.Move(newFullName);
        FileViewModel.MoveOrRenameAndSave(newFullName);
        OnPropertyChanged(nameof(FileViewModel));
    }
    private async void DeleteClick(object sender, RoutedEventArgs e)
    {
        if (!await ShowMessageDialog.Warning("是否删除该文件？"))
            return;
        FileViewModel.FileModel.Delete();
        Remove(FileViewModel);
    }
    private void CopyClick(object sender, TappedRoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText((string)((SettingsCard)sender).Header);
        Clipboard.SetContent(dataPackage);
    }

    #endregion

    #region 操作

    private void Load(FileViewModel fileViewModel)
    {
        FileViewModel = fileViewModel;
        OnPropertyChanged(nameof(FileViewModel));
    }

    private static void Remove(FileViewModel fileViewModel)
    {
        App.RootFrame.GoBack(new SlideNavigationTransitionInfo());
        fileViewModel.RemoveAndSave();
        TagSearchFilesPage.FileRemoved(fileViewModel);
    }

    #endregion
}
