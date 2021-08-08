using ModernWpf.Controls;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels.Controls;
using TagsTree.Views.Controls;

namespace TagsTree.Services.Controls
{
	public static class TagCompleteBoxService
	{
		private static TagCompleteBoxViewModel Vm;
		public static void Load(TagCompleteBox tagSuggestBox) => Vm = (TagCompleteBoxViewModel)tagSuggestBox.AutoSuggestBox.DataContext;
		public static void PathComplement(object sender, RoutedEventArgs e) => Vm.Path = Vm.Path.GetTagModel()?.FullName ?? Vm.Path;
		public static void PathChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
		{
			Vm.Path = Regex.Replace(Vm.Path, $@"[{App.FileX.GetInvalidPathChars}]+", "");
			sender.ItemsSource = App.TagMethods.TagSuggest(sender.Text, '\\');
			var textBox = (TextBox)typeof(AutoSuggestBox).GetField("m_textBox", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sender)!;
			textBox.SelectionStart = textBox.Text.Length;
		}
		public static void SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e) => sender.Text = e.SelectedItem.ToString();
	}
}