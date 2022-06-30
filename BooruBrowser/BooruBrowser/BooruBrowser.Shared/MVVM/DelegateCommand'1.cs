using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BooruBrowser.MVVM
{
    /// <summary>
    /// Delegate (or Relay) command for WPF MVVM commanding.
    /// Command action must have one command argument of type T.
    /// </summary>
    /// <typeparam name="T">Type of command action argument.</typeparam>
    public class DelegateCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute;
        readonly Predicate<T> _canExecute;
        readonly Func<T> _argFunc;

        #endregion // Fields

        #region Constructors

        public DelegateCommand(Action<T> execute, Func<T> argFunc = null, Predicate<T> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
            _argFunc = argFunc;
        }
        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        bool ICommand.CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(_argFunc != null ? _argFunc() : (T)parameter);
        }

        public event EventHandler CanExecuteChanged;

        void ICommand.Execute(object parameter)
        {
            if (_argFunc != null)
                _execute(_argFunc());
            else
                _execute((T)parameter);
        }

        #endregion // ICommand Members

        public void Execute(T parameter)
        {
            (this as ICommand).Execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
