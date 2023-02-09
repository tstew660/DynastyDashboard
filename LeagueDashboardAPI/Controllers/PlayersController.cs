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
    public class PlayersController : ControllerBase
    {

        private readonly HttpClient sleeperClient;

        private readonly PlayersHelper _playersHelper;

        private readonly IMongoCollection<Player> _playersCollection;

        public PlayersController(IOptions<SleeperDashboardDB> playersDatabaseSettings, ILogger<PlayersController> logger, IHttpClientFactory clientFactory)
        {
            sleeperClient = clientFactory.CreateClient("SleeperAPI");

            var mongoClient = new MongoClient(
            playersDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                playersDatabaseSettings.Value.DatabaseName);

            _playersCollection = mongoDatabase.GetCollection<Player>(
                playersDatabaseSettings.Value.PlayersCollectionName);

            _playersHelper = new PlayersHelper(playersDatabaseSettings, clientFactory);
        }

        // GET api/<UserController>/5
        [HttpGet]
        [Route("GetAllPlayersAsync")]
        public async Task<PlayerResponse> GetAllPlayersAsync([FromQuery] PlayerParameters playerParameters )
        {
            return await _playersHelper.GetAllPlayersAsync(playerParameters);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        [Route("GetPlayerByIdAsync/{id}")]
        public async Task<Player> GetPlayerByIdAsync(string id)
        {
            return await _playersHelper.GetPlayerById(id);
        }
    }
}