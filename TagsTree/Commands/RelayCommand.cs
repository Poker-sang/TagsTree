using System;

namespace TagsTree.Commands;

public class RelayCommand : System.Windows.Input.ICommand
{
    public RelayCommand(Func<object?, bool> canExecute, Action<object?> execute)
    {
        _canExecuteFunc = canExecute;
        _executeAction = execute;
    }
    public RelayCommand(Action<object?> execute)
    {
        _canExecuteFunc = _ => true;
        _executeAction = execute;
    }

    private readonly Func<object?, bool> _canExecuteFunc;
    private readonly Action<object?> _executeAction;

    public bool CanExecute(object? parameter) => _canExecuteFunc(parameter);
    public void Execute(object? parameter) => _executeAction(parameter);
    public event EventHandler? CanExecuteChanged;

    public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}