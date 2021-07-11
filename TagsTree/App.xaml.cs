using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace TagsTree
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static Point LastMousePos { get; set; }
		public static bool MouseDisplace(double distance, Point currentPos)
			=> Math.Abs(currentPos.X - LastMousePos.X) > distance ||
			   Math.Abs(currentPos.Y - LastMousePos.Y) > distance;
		/// <summary>
		/// 显示一条错误信息
		/// </summary>
		public static void ErrorMessageBox(string message) => _ = MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
	}
}
