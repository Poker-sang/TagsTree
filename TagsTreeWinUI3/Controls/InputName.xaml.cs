using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.RegularExpressions;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;

namespace TagsTreeWinUI3.Controls
{
    /// <summary>
    /// InputName.xaml 的交互逻辑
    /// </summary>
    public partial class InputName : ContentDialog
    {
        public InputName() => InitializeComponent();

        public void Load(FileSystemHelper.InvalidMode mode, string text = "")
        {
            InitializeComponent();
            switch (mode)
            {
                case FileSystemHelper.InvalidMode.Name:
                    AsBox.PlaceholderText = @"不能包含\/:*?""<>|和除空格外的空白字符";
                    _invalidRegex = FileSystemHelper.GetInvalidNameChars;
                    break;
                case FileSystemHelper.InvalidMode.Path:
                    AsBox.PlaceholderText = @"不能包含/:*?""<>|和除空格外的空白字符";
                    _invalidRegex = FileSystemHelper.GetInvalidPathChars;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
            AsBox.Text = text;
        }

        public bool Canceled = true;
        private string _invalidRegex = "";

        private async void InputName_OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
        {
            if (!new Regex($@"^[^{_invalidRegex}]+$").IsMatch(AsBox.Text))
            {
                await ShowMessageDialog.Information(true, AsBox.PlaceholderText);
                e.Cancel = true;
            }
            else Canceled = false;
        }

        private void InputName_OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) => Canceled = true;
    }
}
