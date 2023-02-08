using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueDashboardAPI.Models
{
    public class Pick
    {
        public string season { get; set; }
        public int round { get; set; }
        public int roster_id { get; set; }
        public int previous_owner_id { get; set; }
        public int owner_id { get; set; }
    }

}
