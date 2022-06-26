namespace TagsTree.Services.ExtensionMethods;

public static class FullNameHelper
{
    public static string GetPartialPath(this string fullName) => fullName.Replace(App.AppConfiguration.LibraryPath, "...");
    public static string GetPartialPath(this Interfaces.IFullName fullName) => fullName.Path.GetPartialPath();
}
