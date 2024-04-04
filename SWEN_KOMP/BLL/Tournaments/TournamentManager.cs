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

        public TournamentInfoSchema GetTournamentInfo(string username)
        {
            string? tournamentName = _tournamentDao.GetActiveTournament(username);

            if(tournamentName == null)
            {
                throw new NoTournamentException();
            }

            List<HistorySchema> historyEntries = _tournamentDao.RetrieveTournament(tournamentName);
            List<HistorySchema> summedUpUserEntries = new List<HistorySchema>();
            List<string> usersChecked = new List<string>();

            HistorySchema? userWithHighestCount = null;

            foreach (var entry in historyEntries)
            {
                if (!usersChecked.Contains(entry.Username))
                {
                    usersChecked.Add(entry.Username);

                    int totalDuration = 0;
                    int totalCount = 0;

                    foreach (var innerEntry in historyEntries)
                    {
                        if (innerEntry.Username == entry.Username)
                        {
                            totalCount += innerEntry.Count;
                            totalDuration += innerEntry.Duration;
                        }
                    }

                    HistorySchema summedEntry = new HistorySchema(totalCount, totalDuration, entry.Username);
                    summedUpUserEntries.Add(summedEntry);

                    if (userWithHighestCount == null || totalCount > userWithHighestCount.Count)
                    {
                        userWithHighestCount = summedEntry;
                    }
                }
            }

            int participantCount = usersChecked.Count;
            string leaderUsername = userWithHighestCount != null ? userWithHighestCount.Username : "";

            return new TournamentInfoSchema(participantCount, leaderUsername, 60); // STARTTIME UMÄNDERN VON HARDCODE AUF DYNAMIC !!!! 
        }
    }
}
