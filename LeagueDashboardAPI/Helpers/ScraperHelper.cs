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
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace LeagueDashboardAPI.Helpers
{

    public class ScraperHelper
    {
        public readonly IMongoCollection<Player> _playersCollection;

        public ScraperHelper(IOptions<SleeperDashboardDB> playersDatabaseSettings)
        {

            var mongoClient = new MongoClient(
            playersDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                playersDatabaseSettings.Value.DatabaseName);

            _playersCollection = mongoDatabase.GetCollection<Player>(
                playersDatabaseSettings.Value.PlayersCollectionName);
        }

        public async Task<List<string>> ScrapeForRankings(string sfSite, string oneQBSite, string site)
        {
            var activeStandardPlayers = await _playersCollection
                .Find(x => x.position == "qb" || x.position == "rb" || x.position == "wr" || x.position == "te")
                .Project<Player>(Builders<Player>.Projection.Exclude(x => x._id)).ToListAsync();
            var unaddedPlayerList = new List<string>();
            switch (site)
            {
                case "KTC":
                    unaddedPlayerList.AddRange(await ScrapeKTCSF(sfSite, activeStandardPlayers));
                    unaddedPlayerList.AddRange(await ScrapeKTCOneQB(oneQBSite, activeStandardPlayers));
                    break;
                case "FP":
                    unaddedPlayerList.AddRange(await ScrapeFPSF(sfSite, activeStandardPlayers));
                    unaddedPlayerList.AddRange(await ScrapeFPOneQB(oneQBSite, activeStandardPlayers));
                    break;
                default:
                    break;
            }


            return unaddedPlayerList;
        }

        private async Task<List<string>> ScrapeKTCSF(string site, List<Player> activeStandardPlayers)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(site);

            var unaddedPlayerList = new List<string>();
            var rankings = htmlDoc.DocumentNode.SelectNodes("//div[@class='onePlayer']");
            foreach (var player in rankings)
            {
                var playerPosition = player.SelectSingleNode(".//div[@class='position-team']//p[@class='position']").InnerText;
                if (playerPosition != "PICK")
                {
                    var rankedPlayer = new Player();
                    var rank = Convert.ToInt32(player.SelectSingleNode(".//div[@class='single-ranking-wrapper']//div[@class='value']//p").InnerText);
                    var fullName = player.SelectSingleNode(".//div[@class='player-name']//a").InnerText;
                    var splitName = fullName.Split(' ', 3);
                    var firstName = CleanInput(splitName[0].ToLower());
                    var lastName = CleanInput(splitName[1].ToLower());
                    var playerAge = player.SelectSingleNode(".//div[@class='position-team']//p[@class='position hidden-xs']").InnerText;
                    var playerAgeClean = playerAge.Substring(0, 2);
                    var playerAgeInt = Convert.ToInt32(playerAgeClean);
                    var playerPositionClean = playerPosition.Substring(0, 2).ToLower();
                    var rankedPlayerList = activeStandardPlayers.Where(x => x.search_last_name == lastName && x.age == playerAgeInt && x.position == playerPositionClean).ToList();
                    if (rankedPlayerList.Count > 1)
                    {
                        rankedPlayer = rankedPlayerList.SingleOrDefault(x => x.search_first_name == firstName);
                        if (rankedPlayer == null)
                        {
                            unaddedPlayerList.Add(fullName);
                            continue;
                        }
                    }
                    else if (rankedPlayerList.Count == 1)
                        rankedPlayer = rankedPlayerList[0];
                    else
                    {
                        unaddedPlayerList.Add(fullName);
                        continue;
                    }
                    rankedPlayer.ktc_rank_sf = Convert.ToInt32(rank);
                    await _playersCollection.UpdateOneAsync(
                        x => x.player_id == rankedPlayer.player_id,
                        Builders<Player>.Update.Set(p => p.ktc_rank_sf, rank),
                        new UpdateOptions { IsUpsert = false }
                    );
                }

            }

            return unaddedPlayerList;

        }

        private async Task<List<string>> ScrapeKTCOneQB(string site, List<Player> activeStandardPlayers)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(site);

            var unaddedPlayerList = new List<string>();
            var rankings = htmlDoc.DocumentNode.SelectNodes("//div[@class='onePlayer']");
            foreach (var player in rankings)
            {
                var playerPosition = player.SelectSingleNode(".//div[@class='position-team']//p[@class='position']").InnerText;
                if (playerPosition != "PICK")
                {
                    var rankedPlayer = new Player();
                    var rank = Convert.ToInt32(player.SelectSingleNode(".//div[@class='single-ranking-wrapper']//div[@class='value']//p").InnerText);
                    var fullName = player.SelectSingleNode(".//div[@class='player-name']//a").InnerText;
                    var splitName = fullName.Split(' ', 3);
                    var firstName = CleanInput(splitName[0].ToLower());
                    var lastName = CleanInput(splitName[1].ToLower());
                    var playerAge = player.SelectSingleNode(".//div[@class='position-team']//p[@class='position hidden-xs']").InnerText;
                    var playerAgeClean = playerAge.Substring(0, 2);
                    var playerAgeInt = Convert.ToInt32(playerAgeClean);
                    var playerPositionClean = playerPosition.Substring(0, 2).ToLower();
                    var rankedPlayerList = activeStandardPlayers.Where(x => x.search_last_name == lastName && x.age == playerAgeInt && x.position == playerPositionClean).ToList();
                    if (rankedPlayerList.Count > 1)
                    {
                        rankedPlayer = rankedPlayerList.SingleOrDefault(x => x.search_first_name == firstName);
                        if (rankedPlayer == null)
                        {
                            unaddedPlayerList.Add(fullName);
                            continue;
                        }
                    }
                    else if (rankedPlayerList.Count == 1)
                        rankedPlayer = rankedPlayerList[0];
                    else
                    {
                        unaddedPlayerList.Add(fullName);
                        continue;
                    }
                    rankedPlayer.ktc_rank_oneQB = Convert.ToInt32(rank);
                    await _playersCollection.UpdateOneAsync(
                        x => x.player_id == rankedPlayer.player_id,
                        Builders<Player>.Update.Set(p => p.ktc_rank_oneQB, rank),
                        new UpdateOptions { IsUpsert = false }
                    );
                }
            }

            return unaddedPlayerList;

        }

        private async Task<List<string>> ScrapeFPSF(string site, List<Player> activeStandardPlayers)
        {
            int pFrom = site.IndexOf("\"players\":");
            int pTo = site.IndexOf(",\"experts_available\"") - 10;
            string playersJson = site.Substring(pFrom + 10, pTo - pFrom);

            var unaddedPlayerList = new List<string>();
            var playerJsonObject = JsonConvert.DeserializeObject<List<FantasyProsPlayer>>(playersJson);
            double maxRank = playerJsonObject.Count();
            foreach (var player in playerJsonObject)
            {
                var rankedPlayer = new Player();
                var splitName = player.player_name.Split(' ', 3);
                var firstName = splitName[0].ToLower();
                var lastName = splitName[1].ToLower();
                var playerTeam = player.player_team_id.ToLower();
                var position = player.player_position_id.ToLower();
                var rankedPlayerList = activeStandardPlayers.Where(x => x.first_name == firstName && x.last_name == lastName && x.position == position).ToList();
                if (rankedPlayerList.Count > 1)
                    rankedPlayer = rankedPlayerList.Single(x => x.team == playerTeam);
                else if (rankedPlayerList.Count == 1)
                    rankedPlayer = rankedPlayerList[0];
                else
                {
                    unaddedPlayerList.Add(firstName + " " + lastName);
                    continue;
                }
                var rank = Convert.ToInt32((maxRank - Convert.ToDouble(player.rank_ave)) * 25);
                rankedPlayer.fantasy_pros_rank_sf = rank;
                await _playersCollection.UpdateOneAsync(
                    x => x.player_id == rankedPlayer.player_id,
                    Builders<Player>.Update.Set(p => p.fantasy_pros_rank_sf, rankedPlayer.fantasy_pros_rank_sf),
                    new UpdateOptions { IsUpsert = false }
                );
            }

            return unaddedPlayerList;

        }

        private async Task<List<string>> ScrapeFPOneQB(string site, List<Player> activeStandardPlayers)
        {
            int pFrom = site.IndexOf("\"players\":");
            int pTo = site.IndexOf(",\"experts_available\"") - 10;
            string playersJson = site.Substring(pFrom + 10, pTo - pFrom);

            var unaddedPlayerList = new List<string>();
            var playerJsonObject = JsonConvert.DeserializeObject<List<FantasyProsPlayer>>(playersJson);
            double maxRank = playerJsonObject.Count();
            foreach (var player in playerJsonObject)
            {
                var rankedPlayer = new Player();
                var splitName = player.player_name.Split(' ', 3);
                var firstName = splitName[0].ToLower();
                var lastName = splitName[1].ToLower();
                var playerTeam = player.player_team_id.ToLower();
                var position = player.player_position_id.ToLower();
                var rankedPlayerList = activeStandardPlayers.Where(x => x.first_name == firstName && x.last_name == lastName && x.position == position).ToList();
                if (rankedPlayerList.Count > 1)
                    rankedPlayer = rankedPlayerList.Single(x => x.team == playerTeam);
                else if (rankedPlayerList.Count == 1)
                    rankedPlayer = rankedPlayerList[0];
                else
                {
                    unaddedPlayerList.Add(firstName + " " + lastName);
                    continue;
                }
                var rank = Convert.ToInt32((maxRank - Convert.ToDouble(player.rank_ave)) * 25);
                rankedPlayer.fantasy_pros_rank_oneQB = rank;
                await _playersCollection.UpdateOneAsync(
                    x => x.player_id == rankedPlayer.player_id,
                    Builders<Player>.Update.Set(p => p.fantasy_pros_rank_oneQB, rankedPlayer.fantasy_pros_rank_oneQB),
                    new UpdateOptions { IsUpsert = false }
                );
            }

            return unaddedPlayerList;

        }

        private static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
    }
}
