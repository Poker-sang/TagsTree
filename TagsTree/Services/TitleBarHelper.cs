using PInvoke;
using TagsTree.Services.ExtensionMethods;

namespace TagsTree.Services;

internal class TitleBarHelper
{
    public static void TriggerTitleBarRepaint()
    {
        // to trigger repaint tracking task id 38044406
        if (WindowHelper.HWnd == User32.GetActiveWindow())
        {
            _ = User32.SendMessage(WindowHelper.HWnd, User32.WindowMessage.WM_ACTIVATE, WA_INACTIVE, 0);
            _ = User32.SendMessage(WindowHelper.HWnd, User32.WindowMessage.WM_ACTIVATE, WA_ACTIVE, 0);
        }
        else
        {
            _ = User32.SendMessage(WindowHelper.HWnd, User32.WindowMessage.WM_ACTIVATE, WA_ACTIVE, 0);
            _ = User32.SendMessage(WindowHelper.HWnd, User32.WindowMessage.WM_ACTIVATE, WA_INACTIVE, 0);
        }
    }
    // ReSharper disable InconsistentNaming
    public static readonly nint WA_INACTIVE = 0x00;
    public static readonly nint WA_ACTIVE = 0x01;
    public static readonly nint WA_CLICKACTIVE = 0x02;
    // ReSharper restore InconsistentNaming
}
