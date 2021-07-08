using System;
using System.Windows.Input;

namespace TagsTree.Commands
{
	public static class TagsManagerCommand
	{
		public static RoutedCommand NewCommand { get; } = new();
		public static RoutedCommand NewXCommand { get; } = new();
		public static RoutedCommand CutCommand { get; } = new();
		public static RoutedCommand PasteCommand { get; } = new();
		public static RoutedCommand PasteXCommand { get; } = new();
		public static RoutedCommand RenameCommand { get; } = new();
		public static RoutedCommand DeleteCommand { get; } = new();
	}

	public class TagsManagerButtonCommand : ICommand
	{
		public TagsManagerButtonCommand(Func<object?, bool> canExecute, Action<object?> execute)
		{
			_canExecuteFunc = canExecute;
			_executeAction = execute;
		}

		private readonly Func<object?, bool> _canExecuteFunc;
		private readonly Action<object?> _executeAction;

		public bool CanExecute(object? parameter) => _canExecuteFunc(parameter);
		public void Execute(object? parameter) => _executeAction(parameter);
		public event EventHandler? CanExecuteChanged;
	}
}
