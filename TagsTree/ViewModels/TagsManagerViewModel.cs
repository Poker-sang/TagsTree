using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using TagsTree.Annotations;
using TagsTree.Commands;

namespace TagsTree.ViewModels
{
	public sealed class TagsManagerViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public XmlElement? ClipBoard { get; set; }

		public TagsManagerViewModel()
		{
			static bool Func1(object? _) => true;
			//bool Func2(object? _) => ClipBoard is not null;
			_newBClick = new TagsManagerButtonCommand(Func1, Services.TagsManagerServices.NewBClick);
			_moveBClick = new TagsManagerButtonCommand(Func1, Services.TagsManagerServices.MoveBClick);
			_renameBClick = new TagsManagerButtonCommand(Func1, Services.TagsManagerServices.RenameBClick);
			_deleteBClick = new TagsManagerButtonCommand(Func1, Services.TagsManagerServices.DeleteBClick);
			_saveBClick = new TagsManagerButtonCommand(Func1, Services.TagsManagerServices.SaveBClick);
			//_newCmClick = new TagsManagerButtonCommand(Func1, Services.TagsManagerServices.NewCmClick);
			//_newXCmClick = new TagsManagerButtonCommand(Func1, Services.TagsManagerServices.NewXCmClick);
			//_cutCmClick = new TagsManagerButtonCommand(Func1, Services.TagsManagerServices.CutCmClick);
			//_pasteCmClick = new TagsManagerButtonCommand(Func2, Services.TagsManagerServices.PasteCmClick);
			//_pasteXCmClick = new TagsManagerButtonCommand(Func2, Services.TagsManagerServices.PasteXCmClick);
			//_renameCmClick = new TagsManagerButtonCommand(Func1, Services.TagsManagerServices.RenameCmClick);
			//_deleteCmClick = new TagsManagerButtonCommand(Func1, Services.TagsManagerServices.DeleteCmClick);
		}

		private string _name = "";
		private string _path = "";
		private bool _changed;
		private XmlDataProvider? _xdp;
		private TagsManagerButtonCommand _newBClick;
		private TagsManagerButtonCommand _moveBClick;
		private TagsManagerButtonCommand _renameBClick;
		private TagsManagerButtonCommand _deleteBClick;
		private TagsManagerButtonCommand _saveBClick;
		//private TagsManagerButtonCommand _newCmClick;
		//private TagsManagerButtonCommand _newXCmClick;
		//private TagsManagerButtonCommand _cutCmClick;
		//private TagsManagerButtonCommand _pasteCmClick;
		//private TagsManagerButtonCommand _pasteXCmClick;
		//private TagsManagerButtonCommand _renameCmClick;
		//private TagsManagerButtonCommand _deleteCmClick;

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}
		public string Path
		{
			get => _path;
			set
			{
				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}
		public bool Changed
		{
			get => _changed;
			set
			{
				_changed = value;
				OnPropertyChanged(nameof(Changed));
			}
		}
		public XmlDataProvider? Xdp
		{
			get => _xdp;
			set
			{
				_xdp = value;
				OnPropertyChanged(nameof(Xdp));
			}
		}
		public TagsManagerButtonCommand NewBClick
		{
			get => _newBClick;
			set
			{
				_newBClick = value;
				OnPropertyChanged(nameof(NewBClick));
			}
		}
		public TagsManagerButtonCommand MoveBClick
		{
			get => _moveBClick;
			set
			{
				_moveBClick = value;
				OnPropertyChanged(nameof(MoveBClick));
			}
		}
		public TagsManagerButtonCommand RenameBClick
		{
			get => _renameBClick;
			set
			{
				_renameBClick = value;
				OnPropertyChanged(nameof(RenameBClick));
			}
		}
		public TagsManagerButtonCommand DeleteBClick
		{
			get => _deleteBClick;
			set
			{
				_deleteBClick = value;
				OnPropertyChanged(nameof(DeleteBClick));
			}
		}
		public TagsManagerButtonCommand SaveBClick
		{
			get => _saveBClick;
			set
			{
				_saveBClick = value;
				OnPropertyChanged(nameof(SaveBClick));
			}
		}

		/*
		public TagsManagerButtonCommand NewCmClick
		{
			get => _newCmClick;
			set
			{
				_newCmClick = value;
				OnPropertyChanged(nameof(NewCmClick));
			}
		}
		public TagsManagerButtonCommand NewXCmClick
		{
			get => _newXCmClick;
			set
			{
				_newXCmClick = value;
				OnPropertyChanged(nameof(NewXCmClick));
			}
		}
		public TagsManagerButtonCommand CutCmClick
		{
			get => _cutCmClick;
			set
			{
				_cutCmClick = value;
				OnPropertyChanged(nameof(CutCmClick));
			}
		}
		public TagsManagerButtonCommand PasteCmClick
		{
			get => _pasteCmClick;
			set
			{
				_pasteCmClick = value;
				OnPropertyChanged(nameof(PasteCmClick));
			}
		}
		public TagsManagerButtonCommand PasteXCmClick
		{
			get => _pasteXCmClick;
			set
			{
				_pasteXCmClick = value;
				OnPropertyChanged(nameof(PasteXCmClick));
			}
		}
		public TagsManagerButtonCommand RenameCmClick
		{
			get => _renameCmClick;
			set
			{
				_renameCmClick = value;
				OnPropertyChanged(nameof(RenameCmClick));
			}
		}
		public TagsManagerButtonCommand DeleteCmClick
		{
			get => _deleteCmClick;
			set
			{
				_deleteCmClick = value;
				OnPropertyChanged(nameof(DeleteCmClick));
			}
		}
		*/
	}
}
