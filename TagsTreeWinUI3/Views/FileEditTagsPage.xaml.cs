using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Views
{
    /// <summary>
    /// FileEditTagsPage.xaml 的交互逻辑
    /// </summary>
    public partial class FileEditTagsPage : Page
    {
        public FileEditTagsPage()
        {
            _vm = new FileEditTagsViewModel();
            InitializeComponent();
        }

        private readonly FileEditTagsViewModel _vm;

        #region 事件处理

        protected override void OnNavigatedTo(NavigationEventArgs e) => _vm.Load((FileViewModel)e.Parameter);
        private void BackBClick(object sender, RoutedEventArgs e) => App.RootFrame.GoBack(new SlideNavigationTransitionInfo());

        private async void AddTag(object sender, DoubleTappedRoutedEventArgs e)
        {
            var newTag = (TagViewModel)((TreeViewItem)sender).Tag;
            foreach (var tagExisted in _vm.VirtualTags)
                if (tagExisted.Name == newTag.Name)
                {
                    await ShowMessageDialog.Information(true, "已拥有该标签");
                    return;
                }
                else if (newTag.HasChildTag(tagExisted))
                {
                    await ShowMessageDialog.Information(true, $"已拥有下级标签「{tagExisted.Name}」或更多");
                    return;
                }
                else if (tagExisted.HasChildTag(newTag))
                    if (await ShowMessageDialog.Warning($"将会覆盖上级标签「{tagExisted.Name}」，是否继续？"))
                    {
                        _ = _vm.VirtualTags.Remove(tagExisted);
                        break;
                    }
                    else return;
            _vm.VirtualTags.Add(newTag);
            BSave.IsEnabled = true;
        }

        private void DeleteTag(object sender, RoutedEventArgs e)
        {
            _ = _vm.VirtualTags.Remove((TagViewModel)((ListViewItem)sender).Tag);
            BSave.IsEnabled = true;
        }

        private async void SaveBClick(object sender, RoutedEventArgs e)
        {
            foreach (var tag in App.Tags.TagsDictionaryValues)
                App.Relations[_vm.FileViewModel.GetFileModel(), tag] = _vm.VirtualTags.Contains(tag);
            _vm.FileViewModel.TagsUpdated();
            App.SaveRelations();
            BSave.IsEnabled = false;
            await ShowMessageDialog.Information(false, "已保存更改");
        }
        #endregion
    }
}
