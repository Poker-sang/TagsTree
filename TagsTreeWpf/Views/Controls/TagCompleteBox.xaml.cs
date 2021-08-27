using JetBrains.Annotations;
using ModernWpf.Controls;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using TagsTreeWpf.Services;
using TagsTreeWpf.Services.ExtensionMethods;

namespace TagsTreeWpf.Views.Controls
{
	/// <summary>
	/// TagCompleteBox.xaml 的交互逻辑
	/// </summary>
	public partial class TagCompleteBox : UserControl, INotifyPropertyChanged
	{
		public TagCompleteBox() => InitializeComponent();
		private void PathComplement(object sender, RoutedEventArgs e) => Path = Path.GetTagModel()?.FullName ?? Path;
		private void PathChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
		{
			Path = Regex.Replace(Path, $@"[{FileX.GetInvalidPathChars}]+", "");
			sender.ItemsSource = sender.Text.TagSuggest('\\');
			var textBox = (TextBox)typeof(AutoSuggestBox).GetField("m_textBox", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sender)!;
			textBox.SelectionStart = textBox.Text.Length;
		}
		private void SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e) => sender.Text = e.SelectedItem.ToString();

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

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	}
}
