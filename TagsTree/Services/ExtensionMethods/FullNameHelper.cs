using System;

namespace TagsTree.Services.ExtensionMethods;

public static class FullNameHelper
{
    public static string GetPartialPath(this string fullName) => fullName.Replace(AppContext.AppConfig.LibraryPath, "...");

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
}
