using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.Models.Schemas
{
    internal class UserStatsSchema
    {
        public string Name { get; set; }
        public int Elo {  get; set; }
        public int OverallPushups { get; set; }

        public UserStatsSchema(string name, int elo, int overallPushups)
        {
            Name = name;
            Elo = elo;
            OverallPushups = overallPushups;
        }
    }
}
