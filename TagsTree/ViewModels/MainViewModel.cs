using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TagsTree.Annotations;
using TagsTree.Commands;
using TagsTree.Models;
using TagsTree.Services;


namespace TagsTree.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public MainViewModel()
		{
			static bool Func(object? _) => true;
			_openCmClick = new RelayCommand(Func, MainService.OpenCmClick);
			_openExplorerCmClick = new RelayCommand(Func, MainService.OpenExplorerCmClick);
			_removeFileCmClick = new RelayCommand(Func, MainService.RemoveFileCmClick);
			_propertiesCmClick = new RelayCommand(Func, MainService.PropertiesCmClick);
		}

		public readonly Func<bool> CheckConfig = MainService.CheckConfig;
		public readonly MouseButtonEventHandler DgItemMouseDoubleClick = MainService.DgItemMouseDoubleClick;
		public readonly TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen = MainService.SuggestionChosen;
		public readonly TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged = MainService.TextChanged;
		public readonly TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted = MainService.QuerySubmitted;
		private ObservableCollection<FileModel> _fileModels = new();

		private RelayCommand _openCmClick;
		private RelayCommand _openExplorerCmClick;
		private RelayCommand _removeFileCmClick;
		private RelayCommand _propertiesCmClick;

		private string _search = "";
		public string Search
		{
			get => _search;
			set
			{
				if (Equals(_search, value)) return;
				_search = value;
				OnPropertyChanged(nameof(Search));
			}
		}
		public ObservableCollection<FileModel> FileModels
		{
			get => _fileModels;
			set
			{
				if (Equals(_fileModels, value)) return;
				_fileModels = value;
				OnPropertyChanged(nameof(FileModels));
			}
		}
		public RelayCommand OpenCmClick
		{
			get => _openCmClick;
			set
			{
				if (Equals(value, _openCmClick)) return;
				_openCmClick = value;
				OnPropertyChanged(nameof(OpenCmClick));
			}
		}
		public RelayCommand OpenExplorerCmClick
		{
			get => _openExplorerCmClick;
			set
			{
				if (Equals(value, _openExplorerCmClick)) return;
				_openExplorerCmClick = value;
				OnPropertyChanged(nameof(OpenExplorerCmClick));
			}
		}
		public RelayCommand RemoveFileCmClick
		{
			get => _removeFileCmClick;
			set
			{
				if (Equals(value, _removeFileCmClick)) return;
				_removeFileCmClick = value;
				OnPropertyChanged(nameof(RemoveFileCmClick));
			}
		}
		public RelayCommand PropertiesCmClick
		{
			get => _propertiesCmClick;
			set
			{
				if (Equals(value, _propertiesCmClick)) return;
				_propertiesCmClick = value;
				OnPropertyChanged(nameof(PropertiesCmClick));
			}
		}

	}
}
