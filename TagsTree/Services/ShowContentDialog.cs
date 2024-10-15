using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace TagsTree.Services;

public static class ShowContentDialog
{
    /// <summary>
    /// 显示一条错误信息
    /// </summary>
    /// <param name="mode"><see langword="true"/>：错误，<see langword="false"/>：提示</param>
    /// <param name="control">错误信息</param>
    public static async Task Information(bool mode, object control)
    {
        var messageDialog = new ContentDialog
        {
            Title = mode ? "错误" : "提示",
            Content = control,
            CloseButtonText = "确定",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = App.MainWindow.Content.XamlRoot
        };
        _ = await messageDialog.ShowAsync();
    }

    /// <inheritdoc cref="Warning(object)"/>
    /// <param name="message">警告（错误）信息</param>
    /// <param name="okHint">选择确认结果（可选）</param>
    /// <param name="cancelHint">选择取消结果（可选）</param>
    public static async Task<bool> Warning(string message, string okHint = "", string cancelHint = "")
    {
        var ok = okHint is "" ? "" : $"\n按“确认”{okHint}";
        var cancel = okHint is "" ? "" : $"\n按“取消”{cancelHint}";
        return await Warning(control: message + "\n" + ok + cancel);
    }

    /// <summary>
    /// 显示一条双项选择的警告（错误）信息
    /// </summary>
    /// <param name="control">警告（错误）信息</param>
    /// <returns>选择确定则返回<see langword="true"/>，取消返回<see langword="false"/></returns>
    public static async Task<bool> Warning(object control)
    {
        var result = false;
        var messageDialog = new ContentDialog
        {
            Title = "警告",
            Content = control,
            PrimaryButtonText = "确定",
            CloseButtonText = "取消",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = App.MainWindow.Content.XamlRoot
        };
        messageDialog.PrimaryButtonClick += (_, _) => result = true;
        messageDialog.CloseButtonClick += (_, _) => result = false;
        _ = await messageDialog.ShowAsync();
        return result;
    }

    /// <inheritdoc cref="Question(object)"/>
    /// <param name="message">询问信息</param>
    /// <param name="yesHint">选择是结果（可选）</param>
    /// <param name="noHint">选择否结果（可选）</param>
    /// <param name="cancelHint">选择取消结果（可选）</param>
    public static async Task<bool?> Question(string message, string yesHint = "", string noHint = "", string cancelHint = "")
    {
        var yes = yesHint is "" ? "" : $"\n按“是”{yesHint}";
        var no = noHint is "" ? "" : $"\n按“否”{noHint}";
        var cancel = cancelHint is "" ? "" : $"\n按“取消”{cancelHint}";
        return await Question(control: message + "\n" + yes + no + cancel);
    }

    /// <summary>
    /// 显示一条三项选择的询问信息
    /// </summary>
    /// <param name="control">询问信息</param>
    /// <returns>选择是则返回<see langword="true"/>，否返回<see langword="false"/>，取消返回<see langword="null"/></returns>
    public static async Task<bool?> Question(object control)
    {
        bool? result = null;
        var messageDialog = new ContentDialog
        {
            Title = "提示",
            Content = control,
            PrimaryButtonText = "是",
            SecondaryButtonText = "否",
            CloseButtonText = "取消",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = App.MainWindow.Content.XamlRoot
        };
        messageDialog.PrimaryButtonClick += (_, _) => result = true;
        messageDialog.SecondaryButtonClick += (_, _) => result = false;
        messageDialog.CloseButtonClick += (_, _) => result = null;
        _ = await messageDialog.ShowAsync();
        return result;
    }
}
