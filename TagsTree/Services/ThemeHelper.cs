using Microsoft.UI.Xaml;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Services;

public class ThemeHelper
{
    public static ElementTheme RootTheme
    {
        get => WindowHelper.Window.Content is FrameworkElement rootElement
            ? rootElement.RequestedTheme
            : ElementTheme.Default;
        set
        {
            if (WindowHelper.Window.Content is FrameworkElement rootElement)
                rootElement.RequestedTheme = value;

            App.AppConfiguration.Theme = (int)value;
            AppContext.ChangeTheme = (int)value;
        }
    }
}
