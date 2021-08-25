using System.IO;

namespace TagsTreeWpf.Services
{
	public static class FileX
	{
		public static string GetInvalidNameChars => new string(Path.GetInvalidPathChars()) + @"/:*?""<>|";
		public static string GetInvalidPathChars => new string(Path.GetInvalidPathChars()) + @"\/:*?""<>|";
		public enum InvalidMode
		{
			Name = 0,
			Path = 1
		}
		public static string CountSize(FileInfo file) => " " +
			file.Length switch
			{
				< 1 << 10 => file.Length.ToString("F2") + "Byte",
				< 1 << 20 => ((double)file.Length / (1 << 10)).ToString("F2") + "KB",
				< 1 << 30 => ((double)file.Length / (1 << 20)).ToString("F2") + "MB",
				_ => ((double)file.Length / (1 << 30)).ToString("F2") + "GB"
			};
	}

}
