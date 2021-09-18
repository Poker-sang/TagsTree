using Microsoft.UI.Xaml;
using PInvoke;
using System;
using System.Runtime.InteropServices;
using WinRT;

namespace TagsTree.Services.ExtensionMethods
{
    public static class WindowHelper
    {
        private static IntPtr HWnd => WinRT.Interop.WindowNative.GetWindowHandle(App.Window);

        public static T InitializeWithWindow<T>(this T obj)
        {
            // When running on win32, FileOpenPicker needs to know the top-level hWnd via IInitializeWithWindow::Initialize.
            if (Window.Current is null)
                obj.As<IInitializeWithWindow>()?.Initialize(HWnd); //HWnd 或者 User32.GetActiveWindow()
            return obj;
        }

        public static void SetWindowSize(int width, int height)
        {
            // Win32 uses pixels and WinUI 3 uses effective pixels, so you should apply the DPI scale factor
            var dpi = User32.GetDpiForWindow(HWnd);
            var scalingFactor = (float)dpi / 96;
            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);
            _ = User32.SetWindowPos(HWnd, User32.SpecialWindowHandles.HWND_TOP, 0, 0, width, height, User32.SetWindowPosFlags.SWP_NOMOVE);
        }

        [ComImport, Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IInitializeWithWindow { void Initialize([In] IntPtr hWnd); }
    }
}