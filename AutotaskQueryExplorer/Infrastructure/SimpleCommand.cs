using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutotaskQueryExplorer.Infrastructure
{
    public class SimpleCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public SimpleCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _canExecute = canExecute ?? (() => true);
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
