using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.Commands
{
  /// <summary>
  /// Represents async command
  /// </summary>
  public interface IAsyncCommand<TResult> : ICommand, INotifyPropertyChanged
  {
    bool IsBusy { get; }
    Task ExecuteAsync(object parameter);
    CancellationTokenSource CancellationTokenSource { get; }
  }
}
