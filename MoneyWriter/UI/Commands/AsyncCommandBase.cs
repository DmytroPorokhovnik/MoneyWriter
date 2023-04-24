using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Common.Extensions;
using UI.ViewModels;

namespace UI.Commands
{
    public abstract class AsyncCommandBase<TResult> : NotifyPropertyChangedBase, IAsyncCommand<TResult>
    {
        #region Properties

        public bool IsBusy
        {
            get => _isBusy;
            protected set => Set(ref _isBusy, value);
        }

        public CancellationTokenSource CancellationTokenSource { get; protected set; }

        #endregion

        #region Fields

        private bool _isBusy;
        protected event EventHandler _canExecuteChanged = (sender, args) => { };
        protected readonly Func<bool>? _canExecuteCondition;
        protected readonly Action<Exception>? _onException;
        protected readonly Action<TaskStatus, TResult>? _onFinished;

        #endregion

        #region Constructor

        protected AsyncCommandBase(Func<bool>? canExecute = null, Action<TaskStatus, TResult?>? onFinished = null,
            Action<Exception>? onException = null)
        {
            _onException = onException;
            _onFinished = onFinished;
            _canExecuteCondition = canExecute ;

            CancellationTokenSource = new CancellationTokenSource();
        }

        #endregion

        #region ICommand implementation

        public void Execute(object? parameter)
        {
            ExecuteAsync(parameter).RunSafe(OnException);
        }

        public bool CanExecute(object? parameter)
        {
            return !IsBusy && (_canExecuteCondition?.Invoke() ?? true);
        }

        public event EventHandler CanExecuteChanged
        {
            add => _canExecuteChanged += value;
            remove => _canExecuteChanged -= value;
        }

        #endregion

        #region Public Methods

        public abstract Task ExecuteAsync(object parameter);

        #endregion

        #region Protected Methods

        protected void OnException(Exception exception)
        {
            if (_onException == null)
            {
                throw exception;
            }

            _onException.Invoke(exception);
        }

        protected void RaiseCanExecuteChanged()
        {
            _canExecuteChanged.Invoke(this, EventArgs.Empty);
        }

        protected void CanExecuteOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        #endregion
    }

}