using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace WinUI3Utilities;

public static class PickerHelper
{
    public static async Task<StorageFolder> PickFolderAsync(PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => await new FolderPicker
        {
            FileTypeFilter = { "*" }, /*不加会崩溃*/
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow().PickSingleFolderAsync();

    public static async Task<StorageFile> PickSingleFileAsync(PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => await new FileOpenPicker
        {
            FileTypeFilter = { "*" },
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow().PickSingleFileAsync();

    public static async Task<IReadOnlyList<StorageFile>> PickMultipleFilesAsync(PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
        => await new FileOpenPicker
        {
            FileTypeFilter = { "*" },
            SuggestedStartLocation = suggestedStartLocation,
            ViewMode = viewMode
        }.InitializeWithWindow().PickMultipleFilesAsync();

    [Obsolete($"Use {nameof(PickSingleFileAsync)} instead")]
    public static IAsyncOperation<StorageFile?> PickSaveFileAsync(string suggestedFileName, string fileTypeName, string fileTypeId, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop)
        => new FileSavePicker
        {
            SuggestedStartLocation = suggestedStartLocation,
            FileTypeChoices =
            {
                [fileTypeId] = new List<string> { fileTypeId }
            },
            SuggestedFileName = suggestedFileName
        }.InitializeWithWindow().PickSaveFileAsync();
}
