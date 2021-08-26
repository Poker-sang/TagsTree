using System;

namespace TagsTreeWpf.Services.ExtensionMethods
{
	public static class PageUri
	{
		public static Uri Get(string page) => new($"pack://application:,,,/views/{page}.xaml");
	}
}
