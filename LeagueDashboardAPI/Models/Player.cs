using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace LeagueDashboardAPI.Models
{

    public class Player
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId _id { get; set; }
        public string hashtag { get; set; }
        public string status { get; set; }
        public string sport { get; set; }
        public string[] fantasy_positions { get; set; }
        public int? number { get; set; }
        public string search_last_name { get; set; }
        public object injury_start_date { get; set; }
        public string weight { get; set; }
        public string position { get; set; }
        public object practice_participation { get; set; }
        public string sportradar_id { get; set; }
        public string team { get; set; }
        public string last_name { get; set; }
        public string college { get; set; }
        public int? fantasy_data_id { get; set; }
        public object injury_status { get; set; }
        public string player_id { get; set; }
        public string height { get; set; }
        public string search_full_name { get; set; }
        public int? age { get; set; }
        public string stats_id { get; set; }
        public string birth_country { get; set; }
        public object espn_id { get; set; }
        public int? search_rank { get; set; }
        public string first_name { get; set; }
        public int? years_exp { get; set; }
        public object rotowire_id { get; set; }
        public object rotoworld_id { get; set; }
        public string search_first_name { get; set; }
        public object yahoo_id { get; set; }
        public int? ktc_rank_oneQB { get; set; }
        public int? ktc_rank_sf { get; set; }
        public double? fantasy_pros_rank_sf { get; set; }
        public double? fantasy_pros_rank_oneQB { get; set; }
    }

}
