namespace Auth.FWT.API.Controllers.Jobs
{
    public class TelegramMessageAction
    {
        public Core.Enums.Enum.TelegramMessageAction Type { get; set; }
        public string ActionMessage { get; set; }
    }
}