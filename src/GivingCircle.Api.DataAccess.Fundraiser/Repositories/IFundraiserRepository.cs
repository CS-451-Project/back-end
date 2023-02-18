﻿using GivingCircle.Api.Fundraiser.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.Fundraiser.DataAccess
{
    public interface IFundraiserRepository
    {
        /// <summary>
        /// Gets a list of fundraisers associated with a user's id
        /// </summary>
        /// <param name="userId">The users id</param>
        /// <returns>A list of fundraisers, if any</returns>
        Task<IEnumerable<Models.Fundraiser>> ListFundraisersByUserIdAsync(string userId);

        // TODO: Figure this out 
        Task<IEnumerable<Models.Fundraiser>> FilterFundraisersAsync();

        /// <summary>
        /// Creates a fundraiser
        /// </summary>
        /// <param name="fundraiser">The given fundraiser</param>
        /// <returns>True if success, false or an error if failure</returns>
        Task<bool> CreateFundraiserAsync(Models.Fundraiser fundraiser);

        /// <summary>
        /// Updates the fundraiser using the given object
        /// </summary>
        /// <param name="fundraiser">The fundraiser to update to</param>
        /// <returns>True if success, false or an error if un successful</returns>
        Task<bool> UpdateFundraiserAsync(Models.Fundraiser fundraiser);

        /// <summary>
        /// Deletes a fundraiser
        /// </summary>
        /// <param name="fundraiserId">The fundraiser's id</param>
        /// <returns>True if success, false or an error if not</returns>
        Task<bool> DeleteFundraiserAsync(string fundraiserId);


        //BANK ACCOUNT METHODS

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