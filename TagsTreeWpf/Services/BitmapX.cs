using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace TagsTreeWpf.Services
{
    public static class BitmapX
    {
        private static BitmapImage BitmapToBitmapImage(Image bitmap)
        {
            using var stream = new MemoryStream();
            var result = new BitmapImage();
            bitmap.Save(stream, ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

            stream.Position = 0;
            result.BeginInit();
            // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
            // Force the bitmap to load right now so we can dispose the stream.
            result.CacheOption = BitmapCacheOption.OnLoad;
            result.StreamSource = stream;
            result.EndInit();
            result.Freeze();
            return result;
        }

        public static readonly BitmapImage FolderIcon = BitmapToBitmapImage(Properties.Resources.Folder);

        public static readonly BitmapImage NotFoundIcon = BitmapToBitmapImage(Properties.Resources.NotFound);
    }
}