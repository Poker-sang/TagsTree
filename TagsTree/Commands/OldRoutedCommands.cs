using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TagsTree.Services;

namespace TagsTree.Commands
{
	class OldRoutedCommands
	{/*
		public static RoutedCommand NewCommand { get; } = new();
		public static RoutedCommand NewXCommand { get; } = new();
		public static RoutedCommand CutCommand { get; } = new();
		public static RoutedCommand PasteCommand { get; } = new();
		public static RoutedCommand PasteXCommand { get; } = new();
		public static RoutedCommand RenameCommand { get; } = new();
		public static RoutedCommand DeleteCommand { get; } = new();
		

		<Window.CommandBindings>
			<CommandBinding Command = "{x:Static commands:TagsManagerCommand.NewCommand}" CanExecute="True_CanExecute"  Executed="New_Execute"/>
			<CommandBinding Command = "{x:Static commands:TagsManagerCommand.NewXCommand}"    CanExecute="True_CanExecute"  Executed="NewX_Execute"/>
			<CommandBinding Command = "{x:Static commands:TagsManagerCommand.CutCommand}" CanExecute="True_CanExecute"  Executed="Cut_Execute"/>
			<CommandBinding Command = "{x:Static commands:TagsManagerCommand.PasteCommand}"   CanExecute="Paste_CanExecute" Executed="Paste_Execute"/>
			<CommandBinding Command = "{x:Static commands:TagsManagerCommand.PasteXCommand}"  CanExecute="Paste_CanExecute" Executed="PasteX_Execute"/>
			<CommandBinding Command = "{x:Static commands:TagsManagerCommand.RenameCommand}"  CanExecute="True_CanExecute"  Executed="Rename_Execute"/>
			<CommandBinding Command = "{x:Static commands:TagsManagerCommand.DeleteCommand}"  CanExecute="True_CanExecute"  Executed="Delete_Execute"/>
		</Window.CommandBindings>
		#region 命令

		private void True_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
		private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ClipBoard is not null;

		private XmlElement? ClipBoard { get; set; }

		private void New_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			Service.NewTag(dialog.Message, TagsTreeStatic.TvItemGetHeader(e.Parameter)!);
		}
		private void NewX_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			Service.NewTag(dialog.Message, TagsTreeStatic.XdpRoot!);
		}
		private void Cut_Execute(object sender, ExecutedRoutedEventArgs e) => ClipBoard = TagsTreeStatic.TvItemGetHeader(e.Parameter);
		private void Paste_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			Service.MoveTag(ClipBoard!, TagsTreeStatic.TvItemGetHeader(e.Parameter));
			ClipBoard = null;
		}
		private void PasteX_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			Service.MoveTag(ClipBoard!, TagsTreeStatic.XdpRoot!);
			ClipBoard = null;
		}
		private void Rename_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new InputName();
			if (dialog.ShowDialog() == false || !TagsTreeStatic.NewTagCheck(dialog.Message))
				return;

			Service.RenameTag(dialog.Message, TagsTreeStatic.TvItemGetHeader(e.Parameter)!);
		}
		private void Delete_Execute(object sender, ExecutedRoutedEventArgs e) => Service.DeleteTag(TagsTreeStatic.TvItemGetHeader(e.Parameter));

		#endregion
		*/
	}
}
