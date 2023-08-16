using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Im_Analyzer.Common
{
    internal class _DelegateCommand : ICommand
    {
        private readonly Action _Command;         // コマンド本体
        private readonly Func<bool> _CanExecute; // 実行可否
        public _DelegateCommand(Action command, Func<bool> canExecute = null)
        {
            _Command = command;
            _CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _CanExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _Command?.Invoke();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

    }
}
