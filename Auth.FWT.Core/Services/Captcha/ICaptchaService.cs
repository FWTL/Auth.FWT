namespace Auth.FWT.Core.Services.Captcha
{
    public interface ICaptchaService
    {
        bool Validate(string response);
    }
}