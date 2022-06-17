using System;
using System.Runtime.InteropServices;

namespace TagsTree.Services;

public static class Imports
{
    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow { void Initialize([In] nint hWnd); }

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(nint hWnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cdAttribute);

    [Flags]
    public enum DwmWindowAttribute : uint
    {
        DwmwaUseImmersiveDarkMode = 20,
        DwmwaMicaEffect = 1029
    }

    // [DllImport("user32.dll", CharSet = CharSet.Auto)]
    // public static extern unsafe nint CallWindowProc(nint lpPrevWndFunc, nint hWnd, User32.WindowMessage msg, void* wParam, void* lParam);

    // [ComImport]
    // [Guid("45D64A29-A63E-4CB6-B498-5781D298CB4F")]
    // [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    // public interface ICoreWindowInterop
    // {
    //     nint WindowHandle { get; }
    //     bool MessageHandled { get; }
    // }
}
