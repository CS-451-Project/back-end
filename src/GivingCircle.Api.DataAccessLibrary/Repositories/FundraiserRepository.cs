using GivingCircle.Api.DataAccess.Client;
using GivingCircle.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingCircle.Api.DataAccess.Repositories
{
    public class FundraiserRepository : IFundraiserRepository
    {
        private readonly PostgresClient<Fundraiser> _postgresClient;

        private readonly string _tableName = "fundraisers";

        public FundraiserRepository(PostgresClient<Fundraiser> postgresClient) 
        {
            _postgresClient = postgresClient;
        }

        public async Task<IEnumerable<Fundraiser>> ListAllFundraisersAsync()
        {
            return await _postgresClient.QueryAsync($"SELECT * FROM {_tableName}");
        }
    }
}
