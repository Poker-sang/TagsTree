using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace TagsTree
{
	public class CIconOfPath
	{
		private struct Rect
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		private struct Point
		{
			public int x;
			public int y;
		}
		// Constants that we need in the function call
		[Flags]
		private enum IconSize : uint
		{
			ShGfiIcon = 0x100,
			ShGfiSmallIcon = 0x1,
			ShGfiLargeIcon = 0x0,
			ShGfiPidl = 0x8,
			ShGfiUseFileAttributes = 0x10,
			ShGfiSysIconIndex = 0x4000,
			ShGfiLinkOverlay = 0x8000,

			ShiLJumbo = 0x4,
			ShiLExtraLarge = 0x2
		}



		// This structure will contain information about the file

		public struct ShFileInfo
		{
			// Handle to the icon representing the file
			public IntPtr HIcon;

			// Index of the icon within the image list
			public int IIcon;

			// Various attributes of the file
			public uint DwAttributes;

			// Path to the file
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string SzDisplayName;

			// File type
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] public string SzTypeName;

			public ShFileInfo(int iIcon, IntPtr hIcon, string szDisplayName, string szTypeName, uint dwAttributes)
			{
				IIcon = iIcon;
				HIcon = hIcon;
				SzDisplayName = szDisplayName;
				SzTypeName = szTypeName;
				DwAttributes = dwAttributes;
			}
		};

		[DllImport("Kernel32.dll")]
		private static extern bool CloseHandle(IntPtr handle);

		private struct ImageListDrawParams
		{
			public int cbSize;
			public IntPtr himl;
			public int i;
			public IntPtr hdcDst;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public int xBitmap; // x offset from the upper left of bitmap
			public int yBitmap; // y offset from the upper left of bitmap
			public int rgbBk;
			public int rgbFg;
			public int fStyle;
			public int dwRop;
			public int fState;
			public int Frame;
			public int crEffect;
		}

		[StructLayout(LayoutKind.Sequential)]
		private readonly struct ImageInfo
		{
			public readonly IntPtr hbmImage;
			public readonly IntPtr hbmMask;
			public readonly int Unused1;
			public readonly int Unused2;
			public readonly Rect rcImage;
		}

		#region Private ImageList COM Interop (XP)

		[ComImport, Guid("46EB5926-582E-4017-9FDF-E8998DAA0950"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] //helpString("Image List"),
		private interface IImageList
		{
			[PreserveSig] int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);
			[PreserveSig] int ReplaceIcon(int i, IntPtr hIcon, ref int pi);
			[PreserveSig] int SetOverlayImage(int iImage, int iOverlay);
			[PreserveSig] int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);
			[PreserveSig] int AddMasked(IntPtr hbmImage, int crMask, ref int pi);
			[PreserveSig] int Draw(ref ImageListDrawParams imageListDrawParams);
			[PreserveSig] int Remove(int i);
			[PreserveSig] int GetIcon(int i, int flags, ref IntPtr pIcon);
			[PreserveSig] int GetImageInfo(int i, ref ImageInfo pImageInfo);
			[PreserveSig] int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);
			[PreserveSig] int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riId, ref IntPtr ppv);
			[PreserveSig] int Clone(ref Guid riId, ref IntPtr ppv);
			[PreserveSig] int GetImageRect(int i, ref Rect prc);
			[PreserveSig] int GetIconSize(ref int cx, ref int cy);
			[PreserveSig] int SetIconSize(int cx, int cy);
			[PreserveSig] int GetImageCount(ref int pi);
			[PreserveSig] int SetImageCount(int uNewCount);
			[PreserveSig] int SetBkColor(int clrBk, ref int pclr);
			[PreserveSig] int GetBkColor(ref int pclr);
			[PreserveSig] int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);
			[PreserveSig] int EndDrag();
			[PreserveSig] int DragEnter(IntPtr hwndLock, int x, int y);
			[PreserveSig] int DragLeave(IntPtr hwndLock);
			[PreserveSig] int DragMove(int x, int y);
			[PreserveSig] int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);
			[PreserveSig] int DragShowNoLock(int fShow);
			[PreserveSig] int GetDragImage(ref Point ppt, ref Point pptHotspot, ref Guid riid, ref IntPtr ppv);
			[PreserveSig] int GetItemFlags(int i, ref int dwFlags);
			[PreserveSig] int GetOverlayImage(int iOverlay, ref int piIndex);
		};
		#endregion

		/// SHGetImageList is not exported correctly in XP.  See KB316931
		/// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
		/// Apparently (and hopefully) ordinal 727 isn't going to change.
		[DllImport("shell32.dll", EntryPoint = "#727")] private static extern int SHGetImageList(int iImageList, ref Guid riId, out IImageList ppv);
		// The signature of SHGetFileInfo (located in Shell32.dll)
		[DllImport("Shell32.dll")] private static extern int SHGetFileInfo(string pszPath, int dwFileAttributes, ref ShFileInfo psfi, int cbFileInfo, uint uFlags);
		[DllImport("Shell32.dll")] private static extern int SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes, ref ShFileInfo psfi, int cbFileInfo, uint uFlags);
		[DllImport("shell32.dll", SetLastError = true)] private static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, int nFolder, ref IntPtr ppidl);
		[DllImport("user32")] private static extern int DestroyIcon(IntPtr hIcon);

		public struct Pair
		{
			public Icon Icon { get; set; }
			public IntPtr IconHandleToDestroy { set; get; }
		}

		public static int DestroyIcon2(IntPtr hIcon) => DestroyIcon(hIcon);

		public static BitmapSource BitmapSourceOfIconIcon(Icon ic)
		{
			var ic2 = Imaging.CreateBitmapSourceFromHIcon(ic.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			ic2.Freeze();
			return ic2;
		}
		/*
		public static BitmapSource SystemIcon(bool small, ShellLib.ShellApi.CSIDL csidl)
		{
			var pidlTrash = IntPtr.Zero;
			var hr = SHGetSpecialFolderLocation(IntPtr.Zero, (int)csidl, ref pidlTrash);
			Debug.Assert(hr == 0);
			// Get a handle to the large icon
			var flags = (uint)(IconSize.ShGfiPidl | IconSize.ShGfiIcon | IconSize.ShGfiUseFileAttributes | (!small ? IconSize.ShGfiLargeIcon : IconSize.ShGfiSmallIcon));

			var shInfo = new ShFileInfo();
			var res = SHGetFileInfo(pidlTrash, 0, ref shInfo, Marshal.SizeOf(shInfo), flags);
			Debug.Assert(res != 0);

			var myIcon = Icon.FromHandle(shInfo.HIcon);
			Marshal.FreeCoTaskMem(pidlTrash);
			var bs = BitmapSourceOfIconIcon(myIcon);
			myIcon.Dispose();
			bs.Freeze(); // very important to avoid memory leak
			_ = DestroyIcon(shInfo.HIcon);
			_ = CloseHandle(shInfo.HIcon);
			return bs;

		}
		*/
		public static BitmapSource IconOfPath(string fileName, bool small, bool checkDisk, bool addOverlay)
		{
			var shInfo = new ShFileInfo();
			
			var flags = IconSize.ShGfiIcon | (small ? IconSize.ShGfiSmallIcon : IconSize.ShGfiLargeIcon);

			if (!checkDisk)
				flags |= IconSize.ShGfiUseFileAttributes;
			if (addOverlay)
				flags |= IconSize.ShGfiLinkOverlay;

			var res = SHGetFileInfo(fileName, 0, ref shInfo, Marshal.SizeOf(shInfo), (uint)flags);
			if (res == 0)
				throw new FileNotFoundException();

			var myIcon = Icon.FromHandle(shInfo.HIcon);

			var bs = BitmapSourceOfIconIcon(myIcon);
			myIcon.Dispose();
			bs.Freeze(); // very important to avoid memory leak
			_ = DestroyIcon(shInfo.HIcon);
			//_ = CloseHandle(shInfo.HIcon);
			return bs;
		}

		public static BitmapSource IconOfPathLarge(string fileName, bool jumbo, bool checkDisk)
		{
			const int fileAttributeNormal = 0x80;

			var flags = IconSize.ShGfiSysIconIndex;

			if (!checkDisk) // This does not seem to work. If I try it, a folder icon is always returned.
				flags |= IconSize.ShGfiUseFileAttributes;

			var shInfo = new ShFileInfo();
			if (SHGetFileInfo(fileName, fileAttributeNormal, ref shInfo, Marshal.SizeOf(shInfo), (uint)flags) == 0)
				throw new FileNotFoundException();
			var iconIndex = shInfo.IIcon;

			// Get the System IImageList object from the Shell:
			var iIdImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");

			var size = (int)(jumbo ? IconSize.ShiLJumbo : IconSize.ShiLExtraLarge);
			_ = SHGetImageList(size, ref iIdImageList, out IImageList iml); // writes iml
			//if ( == 0)
			//	throw new Exception("Error SHGetImageList");

			var hIcon = IntPtr.Zero;
			const int ildTransparent = 1;
			_ = iml.GetIcon(iconIndex, ildTransparent, ref hIcon);
			//if ( == 0)
			//	throw new Exception("Error iml.GetIcon");

			var myIcon = Icon.FromHandle(hIcon);
			var bs = BitmapSourceOfIconIcon(myIcon);
			myIcon.Dispose();
			bs.Freeze(); // very important to avoid memory leak
			_ = DestroyIcon(hIcon);
			//_ = CloseHandle(hIcon);
			return bs;
		}
	}
}