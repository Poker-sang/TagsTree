using System;

namespace TagsTree.Services.ExtensionMethods
{
    public static class FullNameHelper
    {
        public static string GetPartialPath(this string fullName) => fullName.Contains(App.AppConfigurations.LibraryPath) ? "..." + fullName.AsSpan()[App.AppConfigurations.LibraryPath.Length..].ToString() : fullName;
        public static string GetPartialPath(this Interfaces.IFullName fullName) => GetPartialPath(fullName.Path);
    }
}