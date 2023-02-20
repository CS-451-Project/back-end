using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.Fundraiser.Models;
using GivingCircle.Api.Fundraiser.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.Fundraiser.DataAccess.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly PostgresClient _postgresClient;

        private readonly string _tableName = "fundraisers";
        private readonly string _banktable = "bank_account";

        /// <summary>
        /// Initializes an instance of the <see cref="BankAccountRepository"/> class
        /// </summary>
        /// <param name="postgresClient">The postgres client</param>
        public BankAccountRepository(PostgresClient postgresClient)
        {
            _postgresClient = postgresClient;
        }

        //BANK ACCOUNT METHODS

        public async Task<BankAccount> GetBankAccount(string bankAccountId)
        {
            // Object to map the parameters to the query
            object parameters = new { Bank_Account_Id = bankAccountId };

            var bankAccount = await _postgresClient.QuerySingleAsync<BankAccount>("SELECT * FROM bank_account WHERE bank_account_id = @Bank_Account_Id", parameters);
            
            return bankAccount;
        }

        public async Task<bool> AddBankAccount(BankAccount bankAccount)
        {
            StringBuilder query = new StringBuilder();
            int createBankAccountResult = 0;

            //creates the sql string
            query
                .Append("INSERT INTO bank_account (account_name, address, city, state, zipcode, bank_name, account_num, routing_num, account_type, bank_account_id) ")
                .Append("VALUES (@Account_Name, @Address, @City, @State, @Zipcode, @Bank_Name, @Account_Num, @Routing_Num, @Account_Type, @Bank_Account_Id)").ToString();

            var querybuild = query.ToString();

            try
            {
                // Execute the query on the database
                createBankAccountResult = await _postgresClient.ExecuteAsync(querybuild, bankAccount);
            }
            catch (Npgsql.PostgresException err)
            {

            }

            return createBankAccountResult == 1 ? true : false;

        }

        public async Task<bool> DeleteBankAccountAsync(string bankAccountId)
        {
            StringBuilder query = new StringBuilder();

            // Object to map the parameters to the query
            object parameters = new { Bank_Account_Id = bankAccountId };

            // Will return 1 if successful
            var deleteBankAccount = await _postgresClient.ExecuteAsync(query
                .Append("DELETE FROM bank_account ")
                .Append("WHERE bank_account_id = @Bank_Account_Id").ToString(),
                parameters);

            return deleteBankAccount == 1 ? true : false;
        }
    }
}
