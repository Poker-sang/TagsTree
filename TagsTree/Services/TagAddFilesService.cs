using System.Windows;
using System.Xml;
using TagsTree.ViewModels;
using TagsTree.Views;
using System.Windows.Input;

namespace TagsTree.Services
{
	public static class TagAddFilesService
	{
		private static TagAddFilesViewModel Vm;
		private static TagAddFiles Win;
		public static void Load(TagAddFiles window)
		{
			Win = window;
			Vm = (TagAddFilesViewModel)window.DataContext;
		}
		public static void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => Vm.Tag = App.TvSelectedItemChanged((XmlElement?)e.NewValue) ?? Vm.Tag;


		public static void ConfirmBClick()
		{
			if (!Win.Mode)
			{


				Win.Mode = true;
			}
			else Win.Close();
		}
	}
}