using UI.Commands;

namespace UI.Tests
{
    [TestClass]
    public class AsyncCommandTests
    {
        #region Fields

        private const int _delayMs = 100;
        private const int _cycleCounter = 10;
        private const int _doTaskResult = 2025;
        private Exception _commandException;
        private TaskStatus _finishedTaskStatus;
        private int _actualResult;
        private IAsyncCommand<int> _asyncCommand;
        private readonly Semaphore _semaphore = new(1, 1);
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        #endregion

        #region TestInitialize

        [TestInitialize]
        public void Initialize()
        {
            _commandException = default;
            _finishedTaskStatus = default;
            _actualResult = default;
        }

        #endregion

        #region Execute tests

        [TestMethod]
        public void Execute_CommandWithAllParams_ReturnsActualResult()
        {
            _asyncCommand = new AsyncCommand<int>(DoTask, () => true, OnFinished, OnException);
            Assert.IsTrue(_asyncCommand.CanExecute(new object()));

            _asyncCommand.Execute(new object());
            _semaphore.WaitOne();

            Assert.IsTrue(_actualResult == _doTaskResult);
            Assert.IsTrue(_finishedTaskStatus == TaskStatus.RanToCompletion);
            Assert.IsTrue(_commandException == null);
            Assert.IsTrue(_asyncCommand.CanExecute(new object()));
            _semaphore.Release();
        }

        [TestMethod]
        public void Execute_CommandWithoutParams_ExecutesSuccessfulReturnsNothing()
        {
            _asyncCommand = new AsyncCommand<int>(DoTask);
            Assert.IsTrue(_asyncCommand.CanExecute(new object()));

            _asyncCommand.Execute(new object());
            _semaphore.WaitOne();

            Assert.IsTrue(_actualResult == 0);
            Assert.IsTrue(_finishedTaskStatus == default);
            Assert.IsTrue(_commandException == null);
            Assert.IsTrue(_asyncCommand.CanExecute(new object()));
            _semaphore.Release();
        }

        [TestMethod]
        public async Task Execute_CancelCommand_ReturnsTaskCancelledStatus()
        {
            _asyncCommand = new AsyncCommand<int>(DoTask, () => true, OnFinished, OnException);
            Assert.IsTrue(_asyncCommand.CanExecute(new object()));

            _asyncCommand.Execute(new object());
            _asyncCommand.CancellationTokenSource.Cancel();

            await Task.Delay(100);
            Assert.IsTrue(_actualResult == 0);
            Assert.IsTrue(_finishedTaskStatus == TaskStatus.Canceled);
            Assert.IsTrue(_commandException == null);
            Assert.IsTrue(_asyncCommand.CanExecute(new object()));
        }

        [TestMethod]
        public async Task Execute_CommandWithException_ReturnsException()
        {
            var exception = new InvalidOperationException();
            _asyncCommand = new AsyncCommand<int>(token => throw exception, () => true, OnFinished, OnException);
            _asyncCommand.Execute(new object());
            await Task.Delay(_delayMs);
            Assert.IsTrue(_actualResult == 0);
            Assert.IsTrue(_finishedTaskStatus == TaskStatus.Faulted);
            Assert.IsTrue(_commandException == exception);
            Assert.IsTrue(_asyncCommand.CanExecute(new object()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Execute_CommandWithoutTask_ThrowsArgumentNullException()
        {
            _asyncCommand = new AsyncCommand<int>(null);
        }

        #endregion

        #region CanExecute Tests

        [TestMethod]
        public void CanExecute_CommandCannotExecute_ReturnsFalse()
        {
            _asyncCommand = new AsyncCommand<int>(DoTask, () => false, OnFinished, OnException);
            Assert.IsFalse(_asyncCommand.CanExecute(new object()));
        }

        [TestMethod]
        public void CanExecute_GivenCommand_FalseDuringExecution()
        {
            _asyncCommand = new AsyncCommand<int>(DoTask, () => true);
            Assert.IsTrue(_asyncCommand.CanExecute(new object()));

            _asyncCommand.Execute(new object());
            Assert.IsFalse(_asyncCommand.CanExecute(new object()));
            _semaphore.WaitOne();
            Assert.IsTrue(_asyncCommand.CanExecute(new object()));
            _semaphore.Release();
        }

        #endregion

        #region IsBusy Tests

        [TestMethod]
        public async Task IsBusy_CommandWithParams_TrueDuringExecution()
        {
            await _semaphoreSlim.WaitAsync();

            _asyncCommand = new AsyncCommand<int>(async token =>
            {
                try
                {
                    await _semaphoreSlim.WaitAsync();
                    return 0;
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }, () => true, OnFinished, OnException);

            _asyncCommand.Execute(new object());
            Assert.IsTrue(_asyncCommand.IsBusy);
            _semaphoreSlim.Release();
        }

        [TestMethod]
        public async Task IsBusy_CommandWithParams_FalseAfterExecution()
        {
            _asyncCommand = new AsyncCommand<int>(DoTask, () => true, OnFinished, OnException);
            await _asyncCommand.ExecuteAsync(new object());
            Assert.IsFalse(_asyncCommand.IsBusy);
        }

        #endregion

        #region Private Methods

        private async Task<int> DoTask(CancellationToken cancellationToken)
        {
            _semaphore.WaitOne();
            try
            {
                for (var i = 0; i < _cycleCounter; i++)
                {
                    await Task.Delay(_delayMs, cancellationToken);
                }

                return _doTaskResult;
            }
            finally
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(_delayMs);
                    return _semaphore.Release();
                });
            }
        }

        private void OnException(Exception e)
        {
            _commandException = e;
        }

        private void OnFinished(TaskStatus taskStatus, int result)
        {
            _finishedTaskStatus = taskStatus;
            _actualResult = result;
        }

        #endregion
    }
}