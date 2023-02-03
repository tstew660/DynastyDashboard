using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueDashboardAPI.Models
{
    public class User
    {
        public string username { get; set; }
        public string user_id { get; set; }
        public string display_name { get; set; }
        public List<League> leagues { get; set; }
        public Metadata metadata { get; set; }

    }

    public class Metadata
    {
        public string team_name { get; set; }
    }

}
