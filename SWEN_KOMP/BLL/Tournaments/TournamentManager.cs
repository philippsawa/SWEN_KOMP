using SWEN_KOMP.DAL.Scores;
using SWEN_KOMP.DAL.Tournaments;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SWEN_KOMP.BLL.Tournaments
{
    internal class TournamentManager : ITournamentManager
    {
        private readonly ITournamentDao _tournamentDao;

        private ConcurrentDictionary<string, Timer> _tournamentTimers = new ConcurrentDictionary<string, Timer>();

        public TournamentManager(ITournamentDao tournamentDao)
        {
            _tournamentDao = tournamentDao;
        }

        public void StartTournament(HistorySchema entry, string tournamentId)
        {
            if (string.IsNullOrWhiteSpace(tournamentId))
            {
                Console.WriteLine("Tournament ID cannot be null or empty.");
                throw new ArgumentException("Tournament ID cannot be null or empty.", nameof(tournamentId));
            }

            if (_tournamentTimers.ContainsKey(tournamentId))
            {
                _tournamentDao.AddHistoryEntry(entry, tournamentId);
            }
            else
            {
                var timer = new Timer(TournamentTimerCallback, tournamentId, 10000, Timeout.Infinite);
                if (_tournamentTimers.TryAdd(tournamentId, timer))
                {
                    Console.WriteLine($"Tournament {tournamentId} started.");
                    _tournamentDao.AddHistoryEntry(entry, tournamentId);
                }
                else
                {
                    Console.WriteLine($"Failed to start tournament {tournamentId}. Please try again.");
                    throw new NoTournamentException();
                }
            }
        }
        private void TournamentTimerCallback(object state)
        {
            var tournamentId = (string)state;
            Console.WriteLine($"Tournament {tournamentId} has ended.");

            if (_tournamentTimers.TryRemove(tournamentId, out var timer))
            {
                timer.Dispose();
                // ELO UPDATE EVERY PARTICIPANTS USER 
                _tournamentDao.DeleteTournamentName(tournamentId);
                Console.WriteLine($"Tournament {tournamentId} cleaned up.");
            }
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

        public List<HistorySchema> RetrieveSummedUpUserEntriesInTournament(string tournamentId)
        {
            List<HistorySchema> historyEntries = _tournamentDao.RetrieveTournament(tournamentId);
            List<HistorySchema> summedUpUserEntries = new List<HistorySchema>();
            List<string> usersChecked = new List<string>();

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
                }
            }

            return summedUpUserEntries;
        } 

        public TournamentInfoSchema GetTournamentInfo(string username)
        {
            string? tournamentName = _tournamentDao.GetActiveTournament(username);

            if(tournamentName == null)
            {
                throw new NoTournamentException();
            }

            List<HistorySchema> summedUpEntries = RetrieveSummedUpUserEntriesInTournament(tournamentName);

            HistorySchema? userWithHighestCount = null;

            foreach (var entry in summedUpEntries)
            {
                if (userWithHighestCount == null || entry.Count > userWithHighestCount.Count)
                {
                    userWithHighestCount = entry;
                }
            }

            int participantCount = summedUpEntries.Count;
            string leaderUsername = userWithHighestCount != null ? userWithHighestCount.Username : "";

            return new TournamentInfoSchema(participantCount, leaderUsername, 60); // STARTTIME UMÄNDERN VON HARDCODE AUF DYNAMIC !!!! 
        }
    }
}
