namespace WinUI3Utilities;

public static class WindowHelper
{
    public static T InitializeWithWindow<T>(this T obj)
    {
        // When running on win32, FileOpenPicker needs to know the top-level hWnd via IInitializeWithWindow::Initialize.
        WinRT.Interop.InitializeWithWindow.Initialize(obj, CurrentContext.HWnd);
        return obj;
    }
}
