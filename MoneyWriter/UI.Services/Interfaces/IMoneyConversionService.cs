
using Models;

namespace UI.Services.Interfaces
{
    /// <summary>
    /// Represents money conversion rest service
    /// </summary>
    public interface IMoneyConversionService
    {
        /// <summary>
        /// Converts money to words asynchronously
        /// </summary>
        /// <param name="moneyAmount">money amount string</param>
        /// <param name="currency">money currency</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>money words</returns>
        /// <exception cref="HttpRequestException">The HTTP response is unsuccessful.</exception>
        Task<string> ConvertMoneyToWordsAsync(string moneyAmount, Currency currency, CancellationToken cancellationToken);
    }
}
