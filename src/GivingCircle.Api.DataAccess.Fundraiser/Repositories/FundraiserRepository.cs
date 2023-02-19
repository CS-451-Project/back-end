using Dapper;
using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.Fundraiser.DataAccess.Exceptions;
using GivingCircle.Api.Fundraiser.DataAccess.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IEnumerable<GetFundraiserResponse>> FilterFundraisersAsync(Dictionary<string, string[]> filterProps)
        {
            IEnumerable<GetFundraiserResponse> fundraisers;
            DynamicParameters parameters = new();
            bool firstFilter = true;

            // Start the query
            var query = $"SELECT * FROM {_tableName} ";

            // Title filter
            if (filterProps.ContainsKey("Title"))
            {
                // Search if the title column contains our title search filter
                query += $"WHERE position(lower(@TitleSearchText) in lower(title))>0 ";

                parameters.Add("@TitleSearchText", filterProps["Title"].ElementAt(0));

                firstFilter = false;
            }

            // Tags filter
            if (filterProps.ContainsKey("Tags"))
            {
                string queryTags = string.Empty;

                // Add the tags to a string to be used as a query parameter
                for (int i = 0; i < filterProps["Tags"].Length; i++)
                {
                    // Check that we're not on the last tag for commas
                    if (i != filterProps["Tags"].Length - 1)
                    {
                        queryTags += filterProps["Tags"].ElementAt(i) + ", ";
                    }
                    else
                    {
                        queryTags += filterProps["Tags"].ElementAt(i);
                    }
                }

                parameters.Add("@TagFilters", queryTags);

                // Whether this part of the query should have an AND or not
                var and = firstFilter ? "WHERE" : "AND";

                // Check if our tags overlap with any others in the tag column
                query += $"{and} ARRAY[@TagFilters] && tags ";

                firstFilter = false;
            }

            // Created Date filter
            if (filterProps.ContainsKey(""))

            // Check that the fundraisers aren't closed
            query += "AND closed_date IS NULL ";

            // Execute the query on the database
            fundraisers = await _postgresClient.QueryAsync<GetFundraiserResponse>(query, parameters);

            return fundraisers ?? Enumerable.Empty<GetFundraiserResponse>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GetFundraiserResponse>> ListFundraisersByUserIdAsync(string userId)
        {
            IEnumerable<GetFundraiserResponse> fundraisers;
            StringBuilder queryBuilder = new();
            DynamicParameters parameters = new ();

            parameters.Add("@UserId", userId);

            // Construct the query
            var query = queryBuilder
                .Append($"SELECT * FROM {_tableName} ")
                .Append("WHERE organizer_id=@UserId ")
                .Append("AND closed_date IS NULL")
                .ToString();
            
            // Execute the query on the database
            fundraisers = await _postgresClient.QueryAsync<GetFundraiserResponse>(query, parameters);

            return fundraisers ?? Enumerable.Empty<GetFundraiserResponse>();
        }

        /// <inheritdoc/>
        public async Task<bool> CreateFundraiserAsync(Models.Fundraiser fundraiser)
        {
            StringBuilder queryBuilder = new();
            int createdResult;

            // Construct the query
            var query = queryBuilder
                .Append($"INSERT INTO {_tableName} ")
                .Append("(fundraiser_id, organizer_id, bank_information_id, picture_id, title, description, created_date, planned_end_date, ")
                .Append("goal_reached_date, closed_date, goal_target_amount, current_balance_amount, tags)\n")
                .Append("VALUES (@FundraiserId, @OrganizerId, @BankInformationId, @PictureId, @Title, @Description,")
                .Append("@CreatedDate,@PlannedEndDate, @GoalReachedDate, @ClosedDate, @GoalTargetAmount, @CurrentBalanceAmount, @Tags)")
                .ToString();

            try
            {
                // Execute the query on the database
                createdResult = await _postgresClient.ExecuteAsync(query, fundraiser);
            }
            catch (Npgsql.PostgresException err)
            {
                // 23503 is the error code thrown when we violate referential integrity
                if (err.SqlState == "23503")
                {
                    throw new InvalidBankAccountIdException("Bank account id is invalid or DNE");
                }
                return false;
            }

            // If we created 1 new fundraiser then we succeeded
            return (createdResult == 1);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteFundraiserAsync(string fundraiserId)
        {
            StringBuilder queryBuilder = new();
            DynamicParameters parameters = new();
            DateTime closedDate = DateTime.Now;

            parameters.Add("@ClosedDate", closedDate);
            parameters.Add("@FundraiserId", fundraiserId);

            //var query = queryBuilder
            //    .Append($"DELETE FROM {_tableName} ")
            //    .Append("WHERE fundraiser_id=@FundraiserId")
            //    .ToString();

            var query = queryBuilder
                .Append($"UPDATE {_tableName} ")
                .Append("SET closed_date = @ClosedDate ")
                .Append("WHERE fundraiser_id = @FundraiserId")
                .ToString();

            try
            {
                // Note that no int is being detected. Because this is a distributed system,
                // we don't check for rows deleted because someone else may have already deleted the item
                await _postgresClient.ExecuteAsync(query, parameters);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public Task<bool> UpdateFundraiserAsync(Models.Fundraiser fundraiser)
        {
            throw new NotImplementedException();
        }
    }
}
