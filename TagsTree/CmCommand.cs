using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TagsTree
{
	public static class CmCommand
	{
		public static RoutedCommand NewCommand { get; } = new();
		public static RoutedCommand NewXCommand { get; } = new();
		public static RoutedCommand CutCommand { get; } = new();
		public static RoutedCommand PasteCommand { get; } = new();
		public static RoutedCommand PasteXCommand { get; } = new();
		public static RoutedCommand RenameCommand { get; } = new();
		public static RoutedCommand DeleteCommand { get; } = new();
	}
}
