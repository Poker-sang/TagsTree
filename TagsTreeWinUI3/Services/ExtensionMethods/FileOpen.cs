using System.Diagnostics;

namespace TagsTreeWinUI3.Services.ExtensionMethods
{
	public static class FileOpen
	{
		public static void Open(this string fileName)
		{
			try
			{
				var process = new Process { StartInfo = new ProcessStartInfo(fileName) };
				process.StartInfo.UseShellExecute = true;
				process.Start();
			}
			catch (System.ComponentModel.Win32Exception)
			{
				MessageDialogX.Information(true, "找不到文件夹，源文件可能已被更改");
			}
		}
	}
}
