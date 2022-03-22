using Microsoft.UI;
using Microsoft.UI.Windowing;
using PInvoke;
using System;
using WinRT;
using WinRT.Interop;

namespace TagsTree.Services.ExtensionMethods;

public static class WindowHelper
{
    public static MainWindow Window { get; private set; } = null!;
    public static IntPtr HWnd { get; private set; } = IntPtr.Zero;
    public static AppWindow AppWindow { get; private set; } = null!;

    public static MainWindow Initialize()
    {
        Window = new MainWindow();
        //等效于 HWnd = PInvoke.User32.GetActiveWindow();
        HWnd = WindowNative.GetWindowHandle(Window);
        AppWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(HWnd));
        return Window;
    }

    public static MainWindow SetWindowSize(this MainWindow mainWindow, int width, int height)
    {
        // Win32 uses pixels and WinUI 3 uses effective pixels, so you should apply the DPI scale factor
        var dpi = User32.GetDpiForWindow(HWnd);
        var scalingFactor = (float)dpi / 96;
        width = (int)(width * scalingFactor);
        height = (int)(height * scalingFactor);
        _ = User32.SetWindowPos(HWnd, User32.SpecialWindowHandles.HWND_TOP, 0, 0, width, height, User32.SetWindowPosFlags.SWP_NOMOVE);
        return mainWindow;
    }

    public static void Activate(this MainWindow mainWindow) => mainWindow.Activate();

    public static void Maximize() => User32.ShowWindow(HWnd, User32.WindowShowStyle.SW_MAXIMIZE);
    public static void Minimize() => User32.ShowWindow(HWnd, User32.WindowShowStyle.SW_MINIMIZE);
    public static void Restore() => User32.ShowWindow(HWnd, User32.WindowShowStyle.SW_RESTORE);

    public static T InitializeWithWindow<T>(this T obj)
    {
        // When running on win32, FileOpenPicker needs to know the top-level hWnd via IInitializeWithWindow::Initialize.
        if (Microsoft.UI.Xaml.Window.Current is null)
            obj.As<Imports.IInitializeWithWindow>()?.Initialize(HWnd); //HWnd 或者 User32.GetActiveWindow()
        return obj;
    }
}