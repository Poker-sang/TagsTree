using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT.Interop;

namespace WinUI3Utilities;

public static class CurrentContext
{
    private static Window _window = null!;

    public static Application App { get; set; } = null!;

    public static Window Window
    {
        get => _window;
        set
        {
            _window = value;
            HWnd = WindowNative.GetWindowHandle(_window);
            WindowId = Win32Interop.GetWindowIdFromWindow(HWnd);
            AppWindow = AppWindow.GetFromWindowId(WindowId);
            AppTitleBar = AppWindow.TitleBar;
        }
    }

    public static nint HWnd { get; private set; }

    public static WindowId WindowId { get; private set; }

    public static AppWindow AppWindow { get; private set; } = null!;

    public static string Title { get; set; } = "";

    public static string IconPath { get; set; } = "";

    public static AppWindowTitleBar AppTitleBar { get; private set; } = null!;

    public static FrameworkElement TitleBar { get; set; } = null!;

    public static TextBlock TitleTextBlock { get; set; } = null!;
}
