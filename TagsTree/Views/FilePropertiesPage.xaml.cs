using System;
using System.IO;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.Views.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using WinUI3Utilities;

namespace TagsTree.Views;

public partial class FilePropertiesPage : Page
{
    public FilePropertiesPage() => InitializeComponent();

    private readonly FilePropertiesPageViewModel _vm = new();

    #region 事件处理

    protected override void OnNavigatedTo(NavigationEventArgs e) => _vm.FileViewModel = e.Parameter.To<FileViewModel>();

    private void OpenClicked(object sender, RoutedEventArgs e) => _vm.FileViewModel.Open();

    private void OpenExplorerClicked(object sender, RoutedEventArgs e) => _vm.FileViewModel.OpenDirectory();

    private void EditTagsClicked(object sender, RoutedEventArgs e) => App.MainWindow.GotoPage<FileEditTagsPage>(_vm.FileViewModel);

    private async void RemoveClicked(object sender, RoutedEventArgs e)
    {
        if (!await ShowContentDialog.Warning("是否从软件移除该文件？"))
            return;
        Remove(_vm.FileViewModel);
    }

    private async void RenameClicked(object sender, RoutedEventArgs e)
    {
        InputName.Load($"文件重命名 {_vm.FileViewModel.Name}", cd =>
        {
            if (_vm.FileViewModel.Name == cd.Text)
                return "新文件名与原文件名一致！";

            var newFullName = _vm.FileViewModel.Path + @"\" + cd.Text;
            if (_vm.FileViewModel.IsFolder ? Directory.Exists(newFullName) : File.Exists(newFullName))
            {
                var isFolder = _vm.FileViewModel.IsFolder ? "夹" : "";
                return $"新文件{isFolder}名与目录中其他文件{isFolder}同名！";
            }

            return null;
        }, FileSystemHelper.InvalidMode.Name, _vm.FileViewModel.Name);
        if (await InputName.ShowAsync())
            return;
        var newFullName = _vm.FileViewModel.Path + @"\" + InputName.Text;
        _vm.FileViewModel.FileModel.Rename(newFullName);
        _vm.FileViewModel.MoveOrRenameAndSave(newFullName);
        // 相当于对FileViewModel的所有属性OnPropertyChanged
        _vm.RaiseOnPropertyChanged();
    }

    private async void MoveClicked(object sender, RoutedEventArgs e)
    {
        if (await App.MainWindow.PickSingleFolderAsync() is not { } folder)
            return;
        if (_vm.FileViewModel.Path == folder.Path)
        {
            await ShowContentDialog.Information(true, "新目录与原目录一致！");
            return;
        }

        if (folder.Path.Contains(_vm.FileViewModel.Path))
        {
            await ShowContentDialog.Information(true, "不能将其移动到原目录下！");
            return;
        }

        var newFullName = folder.Path + @"\" + _vm.FileViewModel.Name;
        if (newFullName.Exists())
        {
            await ShowContentDialog.Information(true, "新名称与目录下其他文件（夹）同名！");
            return;
        }

        _vm.FileViewModel.FileModel.Move(newFullName);
        _vm.FileViewModel.MoveOrRenameAndSave(newFullName);
        _vm.RaiseOnPropertyChanged();
    }

    private async void DeleteClicked(object sender, RoutedEventArgs e)
    {
        if (!await ShowContentDialog.Warning("是否删除该文件？"))
            return;
        _vm.FileViewModel.FileModel.Delete();
        Remove(_vm.FileViewModel);
    }

    private void CopyClicked(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(sender.To<SettingsCard>().Header.To<string>());
        Clipboard.SetContent(dataPackage);
    }

    #endregion

    #region 操作

    private static void Remove(FileViewModel fileViewModel)
    {
        App.MainWindow.Frame.GoBack(new SlideNavigationTransitionInfo());
        fileViewModel.RemoveAndSave();
        TagSearchFilesPage.FileRemoved(fileViewModel);
    }

    #endregion
}
