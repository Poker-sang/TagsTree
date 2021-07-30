using System.Windows;

namespace TagsTree
{
	public partial class App
	{
		public static class MessageBox
		{
			/// <summary>
			/// 显示一条错误信息
			/// </summary>
			/// <param name="message">错误信息</param>
			public static void ErrorMessageBox(string message) => _ = System.Windows.MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);

			/// <summary>
			/// 显示一条可选择的警告信息
			/// </summary>
			/// <param name="message">警告信息</param>
			/// <param name="okHint">选择确认结果</param>
			/// <param name="cancelHint">选择取消结果</param>
			/// <returns>只可能有Ok或Cancel两种结果（直接关闭结果为Cancel）</returns>
			public static MessageBoxResult WarningMessageBox(string message, string okHint = "", string cancelHint = "")
			{
				var ok = okHint is "" ? "" : $"\n按“确认”{okHint}";
				var cancel = okHint is "" ? "" : $"\n按“取消”{cancelHint}";
				return System.Windows.MessageBox.Show(message + ok + cancel, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
			}

			/// <summary>
			/// 显示一条信息
			/// </summary>
			/// <param name="message">信息</param>
			public static void InformationMessageBox(string message) => _ = System.Windows.MessageBox.Show(message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}