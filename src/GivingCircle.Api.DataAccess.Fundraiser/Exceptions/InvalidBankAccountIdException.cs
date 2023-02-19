using System;

namespace GivingCircle.Api.Fundraiser.DataAccess.Exceptions
{
    public class InvalidBankAccountIdException : Exception
    {
        public InvalidBankAccountIdException()
        {
        }

        public InvalidBankAccountIdException(string message)
            : base(message)
        {
        }

        public InvalidBankAccountIdException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
