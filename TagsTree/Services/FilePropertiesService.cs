using TagsTree.Views;

namespace TagsTree.Services
{
	public static class FilePropertiesService
	{
		private static FileProperties ContentDialog;

		public static void Load(FileProperties contentDialog)
		{
			ContentDialog = contentDialog;
		}
	}
}
