using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using TagsTreeWinUI3.Services.ExtensionMethods;

namespace TagsTreeWinUI3.Services
{
	public static class MessageDialogX
	{
		/// <summary>
		/// 显示一条错误信息
		/// </summary>
		/// <param name="mode">true：错误，false：提示</param>
		/// <param name="message">错误信息</param>
		public static async void Information(bool mode, string message)
		{
			var messageDialog = new MessageDialog(message, mode ? "错误" : "提示")
			{
				DefaultCommandIndex = 0,
				CancelCommandIndex = 0
			}.InitializeWithWindow();
			messageDialog.Commands.Add(new UICommand("确定", _ => { }));
			_ = await messageDialog.ShowAsync();
		}

		/// <summary>
		/// 显示一条可选择的警告信息
		/// </summary>
		/// <param name="message">警告信息</param>
		/// <param name="okHint">选择确认结果（可选）</param>
		/// <param name="cancelHint">选择取消结果（可选）</param>
		/// <returns>选择确定则返回true</returns>
		public static async Task<bool> Warning(string message, string okHint = "", string cancelHint = "")
		{
			var ok = okHint is "" ? "" : $"\n按“确认”{okHint}";
			var cancel = okHint is "" ? "" : $"\n按“取消”{cancelHint}";
			var result = false; 
			var messageDialog = new MessageDialog(message + ok + cancel, "警告")
			{
				DefaultCommandIndex = 0,
				CancelCommandIndex = 1
			}.InitializeWithWindow();
			messageDialog.Commands.Add(new UICommand("确定", _ => result = true));
			messageDialog.Commands.Add(new UICommand("取消", _ => result = false));
			_ = await messageDialog.ShowAsync();
			return result;
		}

		/// <summary>
		/// 三项选择
		/// </summary>
		/// <param name="message">询问信息</param>
		/// <param name="yesHint">选择是结果（可选）</param>
		/// <param name="noHint">选择否结果（可选）</param>
		/// <param name="cancelHint">选择取消结果（可选）</param>
		public static async Task<bool?> Question(string message, string yesHint = "", string noHint = "", string cancelHint = "")
		{
			var yes = yesHint is "" ? "" : $"\n按“是”{yesHint}";
			var no = yesHint is "" ? "" : $"\n按“否”{noHint}";
			var cancel = cancelHint is "" ? "" : $"\n按“取消”{cancelHint}";
			bool? result = null;
			var messageDialog = new MessageDialog(message + yes + no + cancel, "提示")
			{
				DefaultCommandIndex = 0,
				CancelCommandIndex = 1
			}.InitializeWithWindow();
			messageDialog.Commands.Add(new UICommand("是", _ => result = true));
			messageDialog.Commands.Add(new UICommand("否", _ => result = false));
			messageDialog.Commands.Add(new UICommand("取消", _ => result = null));
			_ = await messageDialog.ShowAsync();
			return result;
		}
	}
}