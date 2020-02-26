using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Reader
{
    public class Commands
    {
        static Commands()
        {
            RefreshFeed = new RoutedUICommand("Refresh feed", "RefreshFeed", typeof(Commands));
        }

        public static RoutedUICommand RefreshFeed
        {
            get;
            private set;
        }
    }

    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Func<bool> _canExecuteWithoutParam;

        protected readonly Action _action;
        protected readonly Action<object> _parameterizedAction;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public DelegateCommand(Action<object> action)
        {
            _parameterizedAction = action;
        }

        public DelegateCommand(Action action, Predicate<object> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecuteWithoutParam = canExecute;
        }

        public DelegateCommand(Action<object> action, Predicate<object> canExecute)
        {
            _parameterizedAction = action;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action<object> action, Func<bool> canExecute)
        {
            _parameterizedAction = action;
            _canExecuteWithoutParam = canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute(parameter);
            }
            if (_canExecuteWithoutParam != null)
            {
                return _canExecuteWithoutParam();
            }
            return true;
        }

        public void Execute(object parameter)
        {
            if (_action != null)
            {
                _action();
                return;
            }
            if (_parameterizedAction != null)
            {
                _parameterizedAction(parameter);
                return;
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public class Command : DependencyObject, ICommand
    {
        public Action Action
        {
            get { return (Action)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Action.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof(Action), typeof(Command), new PropertyMetadata(null));

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Action();
            //Dispatcher.invok
        }
    }
}
