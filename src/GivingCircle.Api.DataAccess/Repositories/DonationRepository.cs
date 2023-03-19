using Dapper;
using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    /// <inheritdoc />
    public class DonationRepository : IDonationRepository
    {
        private readonly PostgresClient _postgresClient;

        private readonly string _tableName = "donations";

        /// <summary>
        /// A repository for managing <see cref="Donation"/> objects
        /// </summary>
        /// <param name="postgresClient">The postgres client</param>
        public DonationRepository(PostgresClient postgresClient) 
        {
            _postgresClient = postgresClient;
        }

        public async Task<IEnumerable<Donation>> GetFundraiserDonations(string fundraiserId)
        {
            // The fundraiser to be returned
            IEnumerable<Donation> donations;

            // The query string builder
            StringBuilder queryBuilder = new();

            // The parameters to be given to the query
            DynamicParameters parameters = new();

            parameters.Add("@FundraiserId", fundraiserId);

            // Construct the query
            var query = queryBuilder
                .Append($"SELECT * FROM {_tableName} ")
                .Append("WHERE fundraiser_id=@FundraiserId ")
                .Append("ORDER BY date DESC ")
                .ToString();

            donations = await _postgresClient.QueryAsync<Donation>(query, parameters);

            return donations ?? null;
        }

        public Task<IEnumerable<Donation>> GetUserDonations(string userId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> MakeDonation(Donation donation)
        {
            // The string builder
            StringBuilder queryBuilder = new();

            // Parameters dictionary
            Dictionary<string, object> parametersDictionary = new()
            {
                { "@DonationId",  donation.DonationId },
                { "@FundraiserId",  donation.FundraiserId },
                { "@UserId", donation.UserId},
                { "@Message", donation.Message },
                { "@Date", donation.Date },
                { "@Amount", donation.Amount },
                { "@Name", donation.Name }
            };

            // The parameters
            DynamicParameters parameters = new(parametersDictionary);

            // This represents the number of rows affected by our query
            int createdResult;

            // Construct the query
            var query = queryBuilder
                .Append($"INSERT INTO {_tableName} ")
                .Append("(fundraiser_id, user_id, donation_id, date, message, amount, name) ")
                .Append("VALUES (@FundraiserId, @UserId, @DonationId, @Date, @Message, @Amount, @Name) ")
                .ToString();

            createdResult = await _postgresClient.ExecuteAsync(query, parameters);

            // If we created 1 new fundraiser then we succeeded
            return (createdResult == 1);
        }
    }
}
