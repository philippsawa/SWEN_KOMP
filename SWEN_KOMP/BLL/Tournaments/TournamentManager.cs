using SWEN_KOMP.DAL.Scores;
using SWEN_KOMP.DAL.Tournaments;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.BLL.Tournaments
{
    internal class TournamentManager : ITournamentManager
    {
        private readonly ITournamentDao _tournamentDao;

        public TournamentManager(ITournamentDao tournamentDao)
        {
            _tournamentDao = tournamentDao;
        }

        public List<HistorySchema> GetHistory(string username)
        {
            List<HistorySchema> history = _tournamentDao.RetrieveHistory(username);

            if(history.Count == 0)
            {
                throw new EmptyHistoryException();
            }

            return history;
        }
    }
}
