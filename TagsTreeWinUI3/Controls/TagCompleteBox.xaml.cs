using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Controls
{
    /// <summary>
    /// TagCompleteBox.xaml 的交互逻辑
    /// </summary>
    [INotifyPropertyChanged]
    public partial class TagCompleteBox : UserControl
    {
        public TagCompleteBox() => InitializeComponent();
        private void PathComplement(object sender, RoutedEventArgs routedEventArgs) => Path = Path.GetTagViewModel()?.FullName ?? Path;
        private void PathChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            Path = Regex.Replace(Path, $@"[{FileSystemHelper.GetInvalidPathChars}]+", "");
            sender.ItemsSource = sender.Text.TagSuggest('\\');
        }
        private void SuggestionChosen(AutoSuggestBox autoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs e) => autoSuggestBox.Text = e.SelectedItem.ToString();

        private string _path = "";
        public string Path
        {
            get => _path;
            set
            {
                if (Equals(_path, value)) return;
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }
    }
}
