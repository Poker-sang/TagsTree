﻿using ModernWpf.Controls;
using TagsTree.Delegates;
using TagsTree.Services.Controls;

namespace TagsTree.Views.Controls
{
	/// <summary>
	/// FileProperties.xaml 的交互逻辑
	/// </summary>
	public partial class FileProperties : ContentDialog
	{
		public FileProperties()
		{
			InitializeComponent();
			FilePropertiesService.Load(this);
		}

		public event FileRemovedEventHandler FileRemoved;
		public void OnFileRemoved(FileRemovedEventArgs e) => FileRemoved.Invoke(this, e);
	}
}