using GivingCircle.Api.Models;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public interface IBankAccountRepository
    {
        //<summary>
        //Gets the bank account
        //<params bank_account_id="bankAccountId">The bank account id</params>
        //<returns>the bank account object if successful or error if not</returns>
        Task<BankAccount> GetBankAccount(string userId, string bankAccountId);

        //<summary>
        //Adds the bank account to fundraiser
        //<params bank account obj>The bank account object</params>
        //<returns>true if successful or false/error if not</returns>
        Task<bool> AddBankAccount(string userId, BankAccount bankAccount);

        //<summary>
        //Delete the bank account
        //<params bank_account_id="bankAccountId">The bank account id</params>
        //<returns>true if successful or false/error if not</returns>
        Task<bool> DeleteBankAccountAsync(string userId, string bankAccountId);
    }
}
