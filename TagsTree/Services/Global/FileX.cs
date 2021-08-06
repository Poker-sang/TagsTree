using System.Diagnostics;
using System.IO;

namespace TagsTree
{
	public partial class App
	{
		public static class FileX
		{
			public static void Open(string fileName)
			{
				try
				{
					var process = new Process { StartInfo = new ProcessStartInfo(fileName) };
					process.StartInfo.UseShellExecute = true;
					process.Start();
				}
				catch (System.ComponentModel.Win32Exception)
				{
					MessageBoxX.Error("找不到文件夹，源文件可能已被更改");
				}
			}
			public static string GetInvalidNameChars => new string(Path.GetInvalidPathChars()) + @"/:*?""<>|";
			public static string GetInvalidPathChars => new string(Path.GetInvalidPathChars()) + @"\/:*?""<>|";
		}
	}
}
