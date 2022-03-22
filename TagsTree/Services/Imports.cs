using PInvoke;
using System;
using System.Runtime.InteropServices;

namespace TagsTree.Services;

public static class Imports
{

    [ComImport]
    [Guid("45D64A29-A63E-4CB6-B498-5781D298CB4F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICoreWindowInterop
    {
        IntPtr WindowHandle { get; }
        bool MessageHandled { get; }
    }

    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow { void Initialize([In] IntPtr hWnd); }


    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern unsafe IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, User32.WindowMessage msg, void* wParam, void* lParam);
}