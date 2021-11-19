using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TagsTreeWinUI3.Commands;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;
using TagsTreeWinUI3.ViewModels;

namespace TagsTreeWinUI3.Views
{
    public partial class TagsManagerPage : Page
    {
        public TagsManagerPage()
        {
            Current = this;
            _vm = new TagsManagerViewModel();
            RPasteCmClick = new RelayCommand(_ => _clipBoard is not null, PasteCmClick);
            InitializeComponent();
        }

        public static TagsManagerPage Current = null!;

        private readonly TagsManagerViewModel _vm;
        public RelayCommand RPasteCmClick { get; }

        private TagViewModel? _clipBoard;
        private TagViewModel? ClipBoard
        {
            get => _clipBoard;
            set
            {
                if (Equals(value, _clipBoard)) return;
                _clipBoard = value;
                CmPPasteX.IsEnabled = value is not null;
                RPasteCmClick.OnCanExecuteChanged();
            }
        }

        #region 事件处理

        private void TvTags_OnDragItemsCompleted(TreeView sender, TreeViewDragItemsCompletedEventArgs e) => BSave.IsEnabled = true;

        private void NameChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e) => _vm.Name = Regex.Replace(_vm.Name, $@"[{FileSystemHelper.GetInvalidNameChars}]+", "");

        private void Tags_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs e) => TbPath.Path = ((TagViewModel?)e.InvokedItem)?.FullName ?? TbPath.Path;

        private async void NewBClick(object sender, RoutedEventArgs e)
        {
            if (TbPath.Path.GetTagViewModel() is not { } pathTagModel)
            {
                await MessageDialogX.Information(true, "「标签路径」不存在！");
                return;
            }
            if (!await NewTagCheck(_vm.Name)) return;
            NewTag(_vm.Name, pathTagModel);
            _vm.Name = "";
        }
        private async void MoveBClick(object sender, RoutedEventArgs e)
        {
            if (TbPath.Path.GetTagViewModel() is not { } pathTagModel)
            {
                await MessageDialogX.Information(true, "「标签路径」不存在！");
                return;
            }
            if (_vm.Name.GetTagViewModel() is not { } nameTagModel)
            {
                await MessageDialogX.Information(true, "「标签名称」不存在！");
                return;
            }
            MoveTag(nameTagModel, pathTagModel);
            _vm.Name = "";
        }
        private async void RenameBClick(object sender, RoutedEventArgs e)
        {
            if (TbPath.Path is "")
            {
                await MessageDialogX.Information(true, "未输入希望重命名的标签");
                return;
            }
            if (TbPath.Path.GetTagViewModel() is not { } pathTagModel)
            {
                await MessageDialogX.Information(true, "「标签路径」不存在！");
                return;
            }
            if (!await NewTagCheck(_vm.Name)) return;
            RenameTag(_vm.Name, pathTagModel);
            _vm.Name = "";
            TbPath.Path = "";
        }
        private async void DeleteBClick(object sender, RoutedEventArgs e)
        {
            if (TbPath.Path is "")
            {
                await MessageDialogX.Information(true, "未输入希望删除的标签");
                return;
            }
            if (TbPath.Path.GetTagViewModel() is not { } pathTagModel)
            {
                await MessageDialogX.Information(true, "「标签路径」不存在！");
                return;
            }
            DeleteTag(pathTagModel);
            _vm.Name = "";
        }
        private void SaveBClick(object sender, RoutedEventArgs e)
        {
            App.SaveTags(_vm.TagsSource);
            App.SaveRelations();
            BSave.IsEnabled = false;
        }

        private async void NewCmClick(object sender, RoutedEventArgs e)
        {
            InputName.Load(FileSystemHelper.InvalidMode.Name);
            await InputName.ShowAsync();
            if (!InputName.Canceled && await NewTagCheck(InputName.AsBox.Text))
                NewTag(InputName.AsBox.Text, (TagViewModel)((MenuFlyoutItem)sender).Tag!);
        }
        private async void NewXCmClick(object sender, RoutedEventArgs e)
        {
            InputName.Load(FileSystemHelper.InvalidMode.Name);
            await InputName.ShowAsync();
            if (!InputName.Canceled && await NewTagCheck(InputName.AsBox.Text))
                NewTag(InputName.AsBox.Text, App.Tags.TagsTree);
        }
        private void CutCmClick(object sender, RoutedEventArgs e) => ClipBoard = (TagViewModel)((MenuFlyoutItem)sender).Tag!;
        private async void RenameCmClick(object sender, RoutedEventArgs e)
        {
            InputName.Load(FileSystemHelper.InvalidMode.Name);
            await InputName.ShowAsync();
            if (!InputName.Canceled && await NewTagCheck(InputName.AsBox.Text))
                RenameTag(InputName.AsBox.Text, (TagViewModel)((MenuFlyoutItem)sender).Tag!);
        }
        private void PasteXCmClick(object sender, RoutedEventArgs e)
        {
            MoveTag(ClipBoard!, App.Tags.TagsTree);
            ClipBoard = null;
        }

        private void DeleteCmClick(object sender, RoutedEventArgs e) => DeleteTag((TagViewModel)((MenuFlyoutItem)sender).Tag!);

        #endregion

        #region 命令

        private void PasteCmClick(object? parameter)
        {
            MoveTag(ClipBoard!, (TagViewModel)parameter!);
            ClipBoard = null;
        }

        #endregion

        #region 操作

        private void NewTag(string name, TagViewModel path)
        {
            App.Tags.AddTag(path, name);
            App.Relations.NewTag(path);
            BSave.IsEnabled = true;
        }

        private async void MoveTag(TagViewModel name, TagViewModel path)
        {
            if (name == path || App.Tags.TagsDictionary.GetValueOrDefault(name.Id)!.HasChildTag(App.Tags.TagsDictionary.GetValueOrDefault(path.Id)!))
            {
                await MessageDialogX.Information(true, "禁止将标签移动到自己目录下");
                return;
            }
            App.Tags.MoveTag(name, path);
            BSave.IsEnabled = true;
        }

        private void RenameTag(string name, TagViewModel path)
        {
            App.Tags.RenameTag(path, name);
            BSave.IsEnabled = true;
        }
        private void DeleteTag(TagViewModel path)
        {
            App.Tags.DeleteTag(path);
            App.Relations.DeleteTag(path);
            BSave.IsEnabled = true;
        }

        private static async Task<bool> NewTagCheck(string name)
        {
            if (name is "")
            {
                await MessageDialogX.Information(true, "标签名称不能为空！");
                return false;
            }
            if (name.GetTagViewModel() is not null)
            {
                await MessageDialogX.Information(true, "与现有标签重名！");
                return false;
            }
            return true;
        }

        #endregion
    }
}
