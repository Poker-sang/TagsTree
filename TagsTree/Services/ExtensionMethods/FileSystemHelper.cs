using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using TagsTree.Interfaces;
using TagsTree.Models;

namespace TagsTree.Services.ExtensionMethods;

public static class FileSystemHelper
{
    public static bool Exists(this string fullName) => File.Exists(fullName) || Directory.Exists(fullName);

    public static async void Open(this IFullName fileBase)
    {
        try
        {
            var process = new Process { StartInfo = new(fileBase.FullName) };
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
            var process = new Process { StartInfo = new(fullName) };
            process.StartInfo.UseShellExecute = true;
            _ = process.Start();
        }
        catch (System.ComponentModel.Win32Exception)
        {
            await ShowMessageDialog.Information(true, $"打开路径「{fullName}」时出现错误");
        }
    }

    public static async void OpenDirectory(this IFullName fileBase)
    {
        try
        {
            var process = new Process { StartInfo = new(fileBase.Path) };
            process.StartInfo.UseShellExecute = true;
            _ = process.Start();
        }
        catch (System.ComponentModel.Win32Exception)
        {
            await ShowMessageDialog.Information(true, "找不到目录，源文件目录可能已被更改");
        }
    }

    public static void Move(this FileBase fileBase, string newFullName)
    {
        if (fileBase.IsFolder)
            FileSystem.MoveDirectory(fileBase.FullName, newFullName.GetPath(), UIOption.OnlyErrorDialogs);
        else
            FileSystem.MoveFile(fileBase.FullName, newFullName.GetPath(), UIOption.OnlyErrorDialogs);
    }

    public static void Copy(this string sourceDirectory, string destinationDirectory) => FileSystem.CopyDirectory(sourceDirectory, destinationDirectory);

    public static void Rename(this FileBase fileBase, string newFullName)
    {
        if (fileBase.IsFolder)
            FileSystem.RenameDirectory(fileBase.FullName, newFullName.GetName());
        else
            FileSystem.RenameFile(fileBase.FullName, newFullName.GetName());
    }

    public static void Delete(this FileBase fileBase)
    {
        if (fileBase.IsFolder)
            FileSystem.DeleteDirectory(fileBase.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        else
            FileSystem.DeleteFile(fileBase.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
    }

    public static string GetInvalidNameChars => @"\\/:*?""<>|" + new string(Path.GetInvalidPathChars());

    public static string GetInvalidPathChars => @"\/:*?""<>|" + new string(Path.GetInvalidPathChars());

    public enum InvalidMode
    {
        Name = 0,
        Path = 1
    }

    public static string CountSize(FileInfo file) => " " + file.Length switch
    {
        < 1 << 10 => file.Length.ToString("F2") + "Byte",
        < 1 << 20 => ((double)file.Length / (1 << 10)).ToString("F2") + "KB",
        < 1 << 30 => ((double)file.Length / (1 << 20)).ToString("F2") + "MB",
        _ => ((double)file.Length / (1 << 30)).ToString("F2") + "GB"
    };
}
