using JetBrains.Annotations;
using ModernWpf;
using ModernWpf.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Xml;
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
			SaveBClick = new RelayCommand(_ => Changed, TagsManagerService.SaveBClick);
			PasteCmClick = new RelayCommand(_ => _clipBoard is not null, TagsManagerService.PasteCmClick);
			PasteXCmClick = new RelayCommand(_ => _clipBoard is not null, TagsManagerService.PasteXCmClick);
			App.XdTagsReload();
			Xdp = new XmlDataProvider { Document = App.XdTags, XPath = @"TagsTree/Tag" };
		}

		public RelayCommand NewBClick { get; } = new(_ => true, TagsManagerService.NewBClick);
		public RelayCommand MoveBClick { get; } = new(_ => true, TagsManagerService.MoveBClick);
		public RelayCommand RenameBClick { get; } = new(_ => true, TagsManagerService.RenameBClick);
		public RelayCommand DeleteBClick { get; } = new(_ => true, TagsManagerService.DeleteBClick);
		public RelayCommand SaveBClick { get; }
		public RelayCommand NewCmClick { get; } = new(_ => true, TagsManagerService.NewCmClick);
		public RelayCommand NewXCmClick { get; } = new(_ => true, TagsManagerService.NewXCmClick);
		public RelayCommand CutCmClick { get; } = new(_ => true, TagsManagerService.CutCmClick);
		public RelayCommand PasteCmClick { get; }
		public RelayCommand PasteXCmClick { get; }
		public RelayCommand RenameCmClick { get; } = new(_ => true, TagsManagerService.RenameCmClick);
		public RelayCommand DeleteCmClick { get; } = new(_ => true, TagsManagerService.DeleteCmClick);

		public XmlDataProvider Xdp { get; }

		public static RoutedEventHandler PathComplement => TagsManagerService.PathComplement;
		public static RoutedPropertyChangedEventHandler<object> TvSelectItemChanged => TagsManagerService.TvSelectItemChanged;
		public static Action<XmlElement, XmlElement?> MoveTag => TagsManagerService.MoveTag;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen => TagsManagerService.SuggestionChosen;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> NameChanged => TagsManagerService.NameChanged;
		public static TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> PathChanged => TagsManagerService.PathChanged;
		public static CancelEventHandler Closing => TagsManagerService.Closing;

		private string _name = "";
		private string _path = "";
		private bool _changed;
		private XmlElement? _clipBoard;


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
				SaveBClick.OnCanExecuteChanged();
			}
		}
		public XmlElement? ClipBoard
		{
			get => _clipBoard;
			set
			{
				if (Equals(value, _clipBoard)) return;
				_clipBoard = value;
				PasteCmClick.OnCanExecuteChanged();
				PasteXCmClick.OnCanExecuteChanged();
			}
		}
	}
}
