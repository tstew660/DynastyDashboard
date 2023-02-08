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
    public class LeagueHelper
    {
        public readonly IMongoCollection<Player> _playersCollection;

        private readonly HttpClient _sleeperClient;


        public LeagueHelper(IOptions<SleeperDashboardDB> playersDatabaseSettings, IHttpClientFactory clientFactory)
        {
            _sleeperClient = clientFactory.CreateClient("SleeperAPI");

            var mongoClient = new MongoClient(
            playersDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                playersDatabaseSettings.Value.DatabaseName);

            _playersCollection = mongoDatabase.GetCollection<Player>(
                playersDatabaseSettings.Value.PlayersCollectionName);
        }

        public async Task<League> GetLeagueRostersAsync(string leagueId)
        {
            var league = new League();
            var rosters = new List<Roster>();
            var users = new List<User>();
            var picks = new List<Pick>();
            string rostersEndpoint = "league/" + leagueId + "/rosters/";
            string usersEndpoint = "league/" + leagueId + "/users/";
            string leagueEndpoint = "league/" + leagueId;
            string picksEndpoint = "league/" + leagueId + "/traded_picks/";
            using (HttpClient client = _sleeperClient)
            {
                var leagueResponse = await APIGetRequestAsync(leagueEndpoint, client);

                league = System.Text.Json.JsonSerializer.Deserialize<League>(leagueResponse);

                var rosterResponse = await APIGetRequestAsync(rostersEndpoint, client);

                rosters = System.Text.Json.JsonSerializer.Deserialize<List<Roster>>(rosterResponse);

                var userResponse = await APIGetRequestAsync(usersEndpoint, client);

                users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(userResponse);

                var tradedPicksResponse = await APIGetRequestAsync(picksEndpoint, client);

                picks = System.Text.Json.JsonSerializer.Deserialize<List<Pick>>(tradedPicksResponse);
            }

            league.rosters = rosters;
            await MapPlayersToRoster(rosters);
            await MapUsersToRoster(rosters, users);
            await MapPicksToRosters(rosters, picks, Convert.ToInt32(league.season));

            return league;

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

        private async Task MapPlayersToRoster(List<Roster> rosters)
        {
            List<string> fullPlayerList = new List<string>();
            foreach(var playerList in rosters)
            {
                foreach(var player in playerList.players)
                {
                    if(!fullPlayerList.Contains(player))
                    {
                        fullPlayerList.Add(player);
                    }
                    
                }
            }

            var activeStandardPlayers = await _playersCollection
                .Find(Builders<Player>.Filter.In(x => x.player_id, fullPlayerList))
                .Project<Player>(Builders<Player>.Projection.Exclude(x => x._id)).ToListAsync();
            
            foreach (var roster in rosters)
            {
                int ktc_sf_total = 0;
                int ktc_oneQB_total = 0;
                int fp_sf_total = 0;
                int fp_oneQB_total = 0;
                var players = new List<Player>();
                foreach (var player in roster.players)
                {
                    var playerFromDB = activeStandardPlayers.SingleOrDefault(x => x.player_id == player);
                    if(playerFromDB != null)
                    {
                        players.Add(playerFromDB);
                        if (playerFromDB.ktc_rank_sf != null && playerFromDB.ktc_rank_oneQB != null && playerFromDB.fantasy_pros_rank_sf != null && playerFromDB.fantasy_pros_rank_oneQB != null)
                        {
                            ktc_sf_total += (int)playerFromDB.ktc_rank_sf;
                            ktc_oneQB_total += (int)playerFromDB.ktc_rank_oneQB;
                            fp_sf_total += (int)playerFromDB.fantasy_pros_rank_sf;
                            fp_oneQB_total += (int)playerFromDB.fantasy_pros_rank_oneQB;
                        }
                    }  
                }
                roster.PlayersList = players;
                roster.ktc_total_sf = ktc_sf_total;
                roster.ktc_total_oneQB = ktc_oneQB_total;
                roster.fp_total_sf = fp_sf_total;
                roster.fp_total_oneQB = fp_oneQB_total;
            }
        }

        private async Task MapUsersToRoster(List<Roster> rosters, List<User> users)
        {
            foreach (var roster in rosters)
            {
                roster.user = users.SingleOrDefault(x => x.user_id == roster.owner_id);
                if(roster.user.metadata.team_name == null)
                {
                    roster.user.metadata.team_name = roster.user.display_name;
                }
            }
        }

        private async Task MapPicksToRosters(List<Roster> rosters, List<Pick> tradedPicks, int leagueYear)
        {
            for (int year = leagueYear + 1; year < leagueYear + 4; year++)
            {
                //set stock picks
                foreach (var roster in rosters)
                {
                    var stockPicks = new List<Pick>();
                    for (int i = 1; i <= 4; i++)
                    {
                        stockPicks.Add(new Pick
                        {
                            season = year.ToString(),
                            roster_id = roster.roster_id,
                            round = i
                        });
                    }
                    roster.picks = stockPicks; 
                }

                foreach (var pick in tradedPicks)
                {
                    rosters.SingleOrDefault(x => x.roster_id == pick.previous_owner_id).picks.Remove(pick);
                    rosters.SingleOrDefault(x => x.roster_id == pick.owner_id).picks.Add(pick);

                }
            }
        }
    }
}
