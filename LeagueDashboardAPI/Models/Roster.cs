using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueDashboardAPI.Models
{
    public class Roster
    {
        public string[] starters { get; set; }
        public List<Player> StartersList { get; set; }
        public Settings settings { get; set; }
        public int roster_id { get; set; }
        public object[] reserve { get; set; }
        public string[] players { get; set; }
        public List<Player> PlayersList { get; set; }
        public string owner_id { get; set; }
        public string league_id { get; set; }
        public User user { get; set; }
        public int ktc_total_sf { get; set; }
        public int ktc_total_oneQB { get; set; }
        public int fp_total_sf { get; set; }
        public int fp_total_oneQB { get; set; }
    }

    public class Settings
    {
        public int wins { get; set; }
        public int waiver_position { get; set; }
        public int waiver_budget_used { get; set; }
        public int total_moves { get; set; }
        public int ties { get; set; }
        public int losses { get; set; }
        public int fpts_decimal { get; set; }
        public int fpts_against_decimal { get; set; }
        public int fpts_against { get; set; }
        public int fpts { get; set; }
    }

}
