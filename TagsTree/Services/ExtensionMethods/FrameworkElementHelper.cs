using Microsoft.UI.Xaml;

namespace TagsTree.Services.ExtensionMethods;

public static class FrameworkElementHelper
{
    public static T GetTag<T>(this object frameworkElement)
        => (T)((FrameworkElement)frameworkElement).Tag!;
}
