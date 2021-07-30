using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagsTree.Models;
using TagsTree.ViewModels;
using TagsTree.Views;

namespace TagsTree.Services
{
	public static class FilePropertiesService
	{
		private static FilePropertiesViewModel Vm;
		private static FileProperties ContentDialog;

		public static FilePropertiesViewModel Load(FileProperties contentDialog,FileModel file)
		{
			ContentDialog = contentDialog;
			Vm = new FilePropertiesViewModel(file);
			return Vm;
		}
	}
}
