using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
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
        _notFoundIcon = GetBitmapImage(ms1.AsRandomAccessStream());

        using var ms2 = new MemoryStream(Resources.Loading);
        _loadingIcon = GetBitmapImage(ms2.AsRandomAccessStream());

        using var ms3 = new MemoryStream(Resources.Folder);
        _iconList["文件夹"] = GetBitmapImage(ms3.AsRandomAccessStream());

        using var ms4 = new MemoryStream(Resources.Link);
        _iconList["LNK"] = _iconList["URL"] = GetBitmapImage(ms4.AsRandomAccessStream());

        foreach (var file in Directory.GetFiles(ApplicationData.Current.TemporaryFolder.Path, "Temp.*"))
            File.Delete(file);

        await Task.Yield();
        foreach (var extension in App.IdFile.Values.Select(file => file.Extension).Where(extension => !_iconList.ContainsKey(extension)).Distinct())
        {
            _iconRequest.Enqueue(new(extension));
            _iconList[extension] = null;
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
            return _notFoundIcon;
        // 如果图标列表已经有该扩展名项
        if (_iconList.TryGetValue(fileViewModel.Extension, out var icon))
            // 且加载完成
            if (icon is not null)
                return icon;
            else
            {
                _iconRequest.First(iconGetter => iconGetter.Extension == fileViewModel.Extension).CallBack +=
                    fileViewModel.IconChange;
                return _loadingIcon;
            }

        _iconRequest.Enqueue(new(fileViewModel.Extension, fileViewModel.IconChange));
        _iconList[fileViewModel.Extension] = null;
        if (_iconRequest.Count <= 1)
            _ = StartAsync();
        return _loadingIcon;
    }

    /// <summary>
    /// 异步处理请求加载图标的列表里所有图标的加载
    /// </summary>
    private static async Task StartAsync()
    {
        while (_iconRequest.TryPeek(out var item))
        {
            _iconList[item.Extension] = await CreateIcon(item.Extension);
            item.CallBack();
            _ = _iconRequest.TryDequeue(out _);
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
    private static BitmapImage _notFoundIcon = null!;

    /// <summary>
    /// 加载中的图标
    /// </summary>
    private static BitmapImage _loadingIcon = null!;

    /// <summary>
    /// 请求加载图标的列表
    /// </summary>
    private static readonly ConcurrentQueue<IconGetter> _iconRequest = new();

    /// <summary>
    /// 图标字典，键是扩展名，值是图标
    /// </summary>
    private static readonly Dictionary<string, BitmapImage?> _iconList = new();
}
