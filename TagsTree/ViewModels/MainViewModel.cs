﻿using ModernWpf;
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
			_removeCmClick = new RelayCommand(Func, MainService.RemoveCmClick);
			_propertiesCmClick = new RelayCommand(Func, MainService.PropertiesCmClick);
		}

		public static Func<bool> CheckConfig => MainService.CheckConfig;
		public static MouseButtonEventHandler MainMouseLeftButtonDown => MainService.MainMouseLeftButtonDown;
		public static MouseButtonEventHandler DgItemMouseDoubleClick => MainService.DgItemMouseDoubleClick;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen => MainService.SuggestionChosen;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged => MainService.TextChanged;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted => MainService.QuerySubmitted;
		

		private RelayCommand _openCmClick;
		private RelayCommand _openExplorerCmClick;
		private RelayCommand _removeCmClick;
		private RelayCommand _propertiesCmClick;

		private ObservableCollection<FileModel> _fileModels = new();
		private FilePropertiesViewModel _fpViewModel = new();
		private string _search = "";
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
		public FilePropertiesViewModel FpViewModel
		{
			get => _fpViewModel;
			set
			{
				if (Equals(_fpViewModel, value)) return;
				_fpViewModel = value;
				OnPropertyChanged(nameof(FpViewModel));
			}
		}
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
			get => _removeCmClick;
			set
			{
				if (Equals(value, _removeCmClick)) return;
				_removeCmClick = value;
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