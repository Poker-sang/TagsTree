using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using TagsTree.Annotations;
using TagsTree.Commands;
using TagsTree.Services;
using Service = TagsTree.Services.TagsManagerServices;
using static TagsTree.Properties.Settings;

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
			bool Func2(object? _) => ClipBoard is not null;
			_newBClick = new TagsManagerCommand(Func1, Service.NewBClick);
			_moveBClick = new TagsManagerCommand(Func1, Service.MoveBClick);
			_renameBClick = new TagsManagerCommand(Func1, Service.RenameBClick);
			_deleteBClick = new TagsManagerCommand(Func1, Service.DeleteBClick);
			_saveBClick = new TagsManagerCommand(Func1, Service.SaveBClick);
			_newCmClick = new TagsManagerCommand(Func1, Service.NewCmClick);
			_newXCmClick = new TagsManagerCommand(Func1, Service.NewXCmClick);
			_cutCmClick = new TagsManagerCommand(Func1, Service.CutCmClick);
			_pasteCmClick = new TagsManagerCommand(Func2, Service.PasteCmClick);
			_pasteXCmClick = new TagsManagerCommand(Func2, Service.PasteXCmClick);
			_renameCmClick = new TagsManagerCommand(Func1, Service.RenameCmClick);
			_deleteCmClick = new TagsManagerCommand(Func1, Service.DeleteCmClick);
			var xdpDocument = new XmlDocument();
			xdpDocument.Load(Default.ConfigPath + @"\TagsTree.xml");
			_xdp = new XmlDataProvider { Document = xdpDocument, XPath = @"TagsTree/Tag" };
		}

		public readonly RoutedEventHandler NameComplement = Service.NameComplement;
		public readonly RoutedEventHandler PathComplement = Service.PathComplement;
		public readonly Action<object?> TvSelectItemChanged = Service.TvSelectItemChanged;
		public readonly Action<XmlElement, XmlElement?> MoveTag = Service.MoveTag;

		private string _name = "";
		private string _path = "";
		private bool _changed;
		private XmlDataProvider _xdp;

		private TagsManagerCommand _newBClick;
		private TagsManagerCommand _moveBClick;
		private TagsManagerCommand _renameBClick;
		private TagsManagerCommand _deleteBClick;
		private TagsManagerCommand _saveBClick;

		private TagsManagerCommand _newCmClick;
		private TagsManagerCommand _newXCmClick;
		private TagsManagerCommand _cutCmClick;
		private TagsManagerCommand _pasteCmClick;
		private TagsManagerCommand _pasteXCmClick;
		private TagsManagerCommand _renameCmClick;
		private TagsManagerCommand _deleteCmClick;


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
		public XmlDataProvider Xdp
		{
			get => _xdp;
			set
			{
				_xdp = value;
				OnPropertyChanged(nameof(Xdp));
			}
		}

		public TagsManagerCommand NewBClick
		{
			get => _newBClick;
			set
			{
				_newBClick = value;
				OnPropertyChanged(nameof(NewBClick));
			}
		}
		public TagsManagerCommand MoveBClick
		{
			get => _moveBClick;
			set
			{
				_moveBClick = value;
				OnPropertyChanged(nameof(MoveBClick));
			}
		}
		public TagsManagerCommand RenameBClick
		{
			get => _renameBClick;
			set
			{
				_renameBClick = value;
				OnPropertyChanged(nameof(RenameBClick));
			}
		}
		public TagsManagerCommand DeleteBClick
		{
			get => _deleteBClick;
			set
			{
				_deleteBClick = value;
				OnPropertyChanged(nameof(DeleteBClick));
			}
		}
		public TagsManagerCommand SaveBClick
		{
			get => _saveBClick;
			set
			{
				_saveBClick = value;
				OnPropertyChanged(nameof(SaveBClick));
			}
		}

		public TagsManagerCommand NewCmClick
		{
			get => _newCmClick;
			set
			{
				_newCmClick = value;
				OnPropertyChanged(nameof(NewCmClick));
			}
		}
		public TagsManagerCommand NewXCmClick
		{
			get => _newXCmClick;
			set
			{
				_newXCmClick = value;
				OnPropertyChanged(nameof(NewXCmClick));
			}
		}
		public TagsManagerCommand CutCmClick
		{
			get => _cutCmClick;
			set
			{
				_cutCmClick = value;
				OnPropertyChanged(nameof(CutCmClick));
			}
		}
		public TagsManagerCommand PasteCmClick
		{
			get => _pasteCmClick;
			set
			{
				_pasteCmClick = value;
				OnPropertyChanged(nameof(PasteCmClick));
			}
		}
		public TagsManagerCommand PasteXCmClick
		{
			get => _pasteXCmClick;
			set
			{
				_pasteXCmClick = value;
				OnPropertyChanged(nameof(PasteXCmClick));
			}
		}
		public TagsManagerCommand RenameCmClick
		{
			get => _renameCmClick;
			set
			{
				_renameCmClick = value;
				OnPropertyChanged(nameof(RenameCmClick));
			}
		}
		public TagsManagerCommand DeleteCmClick
		{
			get => _deleteCmClick;
			set
			{
				_deleteCmClick = value;
				OnPropertyChanged(nameof(DeleteCmClick));
			}
		}
	}
}
