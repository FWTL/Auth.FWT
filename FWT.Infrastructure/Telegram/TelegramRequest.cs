using FluentValidation;
using FluentValidation.Results;
using FWT.Core.Extensions;
using OpenTl.ClientApi.MtProto.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FWT.Infrastructure.Telegram
{
    public static class TelegramRequest
    {
        public static async Task<TResult> Handle<TResult>(Func<Task<TResult>> func, params string[] errorMessages)
        {
            try
            {
                return await func();
            }
            catch (CloudPasswordNeededException ex)
            {
                ThrowValidationException(ex);
            }
            catch (PhoneCodeInvalidException ex)
            {
                ThrowValidationException(ex);
            }
            catch (FileMigrationException ex)
            {
                ThrowValidationException(ex);
            }
            catch (FloodWaitException ex)
            {
                ThrowValidationException(ex);
            }
            catch (UserNotAuthorizeException ex)
            {
                ThrowValidationException(ex);
            }
            catch (PhoneNumberInvalidException ex)
            {
                ThrowValidationException(ex);
            }
            catch (PhoneNumberUnoccupiedException ex)
            {
                ThrowValidationException(ex);
            }
            catch (UnhandledException ex)
            {
                if (errorMessages.IsNotNull() && errorMessages.Contains(ex.Message))
                {
                    return default(TResult);
                }

                throw ex;
            }

            throw new Exception("Unsupported Path");
        }

        private static void ThrowValidationException(Exception ex)
        {
            throw new ValidationException(new List<ValidationFailure>()
            {
                new ValidationFailure("request", ex.Message)
            });
        }
    }
}