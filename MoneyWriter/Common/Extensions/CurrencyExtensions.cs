using Models;
using Resources;

namespace Common.Extensions
{
    public static class CurrencyExtensions
    {
        /// <exception cref="ArgumentOutOfRangeException">if currency isn't supported</exception>
        public static string GetCurrencyName(this Currency currency, bool isPlural)
        {
            return currency switch
            {
                Currency.UsDollar => isPlural ? CurrencyStrings.USDPlural : CurrencyStrings.USD,
                Currency.None => throw new ArgumentOutOfRangeException(nameof(currency), currency, null),
                _ => throw new ArgumentOutOfRangeException(nameof(currency), currency, null)
            };
        }

        /// <exception cref="ArgumentOutOfRangeException">if currency isn't supported</exception>
        public static string GetCurrencyFractionalName(this Currency currency, bool isPlural)
        {
            return currency switch
            {
                Currency.UsDollar => isPlural ? CurrencyStrings.Cents : CurrencyStrings.Cent,
                Currency.None => throw new ArgumentOutOfRangeException(string.Empty, nameof(currency)),
                _ => throw new ArgumentOutOfRangeException(string.Empty, nameof(currency))
            };
        }
    }
}
