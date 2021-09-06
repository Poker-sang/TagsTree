using System;
using TagsTreeWinUI3.Interfaces;

namespace TagsTreeWinUI3.Services.ExtensionMethods
{
	public static class FullNameHelper
	{
		public static string GetPartialPath(this IFullName fullName) => "..." + fullName.Path.AsSpan()[App.AppConfigurations.LibraryPath.Length..].ToString();
	}
}