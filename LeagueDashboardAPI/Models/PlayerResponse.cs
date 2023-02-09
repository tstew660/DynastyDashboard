using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueDashboardAPI.Models
{
    public class PlayerResponse
    {
        public long playersSize { get; set; }
        public List<Player> players { get; set; }
    }
}
