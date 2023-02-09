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
    public class PlayersHelper
    {
        public readonly IMongoCollection<Player> _playersCollection;

        private readonly HttpClient _sleeperClient;


        public PlayersHelper(IOptions<SleeperDashboardDB> playersDatabaseSettings, IHttpClientFactory clientFactory)
        {
            _sleeperClient = clientFactory.CreateClient("SleeperAPI");

            var mongoClient = new MongoClient(
            playersDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                playersDatabaseSettings.Value.DatabaseName);

            _playersCollection = mongoDatabase.GetCollection<Player>(
                playersDatabaseSettings.Value.PlayersCollectionName);
        }

        public async Task<PlayerResponse> GetAllPlayersAsync(PlayerParameters playerParameters)
        {
            var response = new PlayerResponse();
            response.playersSize = _playersCollection.CountDocuments(Builders<Player>.Filter
                .Ne(x => x.ktc_rank_sf, null)) / playerParameters.PageSize;

            response.players = await _playersCollection
                .Find(Builders<Player>.Filter
                .Ne(x => x.ktc_rank_sf, null))
                .SortByDescending(x => x.ktc_rank_sf)
                .Skip((playerParameters.PageNumber - 1) * playerParameters.PageSize)
                .Limit(playerParameters.PageSize)
                .ToListAsync();

            return response;
         }

        public async Task<Player> GetPlayerById(string id)
        {
            return await _playersCollection
                .Find(x => x.player_id == id).SingleAsync();
        }
    }

}
