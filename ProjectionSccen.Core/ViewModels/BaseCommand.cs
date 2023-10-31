using System;
using System.Windows.Input;
namespace ProjectionSccen.Core.ViewModels;
/// <summary>
/// 
/// </summary>
public class BaseCommand:ICommand
{
    /// <summary>
    /// 
    /// </summary>
    public Action<object> ExecuteCommand = null;
    /// <summary>
    /// 
    /// </summary>
    public Func<object, bool> CanExecuteCommand = null;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    public BaseCommand(Action<object> action = null)
    {
        ExecuteCommand = action;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object? parameter)
    {
        return CanExecuteCommand == null ? true : this.CanExecuteCommand.Invoke(parameter);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameter"></param>
    public void Execute(object? parameter)
    {
        this.ExecuteCommand?.Invoke(parameter);
    }

    public event EventHandler? CanExecuteChanged;
}

public class BaseCommand<T> : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="execute"></param>
    /// <param name="canExecute"></param>
    public BaseCommand(Action<object> execute, Predicate<object> canExecute)
    {
        if (execute != null)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="execute"></param>
    public  BaseCommand(Action<object>execute):this(execute,null){}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paramera"></param>
    /// <returns></returns>
    public bool CanExecute(object paramera)
    {
        return _canExecute == null || _canExecute.Invoke(paramera);
    }
    /// <summary>
    /// 
    /// </summary>
    public  event  EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameter"></param>
    public void Execute(object parameter)
    {
        _execute?.Invoke(parameter);
    }
}

 
