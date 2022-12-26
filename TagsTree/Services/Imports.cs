using System;
using System.Runtime.InteropServices;

namespace TagsTree.Services;

public static partial class Imports
{
    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow { void Initialize([In] nint hWnd); }

    [LibraryImport("dwmapi.dll")]
    public static partial int DwmSetWindowAttribute(nint hWnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cdAttribute);

    [Flags]
    public enum DwmWindowAttribute : uint
    {
        DwmwaUseImmersiveDarkMode = 20,
        DwmwaMicaEffect = 1029
    }
}
