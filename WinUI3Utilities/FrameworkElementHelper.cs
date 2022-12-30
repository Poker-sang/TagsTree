using Microsoft.UI.Xaml;

namespace WinUI3Utilities;

public static class FrameworkElementHelper
{
    public static T GetDataContext<T>(this FrameworkElement frameworkElement)
        => (T)frameworkElement.DataContext;

    public static T GetDataContext<T>(this object frameworkElement)
        => ((FrameworkElement)frameworkElement).GetDataContext<T>();

    public static T GetTag<T>(this FrameworkElement frameworkElement)
        => (T)frameworkElement.Tag;

    public static T GetTag<T>(this object frameworkElement)
        => (T)((FrameworkElement)frameworkElement).Tag;
}
