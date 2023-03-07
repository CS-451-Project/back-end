using Dapper;
using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public class DonationRepository : IDonationRepository
    {
        private readonly PostgresClient _postgresClient;

        private readonly string _tableName = "donations";

        public DonationRepository(PostgresClient postgresClient) 
        {
            _postgresClient = postgresClient;
        }
        public Task<IEnumerable<Donation>> GetFundraiserDonations(string fundraiserId)
        {
            throw new System.NotImplementedException();
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
            };

            // The parameters
            DynamicParameters parameters = new DynamicParameters(parametersDictionary);

            // This represents the number of rows effected by our query
            int createdResult;

            // Construct the query
            var query = queryBuilder
                .Append($"INSERT INTO {_tableName} ")
                .Append("(fundraiser_id, user_id, donation_id, date, message, amount) ")
                .Append("VALUES (@FundraiserId, @UserId, @DonationId, @Date, @Message, @Amount) ")
                .ToString();

            createdResult = await _postgresClient.ExecuteAsync(query, parameters);

            // If we created 1 new fundraiser then we succeeded
            return (createdResult == 1);
        }
    }
}
