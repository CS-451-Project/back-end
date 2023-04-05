using Dapper;
using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.DataAccess.Responses;
using GivingCircle.Api.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public class FundraiserPictureRepository : IFundraiserPictureRepository
    {
        private readonly PostgresClient _postgresClient;

        private readonly string _tableName = "fundraiser_pictures";

        /// <summary>
        /// Initializes an instance of the <see cref="FundraiserPictureRepository"/> class
        /// </summary>
        /// <param name="postgresClient">The postgres client</param>
        public FundraiserPictureRepository(PostgresClient postgresClient)
        {
            _postgresClient = postgresClient;
        }

        public async Task<bool> AddFundraiserPicture(string userId, FundraiserPicture fundraiserPicture)
        {
            // The string builder
            StringBuilder queryBuilder = new();

            // Parameters dictionary
            Dictionary<string, object> parameters = new()
            {
                { "@PictureId",  fundraiserPicture.PictureId },
                { "@FundraiserId",  fundraiserPicture.FundraiserId },
                { "@UserId", userId},
                { "@PictureUrl", fundraiserPicture.PictureUrl },
            };

            // This represents the number of rows effected by our query
            int createdResult;

            // Construct the query
            var query = queryBuilder
                .Append($"INSERT INTO {_tableName} ")
                .Append("(fundraiser_id, picture_id, user_id, picture_url) ")
                .Append("VALUES (@FundraiserId, @PictureId, @UserId, @PictureUrl)")
                .ToString();

            createdResult = await _postgresClient.ExecuteAsync(query, parameters);

            // If we created 1 new fundraiser picture then we succeeded
            return (createdResult == 1);
        }

        public async Task<bool> FundraiserPictureExists(string fundraiserId)
        {
            // The query parameters
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@FundraiserId", fundraiserId);

            // The query
            var query = $"SELECT * FROM {_tableName} WHERE fundraiser_id=@FundraiserId";

            try
            {
                // The response from the db
                var response = await _postgresClient.QuerySingleAsync<FundraiserPicture>(query, dynamicParameters);
            }
            // This exception will be thrown if there is nothing there
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            // No exception so there must be a picture there
            return true;
        }

        public async Task<string> GetFundraiserPictureUrl(string fundraiserPictureId)
        {
            // The fundraiser picture url to be returned
            string fundraiser;

            // The query string builder
            StringBuilder queryBuilder = new();

            // The parameters to be given to the query
            DynamicParameters parameters = new();

            parameters.Add("@FundraiserId", fundraiserPictureId);

            // Construct the query
            var query = queryBuilder
                .Append($"SELECT * FROM {_tableName} ")
                .Append("WHERE fundraiser_id=@FundraiserId ")
                .Append("AND closed_date IS NULL")
                .ToString();

            fundraiser = await _postgresClient.QuerySingleAsync<GetFundraiserResponse>(query, parameters);

            return fundraiser ?? null;
        }
    }
}
