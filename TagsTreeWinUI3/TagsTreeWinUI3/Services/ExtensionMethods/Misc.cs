using System;

namespace TagsTreeWinUI3.Services.ExtensionMethods
{
	public static class Misc
	{
		public static T CastThrow<T>(this object? obj) => (T)(obj ?? throw new InvalidCastException());
	}
}