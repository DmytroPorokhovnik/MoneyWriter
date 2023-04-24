using System.Net;
using Models;
using Moq;
using Moq.Protected;

namespace UI.Services.Tests
{
    [TestClass]
    public class MoneyConversionServiceTests
    {
        #region Fields

        private const string _serverAddress = "https://localhost:7113/";

        #endregion

        #region ConvertMoneyToWordsAsync Tests

        [TestMethod]
        public async Task ConvertMoneyToWordsAsync_GivenMoneyString_ReturnsConvertedMoney()
        {
            //Arrange
            var httpHandler = new Mock<HttpMessageHandler>();
            var expectedResult = "1234";
            httpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>((requestMessage) =>
                    requestMessage.RequestUri.ToString().Contains($"{_serverAddress}api/money/getWords")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedResult)
                });

            var moneyConversionService = new MoneyConversionService(new HttpClient(httpHandler.Object));

            //Act
            var value = await moneyConversionService.ConvertMoneyToWordsAsync("45", Currency.UsDollar, CancellationToken.None);

            //Assert
            httpHandler.VerifyAll();
            Assert.AreEqual(expectedResult, value);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task ConvertMoneyToWordsAsync_GivenHttpErrorStatus_ThrowsHttpRequestException()
        {
            //Arrange
            var httpHandler = new Mock<HttpMessageHandler>();
            var expectedResult = "1234";
            httpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>((requestMessage) =>
                    requestMessage.RequestUri.ToString().Contains($"{_serverAddress}api/money/getWords")), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(expectedResult)
                });

            var moneyConversionService = new MoneyConversionService(new HttpClient(httpHandler.Object));

            //Act
            _ = await moneyConversionService.ConvertMoneyToWordsAsync(expectedResult, Currency.UsDollar, CancellationToken.None);

            //Assert
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task ConvertMoneyToWordsAsync_GivenCancelledToken_ThrowsTaskCanceledException()
        {
            //Arrange
            var httpHandler = new Mock<HttpMessageHandler>();
            httpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.Is<CancellationToken>(token => token.IsCancellationRequested))
                .Throws(new TaskCanceledException());
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();

            var moneyConversionService = new MoneyConversionService(new HttpClient(httpHandler.Object));

            //Act
            _ = await moneyConversionService.ConvertMoneyToWordsAsync("test", Currency.UsDollar, tokenSource.Token);

            //Assert
        }

        #endregion
    }
}