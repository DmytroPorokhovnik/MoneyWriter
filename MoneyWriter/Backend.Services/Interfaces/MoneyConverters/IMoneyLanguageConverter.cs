using Backend.Services.Exceptions;
using Models;

namespace Backend.Services.Interfaces.MoneyConverters
{
    public interface IMoneyLanguageConverter
    {
        /// <summary>
        /// Converts number that represents money to words number using given currency
        /// </summary>
        /// <param name="amount">money number</param>
        /// <param name="currency">money currency</param>
        /// <returns>money word representation</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="amount"/> is empty or <paramref name="currency"/> is none/</exception>
        /// <exception cref="MoneyStringConversionException">When money input is in a wrong format/</exception>
        string GetMoneyWordValue(string amount, Currency currency);
    }
}
