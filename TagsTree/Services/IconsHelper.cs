using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagsTree.Properties;
using TagsTree.ViewModels;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace TagsTree.Services;

public static class IconsHelper
{
    /// <summary>
    /// 加载四个常用图标
    /// </summary>
    public static async void Initialize()
    {
        await using var ms1 = new MemoryStream(Resources.NotFound);
        _notFoundIcon = await GetBitmapImage(ms1.AsRandomAccessStream());

        await using var ms2 = new MemoryStream(Resources.Loading);
        _loadingIcon = await GetBitmapImage(ms2.AsRandomAccessStream());

        await using var ms3 = new MemoryStream(Resources.Folder);
        IconList["文件夹"] = await GetBitmapImage(ms3.AsRandomAccessStream());

        await using var ms4 = new MemoryStream(Resources.Link);
        IconList["LNK"] = IconList["URL"] = await GetBitmapImage(ms4.AsRandomAccessStream());
    }

    /// <summary>
    /// 将已有文件列表里所有文件图标预加载
    /// </summary>
    public static void LoadFilesIcons()
    {
        //InnerLoadFilesIcons();
        //static async void InnerLoadFilesIcons()
        //{
        //    foreach (var (extension, _) in IconList) []
        //        IconList[extension] = await CreateIcon(extension);
        //}
    }

    /// <summary>
    /// 同步获取图标，如果没有加载图标则先返回加载图片，在加载后更新UI
    /// </summary>
    /// <param name="fileViewModel">文件</param>
    /// <returns>图标</returns>
    public static BitmapImage GetIcon(this FileViewModel fileViewModel)
    {
        if (!fileViewModel.Exists)
            return _notFoundIcon;
        //如果图标列表已经有该扩展名项
        if (IconList.ContainsKey(fileViewModel.Extension))
        {
            //若请求列表已有该扩展名项
            foreach (var iconGetter in IconRequest.Where(iconGetter => iconGetter.Extension == fileViewModel.Extension))
            {
                //在加载完成后通知UI
                iconGetter.CallBack += fileViewModel.IconChange;
                return _loadingIcon;
            }
            //否则说明已经加载完成，直接返回图标对象
            return IconList[fileViewModel.Extension];
        }
        //若图标列表没有，且请求列表已有该扩展名项
        foreach (var iconGetter in IconRequest.Where(iconGetter => iconGetter.Extension == fileViewModel.Extension))
        {
            //在加载完成后通知UI
            iconGetter.CallBack += fileViewModel.IconChange;
            return _loadingIcon;
        }

        IconRequest.Add(new IconGetter(fileViewModel.Extension, fileViewModel.IconChange));
        if (IconRequest.Count <= 1)
            _ = StartAsync();
        return _loadingIcon;
    }

    /// <summary>
    /// 异步处理请求加载图标的列表里所有图标的加载
    /// </summary>
    private static async Task StartAsync()
    {
        while (IconRequest.Count is not 0)
        {
            IconList[IconRequest[0].Extension] = await CreateIcon(IconRequest[0].Extension);
            IconRequest[0].CallBack();
            IconRequest.RemoveAt(0);
        }
    }

    /// <summary>
    /// 从流中获取图标
    /// </summary>
    /// <param name="iRandomAccessStream">流</param>
    /// <returns>图标</returns>
    private static async Task<BitmapImage> GetBitmapImage(IRandomAccessStream iRandomAccessStream)
    {
        var bitmapImage = new BitmapImage();
        await bitmapImage.SetSourceAsync(iRandomAccessStream);
        return bitmapImage;
    }

    /// <summary>
    /// 根据后缀名获取图标
    /// </summary>
    /// <param name="extension">后缀名</param>
    /// <returns>图标</returns>
    private static async Task<BitmapImage> CreateIcon(string extension)
    {
        var tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Temp." + extension, CreationCollisionOption.OpenIfExists);
        using var storageItemThumbnail = await tempFile.GetThumbnailAsync(ThumbnailMode.SingleItem, Size);
        await tempFile.DeleteAsync();
        return await GetBitmapImage(storageItemThumbnail);
    }

    /// <summary>
    /// 包含扩展名和对应的回调函数（用于更新UI）
    /// </summary>
    private class IconGetter
    {
        public IconGetter(string extension, Action callBack)
        {
            Extension = extension;
            CallBack = callBack;
        }
        public readonly string Extension;
        public Action CallBack;
    }

    /// <summary>
    /// 图标边长
    /// </summary>
    private const uint Size = 32;

    /// <summary>
    /// 文件不存在的图标
    /// </summary>
    private static BitmapImage _notFoundIcon = null!;
    /// <summary>
    /// 加载中的图标
    /// </summary>
    private static BitmapImage _loadingIcon = null!;

    /// <summary>
    /// 请求加载图标的列表
    /// </summary>
    private static readonly List<IconGetter> IconRequest = new();
    /// <summary>
    /// 图标字典，键是扩展名，值是图标
    /// </summary>
    private static readonly Dictionary<string, BitmapImage> IconList = new();
}