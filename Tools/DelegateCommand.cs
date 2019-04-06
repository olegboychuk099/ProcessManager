using System;
using System.Windows.Input;

namespace ProcessManager.Tools
{
    public class DelegateCommand : ICommand
    {
        readonly Predicate<object> canExecute;
        readonly Action<object> execute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public DelegateCommand(Action<object> execute)
            : this(execute, _ => true)
        {
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
