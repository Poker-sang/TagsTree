using System;
using System.Windows;
namespace TagsTree
{
	public partial class App
	{
		public static class MessageBoxX
		{
			/// <summary>
			/// 显示一条错误信息
			/// </summary>
			/// <param name="message">错误信息</param>
			public static void Error(string message) => _ = MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);

			/// <summary>
			/// 显示一条可选择的警告信息
			/// </summary>
			/// <param name="message">警告信息</param>
			/// <param name="okHint">选择确认结果（可选）</param>
			/// <param name="cancelHint">选择取消结果（可选）</param>
			/// <returns>选择确定则返回true</returns>
			public static bool Warning(string message, string okHint = "", string cancelHint = "")
			{
				var ok = okHint is "" ? "" : $"\n按“确认”{okHint}";
				var cancel = okHint is "" ? "" : $"\n按“取消”{cancelHint}";
				return MessageBox.Show(message + ok + cancel, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK;
			}

			/// <summary>
			/// 显示一条信息
			/// </summary>
			/// <param name="message">信息</param>
			public static void Information(string message) => _ = MessageBox.Show(message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);

			/// <summary>
			/// 三项选择
			/// </summary>
			/// <param name="message">询问信息</param>
			/// <param name="yesHint">选择是结果（可选）</param>
			/// <param name="noHint">选择否结果（可选）</param>
			/// <param name="cancelHint">选择取消结果（可选）</param>
			public static bool? Question(string message, string yesHint = "", string noHint = "", string cancelHint = "")
			{
				{
					var yes = yesHint is "" ? "" : $"\n按“是”{yesHint}";
					var no = yesHint is "" ? "" : $"\n按“是”{noHint}";
					var cancel = cancelHint is "" ? "" : $"\n按“取消”{cancelHint}";
					return MessageBox.Show(message + yes + no + cancel, "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) switch
					{
						MessageBoxResult.Yes => true,
						MessageBoxResult.No => false,
						MessageBoxResult.Cancel => null,
						_ => null
					};
				}
			}
		}
	}
}