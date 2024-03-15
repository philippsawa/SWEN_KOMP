using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.DAL.Scores
{
    internal interface IScoreDao
    {
        UserStatsSchema? GetUserStatsSchema(string sebToken);
        void InsertUserStats(string token);

        List<UserStatsSchema> GetScoreboard();
    }
}
