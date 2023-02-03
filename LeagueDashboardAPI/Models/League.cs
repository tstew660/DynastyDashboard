using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueDashboardAPI.Models
{
    public class League
    {
        public int total_rosters { get; set; }
        public string status { get; set; }
        public string sport { get; set; }
        public string season_type { get; set; }
        public string season { get; set; }
        public string previous_league_id { get; set; }
        public string name { get; set; }
        public string league_id { get; set; }
        public string draft_id { get; set; }
        public string avatar { get; set; }

        public object settings { get; set; }
        public string[] roster_positions { get; set; }
        public List<Roster> rosters { get; set; }

    }
        

}
