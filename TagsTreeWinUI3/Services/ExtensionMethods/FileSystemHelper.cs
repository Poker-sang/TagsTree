using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TagsTreeWinUI3.ViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace TagsTreeWinUI3.Services.ExtensionMethods
{
    public static class FileSystemHelper
    {
        public static bool Exists(this string fullName) => File.Exists(fullName) || Directory.Exists(fullName);

        public static async void Open(this FileViewModel fileViewModel)
        {
            try
            {
                var process = new Process { StartInfo = new ProcessStartInfo(fileViewModel.FullName) };
                process.StartInfo.UseShellExecute = true;
                _ = process.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                await ShowMessageDialog.Information(true, "找不到文件（夹），源文件可能已被更改");
            }
        }
        public static async void Open(this string fullName)
        {
            try
            {
                var process = new Process { StartInfo = new ProcessStartInfo(fullName) };
                process.StartInfo.UseShellExecute = true;
                _ = process.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                await ShowMessageDialog.Information(true, $"打开路径「{AppConfigurations.AppLocalFolder}」时出现错误");
            }
        }
        public static async void OpenDirectory(this FileViewModel fileViewModel)
        {
            try
            {
                var process = new Process { StartInfo = new ProcessStartInfo(fileViewModel.Path) };
                process.StartInfo.UseShellExecute = true;
                _ = process.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                await ShowMessageDialog.Information(true, "找不到目录，源文件目录可能已被更改");
            }
        }

        public static void Move(this FileViewModel fileViewModel, string newFullName)
        {
            var fileModel = fileViewModel.GetFileModel();
            if (fileModel.IsFolder)
                FileSystem.MoveDirectory(fileModel.FullName, newFullName.GetPath(), UIOption.OnlyErrorDialogs);
            FileSystem.MoveFile(fileModel.FullName, newFullName.GetPath(), UIOption.OnlyErrorDialogs);
        }
        public static void Copy(this string sourceDirectory, string destinationDirectory) => FileSystem.CopyDirectory(sourceDirectory, destinationDirectory);

        public static void Rename(this FileViewModel fileViewModel, string newFullName)
        {
            var fileModel = fileViewModel.GetFileModel();
            if (fileModel.IsFolder)
                FileSystem.RenameDirectory(fileModel.FullName, newFullName.GetName());
            FileSystem.RenameFile(fileModel.FullName, newFullName.GetName());
        }
        public static void Delete(this FileViewModel fileViewModel)
        {
            var fileModel = fileViewModel.GetFileModel();

            if (fileModel.IsFolder)
                FileSystem.DeleteDirectory(fileModel.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            FileSystem.DeleteFile(fileModel.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        public static string GetInvalidNameChars => @"\\/:*?""<>|" + new string(Path.GetInvalidPathChars());
        public static string GetInvalidPathChars => @"\/:*?""<>|" + new string(Path.GetInvalidPathChars());

        public enum InvalidMode
        {
            Name = 0,
            Path = 1
        }

        public static string GetName(this string fullName)
        {
            var fullNameSpan = fullName.AsSpan();
            return fullNameSpan[(fullNameSpan.LastIndexOf('\\') + 1)..].ToString();
        }

        public static string GetPath(this string fullName)
        {
            var fullNameSpan = fullName.AsSpan();
            return fullNameSpan.LastIndexOf('\\') is -1
                ? ""
                : fullNameSpan[..fullNameSpan.LastIndexOf('\\')].ToString();
        }

        public static string CountSize(FileInfo file) => " " + file.Length switch
        {
            < 1 << 10 => file.Length.ToString("F2") + "Byte",
            < 1 << 20 => ((double)file.Length / (1 << 10)).ToString("F2") + "KB",
            < 1 << 30 => ((double)file.Length / (1 << 20)).ToString("F2") + "MB",
            _ => ((double)file.Length / (1 << 30)).ToString("F2") + "GB"
        };


        public static async Task<StorageFolder> GetStorageFolder() => await new FolderPicker { FileTypeFilter = { "*" } /*不加会崩溃*/ }.InitializeWithWindow().PickSingleFolderAsync();
        public static async Task<StorageFile> GetStorageFile() => await new FileOpenPicker { FileTypeFilter = { "*" } }.InitializeWithWindow().PickSingleFileAsync();
        public static async Task<IReadOnlyList<StorageFile>> GetStorageFiles() => await new FileOpenPicker { FileTypeFilter = { "*" } }.InitializeWithWindow().PickMultipleFilesAsync();

    }
}