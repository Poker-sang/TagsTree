using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Concurrent;
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
    /// 将已有文件列表里所有文件图标预加载
    /// </summary>
    public static async void LoadFilesIcons()
    {
        using var ms1 = new MemoryStream(Resources.NotFound);
        NotFoundIcon = GetBitmapImage(ms1.AsRandomAccessStream());

        using var ms2 = new MemoryStream(Resources.Loading);
        LoadingIcon = GetBitmapImage(ms2.AsRandomAccessStream());

        using var ms3 = new MemoryStream(Resources.Folder);
        IconList["文件夹"] = GetBitmapImage(ms3.AsRandomAccessStream());

        using var ms4 = new MemoryStream(Resources.Link);
        IconList["LNK"] = IconList["URL"] = GetBitmapImage(ms4.AsRandomAccessStream());

        foreach (var file in Directory.GetFiles(ApplicationData.Current.TemporaryFolder.Path, "Temp.*"))
            File.Delete(file);

        await Task.Yield();
        foreach (var extension in App.IdFile.Values.Select(file => file.Extension).Where(extension => !IconList.ContainsKey(extension)).Distinct())
        {
            IconRequest.Enqueue(new IconGetter(extension));
            IconList[extension] = null;
        }

        _ = StartAsync();
    }

    /// <summary>
    /// 同步获取图标，如果没有加载图标则先返回加载图片，在加载后更新UI
    /// </summary>
    /// <param name="fileViewModel">文件</param>
    /// <returns>图标</returns>
    public static BitmapImage GetIcon(this FileViewModel fileViewModel)
    {
        if (!fileViewModel.Exists)
            return NotFoundIcon;
        //如果图标列表已经有该扩展名项
        if (IconList.TryGetValue(fileViewModel.Extension, out var icon))
            //且加载完成
            if (icon is not null)
                return icon;
            else
            {
                IconRequest.First(iconGetter => iconGetter.Extension == fileViewModel.Extension).CallBack +=
                    fileViewModel.IconChange;
                return LoadingIcon;
            }

        IconRequest.Enqueue(new IconGetter(fileViewModel.Extension, fileViewModel.IconChange));
        IconList[fileViewModel.Extension] = null;
        if (IconRequest.Count <= 1)
            _ = StartAsync();
        return LoadingIcon;
    }

    /// <summary>
    /// 异步处理请求加载图标的列表里所有图标的加载
    /// </summary>
    private static async Task StartAsync()
    {
        while (IconRequest.TryDequeue(out var item))
        {
            IconList[item.Extension] = await CreateIcon(item.Extension);
            item.CallBack();
        }
    }

    /// <summary>
    /// 从流中获取图标
    /// </summary>
    /// <param name="iRandomAccessStream">流</param>
    /// <returns>图标</returns>
    private static async Task<BitmapImage> GetBitmapImageAsync(IRandomAccessStream iRandomAccessStream)
    {
        var bitmapImage = new BitmapImage();
        await bitmapImage.SetSourceAsync(iRandomAccessStream);
        return bitmapImage;
    }

    /// <summary>
    /// 从流中获取图标
    /// </summary>
    /// <param name="iRandomAccessStream">流</param>
    /// <returns>图标</returns>
    private static BitmapImage GetBitmapImage(IRandomAccessStream iRandomAccessStream)
    {
        var bitmapImage = new BitmapImage();
        bitmapImage.SetSource(iRandomAccessStream);
        return bitmapImage;
    }

    /// <summary>
    /// 根据后缀名获取图标
    /// </summary>
    /// <param name="extension">后缀名</param>
    /// <returns>图标</returns>
    private static async Task<BitmapImage> CreateIcon(string extension)
    {
        var tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Temp." + extension, CreationCollisionOption.FailIfExists);
        using var storageItemThumbnail = await tempFile.GetThumbnailAsync(ThumbnailMode.SingleItem, Size);
        await tempFile.DeleteAsync();
        return await GetBitmapImageAsync(storageItemThumbnail);
    }

    /// <summary>
    /// 包含扩展名和对应的回调函数（用于更新UI）
    /// </summary>
    private class IconGetter
    {
        public IconGetter(string extension)
        {
            Extension = extension;
            CallBack = () => { };
        }
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
    private static BitmapImage NotFoundIcon = null!;
    /// <summary>
    /// 加载中的图标
    /// </summary>
    private static BitmapImage LoadingIcon = null!;

    /// <summary>
    /// 请求加载图标的列表
    /// </summary>
    private static readonly ConcurrentQueue<IconGetter> IconRequest = new();
    /// <summary>
    /// 图标字典，键是扩展名，值是图标
    /// </summary>
    private static readonly Dictionary<string, BitmapImage?> IconList = new();
}
