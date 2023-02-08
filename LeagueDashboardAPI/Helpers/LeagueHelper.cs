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
            MapUsersToRoster(rosters, users);
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

        private void MapUsersToRoster(List<Roster> rosters, List<User> users)
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
            var picksFromDB = await _playersCollection
                .Find(x => x.position == "PICK").ToListAsync();

            //set stock picks
            foreach (var roster in rosters)
            {
                int rosterId = Convert.ToInt32(roster.roster_id);
                var stockPicks = new List<Pick>();
                for (int year = leagueYear + 1; year < leagueYear + 4; year++)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        stockPicks.Add(new Pick
                        {
                            season = year.ToString(),
                            roster_id = roster.roster_id,
                            owner_id = roster.roster_id,
                            round = i
                        });
                    }
                    
                    
                }
                stockPicks.AddRange(tradedPicks.Where(x => x.owner_id == rosterId && x.roster_id != rosterId && Convert.ToInt32(x.season) > leagueYear));
                foreach (var pick in tradedPicks)
                {
                    //if the traded pick belongs to this roster but is not owned by this roster
                    if(pick.owner_id != Convert.ToInt32(roster.roster_id) && pick.roster_id == Convert.ToInt32(roster.roster_id))
                    {
                        var removePick = stockPicks.SingleOrDefault(x => x.owner_id != pick.owner_id && x.roster_id == pick.roster_id && x.round == pick.round && x.season == pick.season);
                        stockPicks.Remove(removePick);
                    }
                }
                foreach (var stockPick in stockPicks)
                {
                    string convertedRound = ConvertRound(stockPick.round);
                    stockPick.rank_sf = (int)picksFromDB.SingleOrDefault(x => x.first_name == stockPick.season && x.last_name == convertedRound).ktc_rank_sf;
                    stockPick.rank_oneQB = (int)picksFromDB.SingleOrDefault(x => x.first_name == stockPick.season && x.last_name == convertedRound).ktc_rank_oneQB;
                }
                roster.picks = stockPicks.OrderBy(x => Convert.ToInt32(x.season)).ThenBy(x => x.round).ToList(); 
            }
        }

        private static string ConvertRound(int round) => round switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            4 => "4th",
            _ => "Can't Convert"
        };
    }
}
