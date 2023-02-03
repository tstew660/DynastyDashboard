using LeagueDashboardAPI.Controllers;
using LeagueDashboardAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueDashboardAPI.Helpers
{
    public class UserHelper
    {
        public readonly IMongoCollection<Player> _playersCollection;

        private readonly HttpClient _sleeperClient;


        public UserHelper(IOptions<SleeperDashboardDB> playersDatabaseSettings, IHttpClientFactory clientFactory)
        {
            _sleeperClient = clientFactory.CreateClient("SleeperAPI");

            var mongoClient = new MongoClient(
            playersDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                playersDatabaseSettings.Value.DatabaseName);

            _playersCollection = mongoDatabase.GetCollection<Player>(
                playersDatabaseSettings.Value.PlayersCollectionName);
        }

        public async Task<User> GetUserModelAsync(string userName)
        {
            var user = new User();
            var leagues = new List<League>();
            string userEndpoint = "user/" + userName;
            using (HttpClient client = _sleeperClient)
            {
                var userResponse = await APIGetRequestAsync(userEndpoint, client);
                user = System.Text.Json.JsonSerializer.Deserialize<User>(userResponse);

                string leaguesEndpoint = "user/" + user.user_id + "/leagues/nfl/2022";
                var leaguesResponse = await APIGetRequestAsync(leaguesEndpoint, client);
                leagues = System.Text.Json.JsonSerializer.Deserialize<List<League>>(leaguesResponse);
            }
            user.leagues = leagues;

            return user;

        }

        private async Task<string> APIGetRequestAsync(string endpoint, HttpClient client)
        {
            using (var Response = await client.GetAsync(endpoint))
            {
                if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return await Response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new HttpRequestException(Response.ReasonPhrase);
                }
            }
        }
    }
}
