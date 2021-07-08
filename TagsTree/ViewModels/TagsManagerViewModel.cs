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

		private string _name = "";
		private string _path = "";
		private bool _changed;
		private XmlDataProvider? _xdp;
		private TagsManagerButtonCommand _newBClick = new(_ => true, Services.TagsManagerServices.NewBClick);
		private TagsManagerButtonCommand _moveBClick = new(_ => true, Services.TagsManagerServices.MoveBClick);
		private TagsManagerButtonCommand _renameBClick = new(_ => true, Services.TagsManagerServices.RenameBClick);
		private TagsManagerButtonCommand _deleteBClick = new(_ => true, Services.TagsManagerServices.DeleteBClick);
		private TagsManagerButtonCommand _saveBClick = new(_ => true, Services.TagsManagerServices.SaveBClick);

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
		private TagsManagerButtonCommand _newCmClick = new(_ => true, Services.TagsManagerServices.NewCmClick);
		private TagsManagerButtonCommand _newXCmClick = new(_ => true, Services.TagsManagerServices.NewXCmClick);
		private TagsManagerButtonCommand _cutCmClick = new(_ => true, Services.TagsManagerServices.CutCmClick);
		private TagsManagerButtonCommand _pasteCmClick = new(_ => ClipBoard is not null, Services.TagsManagerServices.PasteCmClick);
		private TagsManagerButtonCommand _pasteXCmClick = new(_ => ClipBoard is not null, Services.TagsManagerServices.PasteXCmClick);
		private TagsManagerButtonCommand _renameCmClick = new(_ => true, Services.TagsManagerServices.RenameCmClick);
		private TagsManagerButtonCommand _deleteCmClick = new(_ => true, Services.TagsManagerServices.DeleteCmClick);
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
