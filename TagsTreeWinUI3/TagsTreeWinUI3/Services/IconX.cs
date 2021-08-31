using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace TagsTreeWinUI3.Services
{
	public static class IconX
	{
		private const int Size = 32;

		public static async void Initialize()
		{
			await using var ms1 = new MemoryStream(Properties.Resources.Folder);
			using var folder = ms1.AsRandomAccessStream();
			FolderIcon = await GetBitmapImage(folder);
			await using var ms2 = new MemoryStream(Properties.Resources.NotFound);
			using var notFound = ms2.AsRandomAccessStream();
			NotFoundIcon = await GetBitmapImage(notFound);
		}

		private static async Task<BitmapImage> GetBitmapImage(IRandomAccessStream iRandomAccessStream)
		{
			var bitmapImage = new BitmapImage();
			await bitmapImage.SetSourceAsync(iRandomAccessStream);
			return bitmapImage;
		}

		public static async Task<BitmapImage> GetIcon(string extension)
		{
			if (IconList.ContainsKey(extension))
				return IconList[extension];
			var tempFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Temp." + extension, CreationCollisionOption.ReplaceExisting);
			using var storageItemThumbnail = await tempFile.GetThumbnailAsync(ThumbnailMode.SingleItem, Size);
			await tempFile.DeleteAsync();
			var task = GetBitmapImage(storageItemThumbnail);
			IconList[extension] = await task;
			task.Wait();
			return IconList[extension];
		}

		public static BitmapImage FolderIcon { get; private set; } = null!;
		public static BitmapImage NotFoundIcon { get; private set; } = null!;

		private static readonly Dictionary<string, BitmapImage> IconList = new();

	}
}
