using System.ComponentModel.DataAnnotations;
using Backend.Services.Exceptions;
using Backend.Services.Interfaces.MoneyConverters;
using Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Models;
using Resources;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/money")]
    public class MoneyConverterController : ControllerBase
    {
        private readonly IMoneyLanguageConverter _moneyLanguageConverter;

        public MoneyConverterController(IMoneyLanguageConverter moneyLanguageConverter)
        {
            _moneyLanguageConverter = moneyLanguageConverter;
        }

        /// <summary>
        /// Money to words endpoint
        /// </summary>
        /// <param name="moneyAmount">string representation of money amount</param>
        /// <param name="currency">currency for money language conversion</param>
        /// <returns>converted money to words string</returns>
        [HttpGet("getWords")]
        public ActionResult<string> GetWords([Required] string moneyAmount, [Required] Currency currency)
        {
            try
            {
                var moneyWords = _moneyLanguageConverter.GetMoneyWordValue(moneyAmount, currency);
                return Ok(moneyWords);
            }
            catch (ArgumentNullException)
            {
                return BadRequest(ErrorMessages.WrongMoneyFormat.GetJsonMessage());
            }
            catch (MoneyStringConversionException)
            {
                return BadRequest(ErrorMessages.WrongMoneyFormat.GetJsonMessage());
            }
        }
    }
}