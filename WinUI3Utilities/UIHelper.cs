using PInvoke;

namespace WinUI3Utilities;

public class UIHelper
{
    /// <summary>
    /// Get the dpi-aware screen size using win32 API, where by "dpi-aware" means that
    /// the result will be divided by the scale factor of the monitor that hosts the app
    /// </summary>
    /// <returns>Screen size</returns>
    public static (int, int) GetScreenSize()
        => (User32.GetSystemMetrics(User32.SystemMetric.SM_CXSCREEN), User32.GetSystemMetrics(User32.SystemMetric.SM_CYSCREEN));
}
