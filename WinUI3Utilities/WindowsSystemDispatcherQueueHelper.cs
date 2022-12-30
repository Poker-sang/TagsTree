using System.Runtime.InteropServices;

namespace WinUI3Utilities;

internal class WindowsSystemDispatcherQueueHelper
{
    [StructLayout(LayoutKind.Sequential)]
    private struct DispatcherQueueOptions
    {
        internal int dwSize;
        internal int threadType;
        internal int apartmentType;
    }

    [DllImport("CoreMessaging.dll")]
    private static extern int CreateDispatcherQueueController(DispatcherQueueOptions options, [MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

    private object? _dispatcherQueueController;

    public void EnsureWindowsSystemDispatcherQueueController()
    {
        // one already exists, so we'll just use it.
        if (Windows.System.DispatcherQueue.GetForCurrentThread() is not null)
            return;

        if (_dispatcherQueueController is null)
        {
            DispatcherQueueOptions options;
            options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
            options.threadType = 2;
            options.apartmentType = 2;

            _ = CreateDispatcherQueueController(options, ref _dispatcherQueueController!);
        }
    }
}
