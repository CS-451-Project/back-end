using System;

namespace GivingCircle.Api.Fundraiser.DataAccess.Exceptions
{
    public class BankAccountIdInvalidException : Exception
    {
        public BankAccountIdInvalidException()
        {
        }

        public BankAccountIdInvalidException(string message)
            : base(message)
        {
        }

        public BankAccountIdInvalidException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
