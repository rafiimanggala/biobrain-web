using System;

namespace Common.ErrorHandling
{
    public enum ErrorType
    {
        Unhandled,
        AuthorizationError,
        GetAccountError,
        UpdateAccountError,
        RegistrationUsedEmail,
        RegistrationWeakPassword,
        EmailError,
        PasswordError,
        TooManyAttemptsError
    }

    public class FirebaseException : Exception
    {
        public ErrorType ErrorType;

        public FirebaseException(ErrorType errorType)
        {
            ErrorType = errorType;
        }


    }
}