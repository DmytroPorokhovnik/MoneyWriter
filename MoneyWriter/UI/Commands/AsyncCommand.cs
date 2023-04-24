using System;
using System.Threading;
using System.Threading.Tasks;

namespace UI.Commands
{
  public class AsyncCommand<TResult> : AsyncCommandBase<TResult>
  {
    #region Fields

    private readonly Func<CancellationToken, Task<TResult>> _command;

    #endregion

    #region Constructor

    public AsyncCommand(Func<CancellationToken, Task<TResult>> command, Func<bool>? canExecute = null, Action<TaskStatus, TResult?>? onFinished = null,
        Action<Exception>? onException = null) :base(canExecute, onFinished, onException)
    {
      _command = command ?? throw new ArgumentNullException(nameof(command));
    }

    #endregion

    #region Public Methods

    public override async Task ExecuteAsync(object parameter)
    {
      if (IsBusy)
      {
        throw new AsyncCommandAlreadyInProgressException();
      }

      IsBusy = true;
      CancellationTokenSource = new CancellationTokenSource();
      var result = default(TResult);
      Task<TResult>? commandTask = null;
      try
      {
        commandTask = _command(CancellationTokenSource.Token);
        RaiseCanExecuteChanged();
        result = await commandTask;
      }
      catch (TaskCanceledException)
      {
        //ignore, result is processed in finally block
      }
      finally
      {
        IsBusy = false;
        RaiseCanExecuteChanged();
        _onFinished?.Invoke(commandTask?.Status ?? TaskStatus.Faulted, result);
      }
    }

    #endregion
  }
}
