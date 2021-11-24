using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views
{
    /// <summary>
    /// TagEditFilesPage.xaml 的交互逻辑
    /// </summary>
    public partial class TagEditFilesPage : Page
    {
        public TagEditFilesPage()
        {
            _vm = new TagEditFilesViewModel();
            InitializeComponent();
            Storyboard0.Begin();
        }

        private readonly TagEditFilesViewModel _vm;

        private void Storyboard1_OnCompleted(object? sender, object e)
        {
            Panel.Children.Remove(Tags);
            BConfirm.Content = "保存";
            Storyboard2.Begin();
        }

        private void Storyboard2_OnCompleted(object? sender, object e)
        {
            TbInput.IsHitTestVisible = true;
            DgResult.IsHitTestVisible = true;
        }

        #region 事件处理

        private void Tags_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => TbPath.Path = ((TagViewModel?)e.InvokedItem)?.FullName ?? TbPath.Path;

        private void ResultChanged(IEnumerable<FileModel> newResult) => _vm.FileViewModels = newResult.Select(fileModel => new FileViewModel(fileModel, TbPath.Path.GetTagViewModel()!)).ToObservableCollection();

        private void Selected(object sender, SelectionChangedEventArgs e)
        {
            if ((FileViewModel)((DataGrid)sender).SelectedItem is null) return;
            ((FileViewModel)((DataGrid)sender).SelectedItem).SelectedFlip();
            ((DataGrid)sender).SelectedIndex = -1;
        }

        private static bool _mode;

        private async void ConfirmBClick(object sender, RoutedEventArgs e)
        {
            if (!_mode)
            {
                if (TbPath.Path.GetTagViewModel() is not { } pathTagModel)
                {
                    await ShowMessageDialog.Information(true, "「标签路径」不存在！");
                    return;
                }
                if (pathTagModel == App.Tags.TagsDictionaryRoot)
                {
                    await ShowMessageDialog.Information(true, "「标签路径」不能为空！");
                    return;
                }
                Storyboard1.Begin();
                _vm.FileViewModels = App.Relations.GetFileModels().Select(fileModel => new FileViewModel(fileModel, pathTagModel)).ToObservableCollection();
                TbPath.IsEnabled = false;
                _mode = true;
            }
            else
            {
                if (TbPath.Path.GetTagViewModel() is not { } pathTagModel)
                {
                    await ShowMessageDialog.Information(true, "「标签路径」不存在！"); //理论上不会到达此代码
                    return;
                }
                foreach (var fileViewModel in _vm.FileViewModels)
                    if (fileViewModel.Selected != fileViewModel.SelectedOriginal)
                    {
                        switch (fileViewModel.SelectedOriginal)
                        {
                            case true: App.Relations[fileViewModel, pathTagModel] = false; break;
                            case false: App.Relations[fileViewModel, pathTagModel] = true; break;
                            case null: //如果原本是null，则删除fileViewModel拥有的相应子标签
                                foreach (var tag in fileViewModel.GetRelativeTags(pathTagModel))
                                    App.Relations[fileViewModel, tag] = false;
                                break;
                        }
                        fileViewModel.TagsUpdated();
                    }
                App.SaveRelations();
                await ShowMessageDialog.Information(false, "已保存更改");
            }
        }

        #endregion
    }
}
