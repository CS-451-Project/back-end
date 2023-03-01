using Dapper;
using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly PostgresClient _postgresClient;

        /// <summary>
        /// Initializes an instance of the <see cref="BankAccountRepository"/> class
        /// </summary>
        /// <param name="postgresClient">The postgres client</param>
        public BankAccountRepository(PostgresClient postgresClient)
        {
            _postgresClient = postgresClient;
        }

        public async Task<BankAccount> GetBankAccount(string userId, string bankAccountId)
        {
            // Object to map the parameters to the query
            object parameters = new 
            { 
                Bank_Account_Id = bankAccountId , 
                UserId = userId 
            };

            var bankAccount = await _postgresClient.QuerySingleAsync<BankAccount>("SELECT * FROM bank_accounts WHERE bank_account_id = @Bank_Account_Id AND user_id = @UserId", parameters);

            return bankAccount;
        }

        public async Task<bool> AddBankAccount(string userId, BankAccount bankAccount)
        {
            StringBuilder query = new StringBuilder();

            // Parameters dictionary
            Dictionary<string, object> parametersDictionary = new()
            {
                { "@Account_Name",  bankAccount.Account_Name },
                { "@Address",  bankAccount.Address },
                { "@City", bankAccount.City},
                { "@State", bankAccount.State },
                { "@Zipcode", bankAccount.Zipcode },
                { "@Bank_Name", bankAccount.Bank_Name },
                { "@Account_Num", bankAccount.Account_Num },
                { "@Routing_Num", bankAccount.Routing_Num },
                { "@Account_Type", bankAccount.Account_Type },
                { "@Bank_Account_Id", bankAccount.Bank_Account_Id },
                { "@UserId", userId },
            };

            // The query parameters
            DynamicParameters parameters = new DynamicParameters(parametersDictionary);

            //creates the sql string
            query
                .Append("INSERT INTO bank_accounts (account_name, address, city, state, zipcode, bank_name, account_num, routing_num, account_type, bank_account_id, user_id) ")
                .Append("VALUES (@Account_Name, @Address, @City, @State, @Zipcode, @Bank_Name, @Account_Num, @Routing_Num, @Account_Type, @Bank_Account_Id, @UserId)").ToString();

            var querybuild = query.ToString();

            // Execute the query on the database
            var createBankAccountResult = await _postgresClient.ExecuteAsync(querybuild, parameters);

            return createBankAccountResult == 1 ? true : false;

        }

        public async Task<bool> DeleteBankAccountAsync(string userId, string bankAccountId)
        {
            StringBuilder query = new StringBuilder();

            // Object to map the parameters to the query
            object parameters = new { Bank_Account_Id = bankAccountId, UserId = userId };

            // Will return 1 if successful
            var deleteBankAccount = await _postgresClient.ExecuteAsync(query
                .Append("DELETE FROM bank_accounts ")
                .Append("WHERE bank_account_id = @Bank_Account_Id AND user_id = @UserId").ToString(),
                parameters);

            return deleteBankAccount == 1 ? true : false;
        }
    }
}
