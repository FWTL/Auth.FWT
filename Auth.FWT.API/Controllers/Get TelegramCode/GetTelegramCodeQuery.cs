using System.Text.RegularExpressions;
using Auth.FWT.CQRS;

namespace Auth.FWT.API.Controllers.Get_TelegramCode
{
    public class GetTelegramCodeQuery : IQuery
    {
        public GetTelegramCodeQuery(string phoneNumber)
        {
            PhoneNumber = $"+{Regex.Match(phoneNumber, @"\d+").Value}";
        }

        public string PhoneNumber { get; set; }
    }
}