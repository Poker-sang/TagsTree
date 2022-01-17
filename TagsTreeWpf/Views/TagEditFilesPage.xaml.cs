using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TagsTreeWpf.Delegates;
using TagsTreeWpf.Models;
using TagsTreeWpf.Services;
using TagsTreeWpf.Services.ExtensionMethods;
using TagsTreeWpf.ViewModels;
using TagsTreeWpf.Views.Controls;

namespace TagsTreeWpf.Views
{
    /// <summary>
    /// TagEditFilesPage.xaml 的交互逻辑
    /// </summary>
    public partial class TagEditFilesPage : Page
    {
        public TagEditFilesPage()
        {
            DataContext = _vm = new TagEditFilesViewModel();
            InitializeComponent();
        }

        private readonly TagEditFilesViewModel _vm;

        private async void BConfirmClick()
        {
            Tags.BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(500)
            });
            await Task.Delay(500);
            DockPanel.Children.Remove(Tags);
            BConfirm.Content = "保存";
            TbInput.BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500)
            });
            DgResult.BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500)
            });
            TbInput.IsHitTestVisible = true;
            DgResult.IsHitTestVisible = true;
        }

        #region 事件处理

        private void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => TbPath.Path = TbPath.Path.TvSelectedItemChanged((TagModel?)e.NewValue);

        private void ResultChanged(TagSearchBox sender, ResultChangedEventArgs e) => _vm.FileViewModels = e.NewResult.Select(fileModel => new FileViewModel(fileModel, TbPath.Path.GetTagModel()!)).ToObservableCollection();

        private void Selected(object sender, SelectionChangedEventArgs e)
        {
            if ((FileViewModel)((DataGrid)sender).SelectedItem is null) return;
            ((FileViewModel)((DataGrid)sender).SelectedItem).SelectedFlip();
            ((DataGrid)sender).SelectedIndex = -1;
        }

        #endregion

        #region 命令

        private static bool _mode;

        private void ConfirmBClick(object parameter, RoutedEventArgs e)
        {
            if (!_mode)
            {
                if (TbPath.Path.GetTagModel() is not { } pathTagModel)
                {
                    MessageBoxX.Error("「标签路径」不存在！");
                    return;
                }
                if (pathTagModel == App.Tags.TagsDictionaryRoot)
                {
                    MessageBoxX.Error("「标签路径」不能为空！");
                    return;
                }
                BConfirmClick();
                _vm.FileViewModels = App.Relations.GetFileModels().Select(fileModel => new FileViewModel(fileModel, pathTagModel)).ToObservableCollection();
                TbPath.IsEnabled = false;
                _mode = true;
            }
            else
            {
                if (TbPath.Path.GetTagModel() is not { } pathTagModel)
                {
                    MessageBoxX.Error("「标签路径」不存在！"); //理论上不会到达此代码
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
                MessageBoxX.Information("已保存更改");
            }
        }

        #endregion
    }
}
