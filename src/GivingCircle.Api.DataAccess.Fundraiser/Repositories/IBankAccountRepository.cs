using GivingCircle.Api.Fundraiser.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.Fundraiser.DataAccess
{
    public interface IBankAccountRepository
    {
        //<summary>
        //Gets the bank account
        //<params bank_account_id="bankAccountId">The bank account id</params>
        //<returns>the bank account object if successful or error if not</returns>
        Task<BankAccount> GetBankAccount(string bankAccountId);

        //<summary>
        //Adds the bank account to fundraiser
        //<params bank account obj>The bank account object</params>
        //<returns>true if successful or false/error if not</returns>
        Task<bool> AddBankAccount(BankAccount bankAccount);

        //<summary>
        //Delete the bank account
        //<params bank_account_id="bankAccountId">The bank account id</params>
        //<returns>true if successful or false/error if not</returns>
        Task<bool> DeleteBankAccountAsync(string bankAccountId);
    }
}
