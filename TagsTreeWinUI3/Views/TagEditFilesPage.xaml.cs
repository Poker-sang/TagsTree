using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using TagsTree.Models;
using TagsTree.Services;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;

namespace TagsTree.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TagEditFilesPage : Page
    {
        public TagEditFilesPage()
        {
            _vm = new TagEditFilesViewModel();
            InitializeComponent();
        }

        private readonly TagEditFilesViewModel _vm;

        #region 事件处理

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _vm.TagViewModel = (TagViewModel)e.Parameter;
            _vm.FileViewModels = App.Relations.GetFileModels().Select(fileModel => new FileViewModel(fileModel, _vm.TagViewModel)).ToObservableCollection();
        }

        private void ResultChanged(IEnumerable<FileModel> newResult) => _vm.FileViewModels = newResult.Select(fileModel => new FileViewModel(fileModel, _vm.TagViewModel.Path.GetTagViewModel()!)).ToObservableCollection();

        private void Selected(object sender, SelectionChangedEventArgs e)
        {
            if ((FileViewModel)((DataGrid)sender).SelectedItem is null) return;
            ((FileViewModel)((DataGrid)sender).SelectedItem).SelectedFlip();
            ((DataGrid)sender).SelectedIndex = -1;
        }
        private async void SaveBClick(object sender, RoutedEventArgs e)
        {
            if (_vm.TagViewModel.FullName.GetTagViewModel() is not { } pathTagModel)
            {
                await ShowMessageDialog.Information(true, "「标签路径」不存在！"); //理论上不会到达此代码
                return;
            }
            foreach (var fileViewModel in _vm.FileViewModels)
                if (fileViewModel.Selected != fileViewModel.SelectedOriginal)
                {
                    switch (fileViewModel.SelectedOriginal)
                    {
                        case true: App.Relations[pathTagModel, fileViewModel] = false; break;
                        //如果原本有上级标签，则覆盖相应上级标签
                        case false:
                            App.Relations[pathTagModel, fileViewModel] = true;
                            foreach (var tagViewModel in fileViewModel.Tags.GetTagViewModels())
                                if (tagViewModel.HasChildTag(pathTagModel))
                                {
                                    App.Relations[tagViewModel, fileViewModel] = false;
                                    break;
                                }
                            break;
                        //如果原本是null，则删除fileViewModel拥有的相应子标签
                        case null: 
                            foreach (var tag in fileViewModel.GetRelativeTags(pathTagModel))
                                App.Relations[tag, fileViewModel] = false;
                            break;
                    }
                    fileViewModel.TagsUpdated();
                }
            App.SaveRelations();
            await ShowMessageDialog.Information(false, "已保存更改");
            GoBack();
        }
        private void BackBClick(object sender, RoutedEventArgs e) => GoBack();

        #endregion

        #region 操作

        private static void GoBack() => App.RootFrame.GoBack(new SlideNavigationTransitionInfo());

        #endregion
    }
}
