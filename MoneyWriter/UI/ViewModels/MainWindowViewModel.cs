using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Models;
using Resources;
using UI.Commands;
using UI.Services.Interfaces;

namespace UI.ViewModels
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        #region Fields

        private string _moneyInput = string.Empty;
        private string _convertedValue = string.Empty;
        private readonly IMoneyConversionService _moneyConversionService;
        private CancellationTokenSource? _conversionCancellationTokenSource;
        
        #endregion

        #region Public Properties

        public string MoneyInput
        {
            get => _moneyInput;
            set => Set(ref _moneyInput, value);
        }

        public string ConvertedValue
        {
            get => _convertedValue;
            set => Set(ref _convertedValue, value);
        }

        public ICommand ConvertMoneyInputCommand { get; }

        #endregion

        #region Constuctor

        public MainWindowViewModel(IMoneyConversionService moneyConversionService)
        {
            _moneyConversionService = moneyConversionService ?? throw new ArgumentNullException(nameof(moneyConversionService));
            ConvertMoneyInputCommand = new AsyncCommand<string>(ConvertMoneyAsync, () => true, OnConvertFinished, OnConvertException);
        }

        #endregion

        #region Private Methods

        private Task<string> ConvertMoneyAsync(CancellationToken cancellation)
        {
            _conversionCancellationTokenSource?.Cancel();
            _conversionCancellationTokenSource = new CancellationTokenSource();
           return _moneyConversionService.ConvertMoneyToWordsAsync(MoneyInput, Currency.UsDollar, _conversionCancellationTokenSource.Token);
        }

        private void OnConvertFinished(TaskStatus taskStatus, string? result)
        {
            if (taskStatus == TaskStatus.RanToCompletion && result != null)
            {
                ConvertedValue = result;
            }
        }

        private void OnConvertException(Exception ex)
        {
            //log
            MessageBox.Show(MainWindowStrings.UnexpectedError);
        }

        #endregion
    }
}