using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TagsTreeWinUI3.Services;
using TagsTreeWinUI3.Services.ExtensionMethods;

namespace TagsTreeWinUI3.Controls
{
    /// <summary>
    /// TagCompleteBox.xaml 的交互逻辑
    /// </summary>
    public partial class TagCompleteBox : UserControl, INotifyPropertyChanged
    {
        public TagCompleteBox() => InitializeComponent();
        private void PathComplement(object sender, RoutedEventArgs routedEventArgs) => Path = Path.GetTagViewModel()?.FullName ?? Path;
        private void PathChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            Path = Regex.Replace(Path, $@"[{FileX.GetInvalidPathChars}]+", "");
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
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
