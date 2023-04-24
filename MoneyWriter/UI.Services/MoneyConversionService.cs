using System.Net.Http.Json;
using Models;
using System.Web;
using UI.Services.Interfaces;

namespace UI.Services
{
    public class MoneyConversionService : IMoneyConversionService
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private const string _apiBaseRoute = "api/";
        private const string _converMoneyToWordsEndpointName = "money/getWords";
        private const string _moneyAmountQueryParamName = "moneyAmount";
        private const string _currencyQueryParamName = "currency";
        private const string _serverAddress = "https://localhost:7113/"; //should be passed into constructor via "config manager" from some appSettings.json

        #endregion

        #region Constructor

        public MoneyConversionService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            ConfigureHttpClient();
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async Task<string> ConvertMoneyToWordsAsync(string moneyAmount, Currency currency, CancellationToken cancellationToken)
        {
            var symbolsQuery = HttpUtility.ParseQueryString(string.Empty);
            symbolsQuery.Add(_moneyAmountQueryParamName, moneyAmount);
            symbolsQuery.Add(_currencyQueryParamName, currency.ToString());
            var responseMessage = await _httpClient.GetAsync($"{_converMoneyToWordsEndpointName}?{symbolsQuery}", cancellationToken);
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            }

            return (await responseMessage.Content.ReadFromJsonAsync<ErrorMessageModel>(cancellationToken: cancellationToken))?.Message
                   ?? string.Empty;
        }

        #endregion

        #region Private Methods

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri($"{_serverAddress}{_apiBaseRoute}");
        }

        #endregion
    }
}
