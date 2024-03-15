using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.BLL.Scores
{
    internal interface IScoreManager
    {
        UserStatsSchema GetSpecificUserStats(string authToken);
        void InsertUserStats(string token);

        List<UserStatsSchema> GetScoreboard();
    }
}
