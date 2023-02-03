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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LeagueDashboardAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly HttpClient sleeperClient;

        private readonly UserHelper _userHelper;

        private readonly IMongoCollection<Player> _playersCollection;

        public UserController(IOptions<SleeperDashboardDB> playersDatabaseSettings, ILogger<UserController> logger, IHttpClientFactory clientFactory)
        {
            sleeperClient = clientFactory.CreateClient("SleeperAPI");

            var mongoClient = new MongoClient(
            playersDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                playersDatabaseSettings.Value.DatabaseName);

            _playersCollection = mongoDatabase.GetCollection<Player>(
                playersDatabaseSettings.Value.PlayersCollectionName);

            _userHelper = new UserHelper(playersDatabaseSettings, clientFactory);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<User> GetAsync(string id)
        {
            return await _userHelper.GetUserModelAsync(id);
        }
    }
}
