using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using WinRT;

namespace TagsTreeWinUI3.Services.ExtensionMethods
{
	public static class ActiveWindow
	{

		[ComImport, Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IInitializeWithWindow
		{
			void Initialize([In] IntPtr hwnd);
		}

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
		private static extern IntPtr GetActiveWindow();

		public static T InitializeWithWindow<T>(this T obj)
		{
			// When running on win32, FileOpenPicker needs to know the top-level hwnd via IInitializeWithWindow::Initialize.
			if (Window.Current is null)
				obj.As<IInitializeWithWindow>().Initialize(GetActiveWindow());
			return obj;
		}
	}
}
