using Backend.Services.Exceptions;
using Backend.Services.MoneyConverters;
using Models;

namespace Backend.Services.Tests
{
    [TestClass]
    public class MoneyLanguageConverterTests
    {
        [TestMethod]
        [ExpectedException(typeof(MoneyStringConversionException))]
        [DataTestMethod]
        [DataRow("1000000000,99")]
        [DataRow("-500,99")]
        [DataRow("500.99")]
        public void GetMoneyWordValue_GivenInvalidInputFractionalFormat_ThrowsMoneyStringConversionException(string value)
        {
            var converter = new MoneyLanguageConverter();
            converter.GetMoneyWordValue(value, Currency.UsDollar);
        }

        [TestMethod]
        [ExpectedException(typeof(MoneyStringConversionException))]
        [DataTestMethod]
        [DataRow("55,100")]
        [DataRow("55,-10")]
        [DataRow("55.10")]
        public void GetMoneyWordValue_GivenInvalidInputIntegerFormat_ThrowsMoneyStringConversionException(string value)
        {
            var converter = new MoneyLanguageConverter();
            converter.GetMoneyWordValue(value, Currency.UsDollar);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow(" 1 , 00 ", "One Dollar")]
        [DataRow("0,01", "Zero Dollars and One Cent")]
        [DataRow("2,00", "Two Dollars")]
        [DataRow("0,02", "Zero Dollars and Two Cents")]
        [DataRow("0", "Zero Dollars")]
        [DataRow("25,10", "Twenty-Five Dollars and Ten Cents")]
        [DataRow("45 100,52", "forty-five thousand one hundred dollars and fifty-two cents")]
        [DataRow("123 456 789,99", "one hundred twenty-three million four hundred fifty-six thousand seven hundred eighty-nine dollars and ninety-nine cents")]
        public void GetMoneyWordValue_GivenValidMoneyAmount_ReturnsMoneyWords(string value, string expectedValue)
        {
            var converter = new MoneyLanguageConverter();
            var result = converter.GetMoneyWordValue(value, Currency.UsDollar);

            Assert.IsTrue(result.IndexOf(expectedValue, StringComparison.InvariantCultureIgnoreCase) == 0 );
        }
    }
}