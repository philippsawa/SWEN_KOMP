using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.DAL.Tournaments
{
    internal interface ITournamentDao
    {
        List<HistorySchema> RetrieveHistory(string username);
    }
}
