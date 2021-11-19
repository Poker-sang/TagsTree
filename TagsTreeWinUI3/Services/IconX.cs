using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using TagsTreeWinUI3.Properties;

namespace TagsTreeWinUI3.Services
{
    public static class IconX
    {
        private const uint Size = 32;

        public static async void Initialize()
        {
            await using var ms1 = new MemoryStream(Resources.Folder);
            using var folder = ms1.AsRandomAccessStream();
            FolderIcon = await GetBitmapImage(folder);
            await using var ms2 = new MemoryStream(Resources.NotFound);
            using var notFound = ms2.AsRandomAccessStream();
            NotFoundIcon = await GetBitmapImage(notFound);
        }
        public static async void LoadFilesIcons()
        {
            foreach (var fileModel in App.IdFile.Values)
                IconList[fileModel.Extension] = NotFoundIcon;
            foreach (var extension in IconList.Keys)
                IconList[extension] = await CreateAndGetIcon(extension);
        }
        private static async Task<BitmapImage> GetBitmapImage(IRandomAccessStream iRandomAccessStream)
        {
            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(iRandomAccessStream);
            return bitmapImage;
        }
        private static async Task<BitmapImage> CreateAndGetIcon(string extension)
        {
            var tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Temp." + extension, CreationCollisionOption.OpenIfExists);
            using var storageItemThumbnail = await tempFile.GetThumbnailAsync(ThumbnailMode.SingleItem, Size);
            await tempFile.DeleteAsync();
            return await GetBitmapImage(storageItemThumbnail);
        }

        public static BitmapImage GetIcon(string extension)
        {
            if (IconList.ContainsKey(extension))
                return IconList[extension];
            //IconList[extension] = CreateAndGetIcon(extension);
            //return IconList[extension];
            return NotFoundIcon;
        }

        public static BitmapImage FolderIcon { get; private set; } = null!;
        public static BitmapImage NotFoundIcon { get; private set; } = null!;

        private static readonly Dictionary<string, BitmapImage> IconList = new();
    }
}
