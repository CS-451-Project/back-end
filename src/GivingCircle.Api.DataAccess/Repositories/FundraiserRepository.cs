using Dapper;
using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    /// <inheritdoc />
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

        public async Task<GetFundraiserResponse> GetFundraiserAsync(string fundraiserId)
        {
            // The fundraiser to be returned
            GetFundraiserResponse fundraiser;

            // The query string builder
            StringBuilder queryBuilder = new();

            // The parameters to be given to the query
            DynamicParameters parameters = new();

            parameters.Add("@FundraiserId", fundraiserId);

            // Construct the query
            var query = queryBuilder
                .Append($"SELECT * FROM {_tableName} ")
                .Append("WHERE fundraiser_id=@FundraiserId ")
                .Append("AND closed_date IS NULL")
                .ToString();

            fundraiser = await _postgresClient.QuerySingleAsync<GetFundraiserResponse>(query, parameters);

            return fundraiser ?? null;
        }

        public async Task<IEnumerable<GetFundraiserResponse>> FilterFundraisersAsync(Dictionary<string, string[]> filterProps)
        {
            // The returned fundraiser
            IEnumerable<GetFundraiserResponse> fundraisers;

            // The query parameters
            DynamicParameters parameters = new();

            // Whether or not the filter encountered is the first to be added or not
            bool firstFilter = true;

            // Start the query
            var query = $"SELECT * FROM {_tableName} ";

            // Title filter
            if (filterProps.ContainsKey("Title"))
            {
                // Search if the title column contains our title search filter string
                query += $"WHERE position(lower(@TitleSearchText) in lower(title))>0 ";

                parameters.Add("@TitleSearchText", filterProps["Title"].ElementAt(0));

                firstFilter = false;
            }

            // woudld actually like to build a query string like [@Tag1, @Tag2]

            // Tags filter
            if (filterProps.ContainsKey("Tags"))
            {
                // The list of query tags
                List<string> queryTags = new();

                // The individual query tags
                string queryTag;

                // Add the tags to a string to be used as a query parameter
                for (int i = 0; i < filterProps["Tags"].Length; i++)
                {
                    // Check that we're not on the last tag for commas
                    if (i != filterProps["Tags"].Length - 1)
                    {
                        queryTag = filterProps["Tags"].ElementAt(i);
                        queryTags.Add(queryTag.Trim());
                    }
                    else
                    {
                        queryTag = filterProps["Tags"].ElementAt(i);
                        queryTags.Add(queryTag.Trim());
                    }
                }

                parameters.Add("@TagFilters", queryTags);

                // Whether this part of the query should have an AND or a WHERE
                var whereOrAndClause = firstFilter ? "WHERE" : "AND";

                // Check if our tags overlap with any others in the tag column
                query += $"{whereOrAndClause} ARRAY[@TagFilters] && tags ";

                firstFilter = false;
            }

            // Created Date offset filter
            // We select based on whether or not the created_date for a fundraiser
            // is within the span of days that we pass in. Eg created within the last
            // 7 days.
            if (filterProps.ContainsKey("CreatedDateOffset"))
            {
                // Whether this part of the query should have an AND or not
                var whereOrAndClause = firstFilter ? "WHERE" : "AND";

                query += $"{whereOrAndClause} created_date > @CreatedDateOffset ";

                parameters.Add("@CreatedDateOffset", DateTime.Parse(filterProps["CreatedDateOffset"].ElementAt(0)));

                firstFilter = false;
            }

            // Planned End Date offset filter
            // We select based on whether or not the planned_end_date for a fundraiser
            // is within the span of days that we pass in. Eg ending within the next
            // 30 days.
            if (filterProps.ContainsKey("EndDateOffset"))
            {
                // Whether this part of the query should have an AND or a WHERE
                var whereOrAndClause = firstFilter ? "WHERE" : "AND";

                query += $"{whereOrAndClause} planned_end_date < @EndDateOffset ";

                parameters.Add("@EndDateOffset", DateTime.Parse(filterProps["EndDateOffset"].ElementAt(0)));

                firstFilter = false;
            }

            // Check that the fundraiser aren't closed. Closed being "deleted" / hidden from the public eye.
            query += firstFilter ? "WHERE closed_date IS NULL " : "AND closed_date IS NULL ";

            // Order by whereOrAndClause ascending or descending
            // Note that dapper doesn't like using dynamic parameters for the ORDER BY whereOrAndClause ASC clauses, whereOrAndClause so the 
            // parameters were added through string interpolation
            if (filterProps.ContainsKey("OrderBy") && filterProps["OrderBy"].ElementAt(0) != "current_balance_amount")
            {
                var orderBy = filterProps["OrderBy"].ElementAt(0);
                var ascending = filterProps["Ascending"].ElementAt(0);

                query += $"ORDER BY {orderBy} {ascending}";
            }
            // Logic for when we want to order by the fundraiser that are closest to their target goal
            else if (filterProps.ContainsKey("OrderBy") && filterProps["OrderBy"].ElementAt(0) == "current_balance_amount")
            {
                var ascending = filterProps["Ascending"].ElementAt(0);

                query += $"ORDER BY (goal_target_amount - current_balance_amount) {ascending}";
            }
            // Default to ordering by title ascending if nothing specified
            else
            {
                query += "ORDER BY title ASC";
            }

            // Execute the query on the database
            fundraisers = await _postgresClient.QueryAsync<GetFundraiserResponse>(query, parameters);

            return fundraisers ?? Enumerable.Empty<GetFundraiserResponse>();
        }

        public async Task<IEnumerable<GetFundraiserResponse>> ListFundraisersByUserIdAsync(string userId)
        {
            // The fundraiser to be returned
            IEnumerable<GetFundraiserResponse> fundraisers;

            // The query string builder
            StringBuilder queryBuilder = new();

            // The parameters to be given to the query
            DynamicParameters parameters = new();

            parameters.Add("@UserId", userId);

            // Construct the query
            var query = queryBuilder
                .Append($"SELECT * FROM {_tableName} ")
                .Append("WHERE organizer_id=@UserId ")
                .Append("AND closed_date IS NULL")
                .ToString();

            fundraisers = await _postgresClient.QueryAsync<GetFundraiserResponse>(query, parameters);

            return fundraisers ?? Enumerable.Empty<GetFundraiserResponse>();
        }

        public async Task<bool> CreateFundraiserAsync(string userId, Fundraiser fundraiser)
        {
            // The string builder
            StringBuilder queryBuilder = new();

            // Parameters dictionary
            Dictionary<string, object> parametersDictionary = new()
            {
                { "@PictureId",  fundraiser.PictureId },
                { "@Description",  fundraiser.Description },
                { "@Title", fundraiser.Title},
                { "@PlannedEndDate", fundraiser.PlannedEndDate },
                { "@GoalTargetAmount", fundraiser.GoalTargetAmount },
                { "@Tags", fundraiser.Tags },
                { "@FundraiserId", fundraiser.FundraiserId },
                { "@BankInformationId", fundraiser.BankInformationId },
                { "@GoalReachedDate", fundraiser.GoalReachedDate },
                { "@CurrentBalanceAmount", fundraiser.CurrentBalanceAmount },
                { "@CreatedDate", fundraiser.CreatedDate },
                { "@ClosedDate", fundraiser.ClosedDate },
                { "@OrganizerId", userId }
            };

            // The parameters
            DynamicParameters parameters = new DynamicParameters(parametersDictionary);

            // This represents the number of rows effected by our query
            int createdResult;

            // Construct the query
            var query = queryBuilder
                .Append($"INSERT INTO {_tableName} ")
                .Append("(fundraiser_id, organizer_id, bank_information_id, picture_id, title, description, created_date, planned_end_date, ")
                .Append("goal_reached_date, closed_date, goal_target_amount, current_balance_amount, tags)\n")
                .Append("VALUES (@FundraiserId, @OrganizerId, @BankInformationId, @PictureId, @Title, @Description,")
                .Append("@CreatedDate,@PlannedEndDate, @GoalReachedDate, @ClosedDate, @GoalTargetAmount, @CurrentBalanceAmount, @Tags)")
                .ToString();

            createdResult = await _postgresClient.ExecuteAsync(query, parameters);

            // If we created 1 new fundraiser then we succeeded
            return (createdResult == 1);
        }

        public async Task<bool> UpdateFundraiserAsync(string userId, string fundraiserId, Fundraiser fundraiser)
        {
            // The string builder
            StringBuilder queryBuilder = new();

            // This represents the number of rows effected by our query
            int updatedResult;

            // The dynamic parameters bag
            Dictionary<string, object> parametersDictionary = new()
            {
                { "@PictureId",  fundraiser.PictureId },
                { "@Description",  fundraiser.Description },
                { "@Title", fundraiser.Title},
                { "@PlannedEndDate", fundraiser.PlannedEndDate },
                { "@GoalTargetAmount", fundraiser.GoalTargetAmount },
                { "@Tags", fundraiser.Tags },
                { "@FundraiserId", fundraiserId },
                { "@OrganizerId", userId},
            };

            // The query parameters
            DynamicParameters parameters = new(parametersDictionary);

            // Build the query
            var query = queryBuilder
                .Append($"UPDATE {_tableName} ")
                .Append("SET description = @Description, ")
                .Append("title = @Title, ")
                .Append("planned_end_date = @PlannedEndDate, ")
                .Append("goal_target_amount = @GoalTargetAmount, ")
                .Append("tags = @Tags ")
                .Append("WHERE fundraiser_id = @FundraiserId  AND organizer_id = @OrganizerId")
                .ToString();

            updatedResult = await _postgresClient.ExecuteAsync(query, parameters);

            // If we created 1 new fundraiser then we succeeded
            return (updatedResult == 1);
        }

        public async Task<bool> DeleteFundraiserAsync(string userId, string fundraiserId)
        {
            // The query string builder
            StringBuilder queryBuilder = new();

            // The dynamic parameters to be supplied to the query
            DynamicParameters parameters = new();

            // This represents the number of rows effected by our query
            int deletedResult;

            // Generate todays date
            DateTime closedDate = DateTime.Now;

            parameters.Add("@ClosedDate", closedDate);
            parameters.Add("@FundraiserId", fundraiserId);
            parameters.Add("@OrganizerId", userId);

            // Build the query
            var query = queryBuilder
                .Append($"UPDATE {_tableName} ")
                .Append("SET closed_date = @ClosedDate ")
                .Append("WHERE fundraiser_id = @FundraiserId AND organizer_id = @OrganizerId")
                .ToString();

            deletedResult = await _postgresClient.ExecuteAsync(query, parameters);

            return (deletedResult == 1);
        }

        public async Task<bool> HardDeleteFundraiserAsync(string userId, string fundraiserId)
        {
            // The query string builder
            StringBuilder queryBuilder = new();

            // The dynamic parameters to be supplied to the query
            DynamicParameters parameters = new();

            parameters.Add("@FundraiserId", fundraiserId);
            parameters.Add("@OrganizerId", userId);

            var query = queryBuilder
                .Append($"DELETE FROM {_tableName} ")
                .Append("WHERE fundraiser_id=@FundraiserId AND organizer_id = @OrganizerId")
                .ToString();

            await _postgresClient.ExecuteAsync(query, parameters);

            return true;
        }

        public async Task<bool> MakeDonation(string fundraiserId, double amount)
        {
            // The query string builder
            StringBuilder queryBuilder = new();

            // The dynamic parameters to be supplied to the query
            DynamicParameters parameters = new();

            // This represents the number of rows effected by our query
            int donateResult;

            parameters.Add("@Amount", amount);
            parameters.Add("@FundraiserId", fundraiserId);

            // Build the query
            var query = queryBuilder
                .Append($"UPDATE {_tableName} ")
                .Append("SET current_balance_amount = (current_balance_amount + @Amount) ")
                .Append("WHERE fundraiser_id = @FundraiserId ")
                .ToString();

            donateResult = await _postgresClient.ExecuteAsync(query, parameters);

            return (donateResult == 1);
        }

        public async Task<string> GetFundraiserPictureIdAsync(string fundraiserId)
        {
            // The fundraiser picture id to be returned
            string fundraiserPictureId;

            // The query string builder
            StringBuilder queryBuilder = new();

            // The parameters to be given to the query
            DynamicParameters parameters = new();

            parameters.Add("@FundraiserId", fundraiserId);

            // Construct the query
            var query = queryBuilder
                .Append($"SELECT picture_id FROM {_tableName} ")
                .Append("WHERE fundraiser_id=@FundraiserId ")
                .Append("AND closed_date IS NULL")
                .ToString();

            fundraiserPictureId = await _postgresClient.QuerySingleAsync<string>(query, parameters);

            return fundraiserPictureId ?? null;
        }

        public async Task<bool> UpdateFundraiserPictureIdAsync(string userId, string fundraiserId, string pictureId)
        {
            // The query string builder
            StringBuilder queryBuilder = new();

            // The dynamic parameters to be supplied to the query
            DynamicParameters parameters = new();

            // This represents the number of rows effected by our query
            int result;

            parameters.Add("@OrganizerId", userId);
            parameters.Add("@FundraiserId", fundraiserId);
            parameters.Add("@PictureId", pictureId);

            // Build the query
            var query = queryBuilder
                .Append($"UPDATE {_tableName} ")
                .Append("SET picture_id = @PictureId ")
                .Append("WHERE fundraiser_id = @FundraiserId AND organizer_id = @OrganizerId")
                .ToString();

            result = await _postgresClient.ExecuteAsync(query, parameters);

            return (result == 1);
        }
    }
}

