using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.BLL.Tournaments
{
    internal interface ITournamentManager
    {
        List<HistorySchema> GetHistory(string username);
        TournamentInfoSchema GetTournamentInfo(string username);
    }
}
