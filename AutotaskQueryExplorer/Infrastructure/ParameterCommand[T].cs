using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutotaskQueryExplorer.Infrastructure
{
    public class ParameterCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;

        public ParameterCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _canExecute = canExecute ?? ((t) => true);
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            ValidateParameter(parameter);
            return _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            ValidateParameter(parameter);
            _execute((T)parameter);
        }

        private static void ValidateParameter(object parameter)
        {
            if (!(parameter is T || parameter == null))
                throw new InvalidOperationException("Wrong parameter type for this command");
        }
    }
}
