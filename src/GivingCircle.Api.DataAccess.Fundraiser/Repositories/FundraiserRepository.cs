using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.Fundraiser.DataAccess.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.Fundraiser.DataAccess
{
    /// <summary>
    /// A class to manage access to the fundraisers database
    /// </summary>
    public class FundraiserRepository : IFundraiserRepository
    {
        private readonly PostgresClient _postgresClient;

        private readonly string _tableName = "fundraisers";

        /// <summary>
        /// Initializes an instance of the <see cref="FundraiserRepository"/> class
        /// </summary>
        /// <param name="postgresClient">The postgres client</param>
        public FundraiserRepository(PostgresClient postgresClient)
        {
            _postgresClient = postgresClient;
        }

        /// <inheritdoc/>
        public async Task<bool> CreateFundraiserAsync(Models.Fundraiser fundraiser)
        {
            StringBuilder queryBuilder = new StringBuilder();
            int created = 0;

            // Construct the query
            queryBuilder.Append($"INSERT INTO {_tableName} ")
                 .Append("(fundraiser_id, organizer_id, bank_information_id, picture_id, title, description, created_date, planned_end_date, ")
                 .Append("goal_reached_date, closed_date, goal_target_amount, current_balance_amount, tags)\n")
                 .Append("VALUES (@FundraiserId, @OrganizerId, @BankInformationId, @PictureId, @Title, @Description,")
                 .Append("@CreatedDate,@PlannedEndDate, @GoalReachedDate, @ClosedDate, @GoalTargetAmount, @CurrentBalanceAmount, @Tags)")
                 .ToString();

            var query = queryBuilder.ToString();

            try
            {
                // Execute the query on the database
                created = await _postgresClient.ExecuteAsync(query, fundraiser);
            }
            catch (Npgsql.PostgresException err)
            {
                // 23503 is the error code thrown when we violate referential integrity
                if (err.SqlState == "23503")
                {
                    throw new BankAccountIdInvalidException("Bank account id is invalid or DNE");
                }
                Console.WriteLine(err.Message);
                return false;
            }

            // If we created 1 new fundraiser then we succeeded
            if (created == 1)
            {
                return true;
            }
            // Else something went wrong
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public Task<bool> DeleteFundraiserAsync(string fundraiserId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Models.Fundraiser>> FilterFundraisersAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Models.Fundraiser>> ListFundraisersByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> UpdateFundraiserAsync(Models.Fundraiser fundraiser)
        {
            throw new NotImplementedException();
        }
    }
}
