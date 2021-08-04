using ModernWpf;
using ModernWpf.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Xml;
using TagsTree.Annotations;
using TagsTree.Commands;
using TagsTree.Services;

namespace TagsTree.ViewModels
{
	public sealed class TagsManagerViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


		public TagsManagerViewModel()
		{
			static bool Func1(object? _) => true;
			bool Func2(object? _) => _clipBoard is not null;
			_newBClick = new RelayCommand(Func1, TagsManagerService.NewBClick);
			_moveBClick = new RelayCommand(Func1, TagsManagerService.MoveBClick);
			_renameBClick = new RelayCommand(Func1, TagsManagerService.RenameBClick);
			_deleteBClick = new RelayCommand(Func1, TagsManagerService.DeleteBClick);
			_saveBClick = new RelayCommand(_ => Changed, TagsManagerService.SaveBClick);
			_newCmClick = new RelayCommand(Func1, TagsManagerService.NewCmClick);
			_newXCmClick = new RelayCommand(Func1, TagsManagerService.NewXCmClick);
			_cutCmClick = new RelayCommand(Func1, TagsManagerService.CutCmClick);
			_pasteCmClick = new RelayCommand(Func2, TagsManagerService.PasteCmClick);
			_pasteXCmClick = new RelayCommand(Func2, TagsManagerService.PasteXCmClick);
			_renameCmClick = new RelayCommand(Func1, TagsManagerService.RenameCmClick);
			_deleteCmClick = new RelayCommand(Func1, TagsManagerService.DeleteCmClick);
			App.XdTagsReload();
			_xdp = new XmlDataProvider { Document = App.XdTags, XPath = @"TagsTree/Tag" };
		}

		public readonly RoutedEventHandler PathComplement = TagsManagerService.PathComplement;
		public readonly Action<object?> TvSelectItemChanged = TagsManagerService.TvSelectItemChanged;
		public readonly Action<XmlElement, XmlElement?> MoveTag = TagsManagerService.MoveTag;
		public readonly TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen = TagsManagerService.SuggestionChosen;
		public readonly TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> NameChanged = TagsManagerService.NameChanged;
		public readonly TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> PathChanged = TagsManagerService.PathChanged;
		public readonly CancelEventHandler Closing = TagsManagerService.Closing;

		private string _name = "";
		private string _path = "";
		private bool _changed;
		private XmlDataProvider _xdp;
		private XmlElement? _clipBoard;

		private RelayCommand _newBClick;
		private RelayCommand _moveBClick;
		private RelayCommand _renameBClick;
		private RelayCommand _deleteBClick;
		private RelayCommand _saveBClick;

		private RelayCommand _newCmClick;
		private RelayCommand _newXCmClick;
		private RelayCommand _cutCmClick;
		private RelayCommand _pasteCmClick;
		private RelayCommand _pasteXCmClick;
		private RelayCommand _renameCmClick;
		private RelayCommand _deleteCmClick;

		public string Name
		{
			get => _name;
			set
			{
				if (Equals(value, _name)) return;
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}
		public string Path
		{
			get => _path;
			set
			{
				if (Equals(value, _path)) return;
				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}
		public bool Changed
		{
			get => _changed;
			set
			{
				if (Equals(value, _changed)) return;
				_changed = value;
				OnPropertyChanged(nameof(Changed));
				_saveBClick.OnCanExecuteChanged();
			}
		}
		public XmlDataProvider Xdp
		{
			get => _xdp;
			set
			{
				if (Equals(value, _xdp)) return;
				_xdp = value;
				OnPropertyChanged(nameof(Xdp));
			}
		}
		public XmlElement? ClipBoard
		{
			get => _clipBoard;
			set
			{
				if (Equals(value, _clipBoard)) return;
				_clipBoard = value;
				_pasteCmClick.OnCanExecuteChanged();
				_pasteXCmClick.OnCanExecuteChanged();
			}
		}

		public RelayCommand NewBClick
		{
			get => _newBClick;
			set
			{
				if (Equals(value, _newBClick)) return;
				_newBClick = value;
				OnPropertyChanged(nameof(NewBClick));
			}
		}
		public RelayCommand MoveBClick
		{
			get => _moveBClick;
			set
			{
				if (Equals(value, _moveBClick)) return;
				_moveBClick = value;
				OnPropertyChanged(nameof(MoveBClick));
			}
		}
		public RelayCommand RenameBClick
		{
			get => _renameBClick;
			set
			{
				if (Equals(value, _renameBClick)) return;
				_renameBClick = value;
				OnPropertyChanged(nameof(RenameBClick));
			}
		}
		public RelayCommand DeleteBClick
		{
			get => _deleteBClick;
			set
			{
				if (Equals(value, _deleteBClick)) return;
				_deleteBClick = value;
				OnPropertyChanged(nameof(DeleteBClick));
			}
		}
		public RelayCommand SaveBClick
		{
			get => _saveBClick;
			set
			{
				if (Equals(value, _saveBClick)) return;
				_saveBClick = value;
				OnPropertyChanged(nameof(SaveBClick));
			}
		}

		public RelayCommand NewCmClick
		{
			get => _newCmClick;
			set
			{
				if (Equals(value, _newCmClick)) return;
				_newCmClick = value;
				OnPropertyChanged(nameof(NewCmClick));
			}
		}
		public RelayCommand NewXCmClick
		{
			get => _newXCmClick;
			set
			{
				if (Equals(value, _newXCmClick)) return;
				_newXCmClick = value;
				OnPropertyChanged(nameof(NewXCmClick));
			}
		}
		public RelayCommand CutCmClick
		{
			get => _cutCmClick;
			set
			{
				if (Equals(value, _cutCmClick)) return;
				_cutCmClick = value;
				OnPropertyChanged(nameof(CutCmClick));
			}
		}
		public RelayCommand PasteCmClick
		{
			get => _pasteCmClick;
			set
			{
				if (Equals(value, _pasteCmClick)) return;
				_pasteCmClick = value;
				OnPropertyChanged(nameof(PasteCmClick));
			}
		}
		public RelayCommand PasteXCmClick
		{
			get => _pasteXCmClick;
			set
			{
				if (Equals(value, _pasteXCmClick)) return;
				_pasteXCmClick = value;
				OnPropertyChanged(nameof(PasteXCmClick));
			}
		}
		public RelayCommand RenameCmClick
		{
			get => _renameCmClick;
			set
			{
				if (Equals(value, _renameCmClick)) return;
				_renameCmClick = value;
				OnPropertyChanged(nameof(RenameCmClick));
			}
		}
		public RelayCommand DeleteCmClick
		{
			get => _deleteCmClick;
			set
			{
				if (Equals(value, _deleteCmClick)) return;
				_deleteCmClick = value;
				OnPropertyChanged(nameof(DeleteCmClick));
			}
		}
	}
}
