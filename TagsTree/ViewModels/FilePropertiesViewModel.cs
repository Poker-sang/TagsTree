using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TagsTree.Annotations;
using TagsTree.Models;

namespace TagsTree.ViewModels
{
	public class FilePropertiesViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public FilePropertiesViewModel(FileModel file)
		{
			if (file.IsFolder)
			{
				_name = file.Name;
				_extension = "文件夹";
			}
			else
			{
				_extension = file.Name.Split('.', StringSplitOptions.RemoveEmptyEntries).Last().ToUpper();
				_name = file.Name[..(file.Name.Length - _extension.Length - 1)];
			}
			_path = file.PartialPath;
			_icon = CIconOfPath.IconOfPathLarge(file.FullName, true, true);
			var tags = App.Relations.GetTags(file).Aggregate("", (current, tag) => current + " " + tag);
			_tags = tags is "" ? "" : tags[1..];
		}

		private ImageSource _icon;
		private string _name;
		private string _path;
		private string _extension;
		private string _tags;

		public ImageSource Icon
		{
			get => _icon;
			set
			{
				if (Equals(_icon, value)) return;
				_icon = value;
				OnPropertyChanged(nameof(Icon));
			}
		}
		public string Name
		{
			get => _name;
			set
			{
				if (Equals(_name, value)) return;
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}
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
		public string Extension
		{
			get => _extension;
			set
			{
				if (Equals(_extension, value)) return;
				_extension = value;
				OnPropertyChanged(nameof(Extension));
			}
		}
		public string Tags
		{
			get => _tags;
			set
			{
				if (Equals(_tags, value)) return;
				_tags = value;
				OnPropertyChanged(nameof(Tags));
			}
		}
	}
}
