using System;

namespace TagsTree.Services.ExtensionMethods;

public static class FullNameHelper
{
    public static string GetPartialPath(this string fullName) => fullName.Contains(App.AppConfiguration.LibraryPath) ? "..." + fullName.AsSpan()[App.AppConfiguration.LibraryPath.Length..].ToString() : fullName;
    public static string GetPartialPath(this Interfaces.IFullName fullName) => GetPartialPath(fullName.Path);
}