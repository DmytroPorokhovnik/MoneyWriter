using System.Text;
using Backend.Services.Exceptions;
using Backend.Services.Interfaces.MoneyConverters;
using Common;
using Common.Extensions;
using Models;
using Resources;

namespace Backend.Services.MoneyConverters
{
    public class MoneyLanguageConverter: IMoneyLanguageConverter
    {

        #region Fields

        private readonly string[] _digitsAndTeens =
        {
            string.Empty, NumberStrings.One, NumberStrings.Two, NumberStrings.Three, NumberStrings.Four, NumberStrings.Five,
            NumberStrings.Six, NumberStrings.Seven, NumberStrings.Eight, NumberStrings.Nine, NumberStrings.Ten, NumberStrings.Eleven,
            NumberStrings.Twelve, NumberStrings.Thirteen, NumberStrings.Fourteen, NumberStrings.Fifteen, NumberStrings.Sixteen,
            NumberStrings.Seventeen, NumberStrings.Eighteen, NumberStrings.Nineteen
        };

        private readonly string[] _tens =
        {
            string.Empty, NumberStrings.Twenty, NumberStrings.Thirty, NumberStrings.Forty, NumberStrings.Fifty, NumberStrings.Sixty, NumberStrings.Seventy,
            NumberStrings.Eighty, NumberStrings.Ninety
        };

        private readonly string[] _tensPower =
        {
            string.Empty, NumberStrings.Million, NumberStrings.Thousand
        };

        private const string _whitespace = " ";

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public string GetMoneyWordValue(string amount, Currency currency)
        {
            if (string.IsNullOrEmpty(amount))
            {
                throw new ArgumentNullException(nameof(amount));
            }

            if (currency == Currency.None)
            {
                throw new ArgumentNullException(nameof(currency));
            }

            if (!TryGetCurrencyFractional(amount, out var fractional) || fractional >= MoneyConstants.FractionalMax || fractional < 0)
            {
                throw new MoneyStringConversionException();
            }

            if (!TryGetCurrencyInteger(amount, out var integer) || integer >= MoneyConstants.IntegerMax || integer < 0)
            {
                throw new MoneyStringConversionException();
            }

            var integerMoneyString = ConvertMoneyToString(integer);
            var fractionalMoneyString = ConvertMoneyToString(fractional);

            return GetMoneyString(integerMoneyString, fractionalMoneyString, integer, fractional, currency).ToLower();
        }

        #endregion

        #region Private Methods

        private bool TryGetCurrencyFractional(string amount, out int fractional)
        {
            var trimmedAmount = amount.Trim().Replace(_whitespace, string.Empty);

            if (!trimmedAmount.Contains(MoneyConstants.CurrencyDelimiter))
            {
                fractional = 0;
                return true;
            }

            var fractionalString =
                trimmedAmount[(trimmedAmount.IndexOf(MoneyConstants.CurrencyDelimiter, StringComparison.InvariantCultureIgnoreCase) + 1)..];
            return int.TryParse(fractionalString, out fractional);
        }

        private bool TryGetCurrencyInteger(string amount, out int integer)
        {
            var integerString = amount.Trim().Replace(_whitespace, string.Empty);

            if (integerString.Contains(MoneyConstants.CurrencyDelimiter))
            {
                integerString = integerString[..integerString.IndexOf(MoneyConstants.CurrencyDelimiter, StringComparison.InvariantCultureIgnoreCase)];
            }

            return int.TryParse(integerString, out integer);
        }

        private string ConvertMoneyToString(int number)
        {
            switch (number)
            {
                case 0:
                    return NumberStrings.Zero;
                case < 20:
                    return _digitsAndTeens[number];
            }

            var limit = MoneyConstants.IntegerMax;
            var result = new StringBuilder();
            var tenPower = 0;
            for (var i = number; i > 0; i %= limit, limit /= 1000)
            {
                var currentTenPower = ProcessThirdOrMoreTenPower(ref limit, ref i, ref tenPower);

                if (currentTenPower > 99)
                {
                    result.Append($"{_digitsAndTeens[currentTenPower / 100]}{_whitespace}{NumberStrings.Hundred}{_whitespace}");
                }

                currentTenPower %= 100;

                ProcessTens(currentTenPower, result);

                if (tenPower < _tensPower.Length)
                {
                    result.Append($"{_whitespace}{_tensPower[tenPower++]}{_whitespace}");
                }
            }

            return result.ToString();
        }

        private static int ProcessThirdOrMoreTenPower(ref int limit, ref int i, ref int tenPower)
        {
            var currentTenPower = i / limit;
            while (currentTenPower == 0)
            {
                i %= limit;
                limit /= 1000;
                currentTenPower = i / limit;
                ++tenPower;
            }

            return currentTenPower;
        }

        private void ProcessTens(int currentTenPower, StringBuilder result)
        {
            if (currentTenPower is > 0 and < 20)
            {
                result.Append($"{_digitsAndTeens[currentTenPower]}");
            }
            else if (currentTenPower % 10 == 0 && currentTenPower != 0)
            {
                result.Append($"{_tens[currentTenPower / 10 - 1]}");
            }
            else if (currentTenPower is > 20 and < 100)
            {
                result.Append($"{_tens[currentTenPower / 10 - 1]}{NumberStrings.CompundDelimiter}{_digitsAndTeens[currentTenPower % 10]}");
            }
        }

        private string GetMoneyString(string integerMoney, string fractionalMoney, int integerValue, int fractionalValue, Currency currency)
        {
            if (!integerMoney.EndsWith(_whitespace))
            {
                integerMoney = $"{integerMoney}{_whitespace}";
            }

            var integerPart = $"{integerMoney}{currency.GetCurrencyName(integerValue is > 1 or 0)}";
            var fractionalPart = string.Empty;

            if (fractionalValue > 0)
            {
                fractionalPart = $"{fractionalMoney} {currency.GetCurrencyFractionalName(fractionalValue > 1)}";
            }

            return fractionalValue > 0 ? $"{integerPart} {CurrencyStrings.And} {fractionalPart}" : integerPart;
        }

        #endregion

    }
}
