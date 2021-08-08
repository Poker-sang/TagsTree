using System.IO;

namespace TagsTree
{
	public partial class App
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
		}
	}
}
