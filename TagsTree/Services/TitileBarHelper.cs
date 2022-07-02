using PInvoke;
using System;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Services;

internal class TitleBarHelper
{
    public static void TriggerTitleBarRepaint()
    {
        // to trigger repaint tracking task id 38044406
        if (WindowHelper.HWnd == User32.GetActiveWindow())
        {
            _ = User32.SendMessage(WindowHelper.HWnd, User32.WindowMessage.WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
            _ = User32.SendMessage(WindowHelper.HWnd, User32.WindowMessage.WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }
        else
        {
            _ = User32.SendMessage(WindowHelper.HWnd, User32.WindowMessage.WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
            _ = User32.SendMessage(WindowHelper.HWnd, User32.WindowMessage.WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }
    }
    // ReSharper disable InconsistentNaming
    public static readonly nint WA_ACTIVE = 0x01;
    public static readonly nint WA_INACTIVE = 0x00;
    // ReSharper restore InconsistentNaming
}
