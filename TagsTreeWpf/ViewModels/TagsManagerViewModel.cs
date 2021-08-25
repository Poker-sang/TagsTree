using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace TagsTreeWpf.ViewModels
{
	public sealed class TagsManagerViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public TagsManagerViewModel()
		{
			App.XdTagsReload();
			Xdp = new XmlDataProvider { Document = App.XdTags, XPath = @"TagsTree/Tag" };
		}

		public XmlDataProvider Xdp { get; }

		private string _name = "";
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
	}
}
