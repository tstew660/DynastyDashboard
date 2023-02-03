using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LeagueDashboardAPI.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LeagueDashboardAPI.Helpers;

namespace LeagueDashboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScraperController : ControllerBase
    {

        private readonly HttpClient sleeperClient;

        private readonly ILogger<UserController> _logger;

        private readonly IMongoCollection<Player> _playersCollection;

        private readonly ScraperHelper _scraperHelper;

        public ScraperController(ILogger<UserController> logger, IHttpClientFactory clientFactory, IOptions<SleeperDashboardDB> playersDatabaseSettings)
        {
            _logger = logger;
            sleeperClient = clientFactory.CreateClient("SleeperAPI");
            

            var mongoClient = new MongoClient(
            playersDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                playersDatabaseSettings.Value.DatabaseName);

            _playersCollection = mongoDatabase.GetCollection<Player>(
                playersDatabaseSettings.Value.PlayersCollectionName);

            _scraperHelper = new ScraperHelper(playersDatabaseSettings);
        }

        [Route("PutPlayersFromSleeper")]
        [HttpPut]
        public async Task PutPlayersFromSleeperAsync()
        {
            using (HttpClient client = sleeperClient)
            {
                var playerList = new List<Player>();
                string endpoint = "players/nfl";
                using (var Response = await client.GetAsync(endpoint))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var playerListObject = await Response.Content.ReadAsStringAsync();
                        JObject jObject = JsonConvert.DeserializeObject<JObject>(playerListObject.ToLower());
                        List<JProperty> jProperties = jObject.Properties().ToList();
                        var list = jProperties.Values();
                        var serialized = JsonConvert.SerializeObject(list);
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        };
                        playerList = JsonConvert.DeserializeObject<List<Player>>(serialized, settings);
                        var bulkOps = new List<WriteModel<Player>>();
                        foreach (var player in playerList)
                        {
                            var upsertOne = new ReplaceOneModel<Player>(
                                Builders<Player>.Filter.Where(x => x.player_id == player.player_id),
                                player)
                            { IsUpsert = true };
                            bulkOps.Add(upsertOne);
                        }
                        await _playersCollection.BulkWriteAsync(bulkOps);
                        //await _playersCollection.InsertManyAsync(playerList);
                    }
                    else
                    {
                        throw new HttpRequestException(Response.ReasonPhrase);
                    }
                }
            }
        }

        [Route("ScrapeKTC")]
        [HttpPut]
        public async Task<List<string>> ScrapeKTCAsync()
        {
            var sfURL = "https://keeptradecut.com/dynasty-rankings?format=2";
            var oneQBURL = "https://keeptradecut.com/dynasty-rankings?format=1";
            HttpClient client = new HttpClient();
            var responseSF = await client.GetStringAsync(sfURL);
            var responseOneQB = await client.GetStringAsync(oneQBURL);
            return await _scraperHelper.ScrapeForRankings(responseSF, responseOneQB, "KTC");

        }

        [Route("ScrapeFantasyPros")]
        [HttpPut]
        public async Task<List<string>> ScrapeFantasyProsAsync()
        {
            var sfURL = "https://www.fantasypros.com/nfl/rankings/superflex-cheatsheets.php";
            var oneQBURL = "https://www.fantasypros.com/nfl/rankings/consensus-cheatsheets.php";
            HttpClient client = new HttpClient();
            var responseSF = await client.GetStringAsync(sfURL);
            var responseOneQB = await client.GetStringAsync(oneQBURL);
            return await _scraperHelper.ScrapeForRankings(responseSF, responseOneQB, "FP");

        }


    }
}
