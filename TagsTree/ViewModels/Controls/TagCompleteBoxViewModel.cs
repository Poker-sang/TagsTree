using JetBrains.Annotations;
using ModernWpf;
using ModernWpf.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TagsTree.Services.Controls;

namespace TagsTree.ViewModels.Controls
{
	public class TagCompleteBoxViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public static RoutedEventHandler PathComplement => TagCompleteBoxService.PathComplement;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> PathChanged => TagCompleteBoxService.PathChanged;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen => TagCompleteBoxService.SuggestionChosen;

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