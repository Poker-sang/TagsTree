using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation.Metadata;

namespace WinUI3Utilities;

public static class TitleBarHelper
{
    public static void PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs e) => UpdateAppTitleMargin(sender);

    public static void PaneOpening(NavigationView sender, object e) => UpdateAppTitleMargin(sender);

    public static void DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs e)
    {
        var currentMargin = CurrentContext.TitleBar.Margin;
        CurrentContext.TitleBar.Margin = sender.DisplayMode is NavigationViewDisplayMode.Minimal
            ? new() { Left = sender.CompactPaneLength * 2, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom }
            : new Thickness { Left = sender.CompactPaneLength, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom };

        UpdateAppTitleMargin(sender);
    }

    public static void UpdateAppTitleMargin(NavigationView sender)
    {
        const int smallLeftIndent = 4, largeLeftIndent = 24;

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            CurrentContext.TitleTextBlock.TranslationTransition = new();

            CurrentContext.TitleTextBlock.Translation = (sender.DisplayMode is NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                     sender.DisplayMode is NavigationViewDisplayMode.Minimal
                ? new(smallLeftIndent, 0, 0)
                : new System.Numerics.Vector3(largeLeftIndent, 0, 0);
        }
        else
        {
            var currentMargin = CurrentContext.TitleTextBlock.Margin;

            CurrentContext.TitleTextBlock.Margin = (sender.DisplayMode is NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                     sender.DisplayMode is NavigationViewDisplayMode.Minimal
                ? new() { Left = smallLeftIndent, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom }
                : new Thickness { Left = largeLeftIndent, Top = currentMargin.Top, Right = currentMargin.Right, Bottom = currentMargin.Bottom };
        }
    }

    public static void InitializeTitleBar()
    {
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            CurrentContext.AppTitleBar.ExtendsContentIntoTitleBar = true;
            CurrentContext.AppTitleBar.ButtonBackgroundColor = Colors.Transparent;
            CurrentContext.AppTitleBar.ButtonHoverBackgroundColor = ((SolidColorBrush)CurrentContext.App.Resources["SystemControlBackgroundBaseLowBrush"]).Color;
            CurrentContext.AppTitleBar.ButtonForegroundColor = ((SolidColorBrush)CurrentContext.App.Resources["SystemControlForegroundBaseHighBrush"]).Color;
        }
    }
}
