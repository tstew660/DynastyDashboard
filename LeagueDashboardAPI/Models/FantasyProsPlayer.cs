using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueDashboardAPI.Models
{
    public class FantasyProsPlayer
    {
        public string player_team_id { get; set; }
        public string player_name { get; set; }
        public string player_position_id { get; set; }
        public string rank_ave { get; set; }
    }
}
