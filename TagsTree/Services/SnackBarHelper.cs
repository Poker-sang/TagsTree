using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using WinUI3Utilities;

namespace TagsTree.Services;

public static class SnackBarHelper
{
    public static TeachingTip SnackBar { get; set; } = null!;

    private static DateTime _closeSnakeBarTime;

    /// <summary>
    /// 出现信息并消失
    /// </summary>
    /// <param name="message">提示信息</param>
    /// <param name="hint">信息附加内容</param>
    /// <param name="severity">严重性</param>
    /// <param name="mSec">信息持续时间</param>
    public static async void Show(string message, Severity severity = Severity.Ok, string hint = "", int mSec = 3000)
    {
        _closeSnakeBarTime = DateTime.Now + TimeSpan.FromMicroseconds(mSec - 100);

        SnackBar.Title = message;
        SnackBar.Subtitle = hint;
        SnackBar.IconSource = new FontIconSource
        {
            Glyph = severity switch
            {
                Severity.Ok => "\xE10B", // Accept
                Severity.Information => "\xE946", // Info
                Severity.Important => "\xE171", // Important
                Severity.Warning => "\xE7BA", // Warning
                Severity.Error => "\xEA39", // ErrorBadge
                _ => ThrowHelper.ArgumentOutOfRange<Severity, string>(severity)
            }
        };

        SnackBar.IsOpen = true;

        await Task.Delay(mSec);

        if (DateTime.Now > _closeSnakeBarTime)
            SnackBar.IsOpen = false;
    }
}

public enum Severity
{
    Ok,
    Information,
    Important,
    Warning,
    Error
}
