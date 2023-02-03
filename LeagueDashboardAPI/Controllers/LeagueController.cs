using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using LeagueDashboardAPI.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.Json;
using LeagueDashboardAPI.Helpers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
namespace LeagueDashboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {

        private readonly HttpClient sleeperClient;

        private readonly LeagueHelper _leagueHelper;

        private readonly IMongoCollection<Player> _playersCollection;

        public LeagueController(IOptions<SleeperDashboardDB> playersDatabaseSettings, ILogger<UserController> logger, IHttpClientFactory clientFactory)
        {
            sleeperClient = clientFactory.CreateClient("SleeperAPI");

            var mongoClient = new MongoClient(
            playersDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                playersDatabaseSettings.Value.DatabaseName);

            _playersCollection = mongoDatabase.GetCollection<Player>(
                playersDatabaseSettings.Value.PlayersCollectionName);

            _leagueHelper = new LeagueHelper(playersDatabaseSettings, clientFactory);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<League> GetLeagueRostersAsync(string id)
        {
            return await _leagueHelper.GetLeagueRostersAsync(id);
        }
    }
}
