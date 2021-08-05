using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TagsTree.Annotations;
using TagsTree.Services;

namespace TagsTree.ViewModels
{
	public class TagAddFilesViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public TagAddFilesViewModel()
		{
			App.XdTagsReload();
			_xdp = new XmlDataProvider { Document = App.XdTags, XPath = @"TagsTree/Tag" };
		}
		
		public static RoutedPropertyChangedEventHandler<object> TvSelectItemChanged => TagAddFilesService.TvSelectItemChanged;
		public static Action ConfirmBClick => TagAddFilesService.ConfirmBClick;
		private string _tag = "";
		private XmlDataProvider _xdp;

		public string Tag
		{
			get => _tag;
			set
			{
				if (Equals(_tag, value)) return;
				_tag = value;
				OnPropertyChanged(nameof(Tag));
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
	}
}