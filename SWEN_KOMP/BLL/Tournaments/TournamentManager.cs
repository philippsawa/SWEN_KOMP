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
using SWEN_KOMP.BLL.Scores;

namespace SWEN_KOMP.BLL.Tournaments
{
    internal class TournamentManager : ITournamentManager
    {
        private readonly ITournamentDao _tournamentDao;
        private readonly IScoreManager _scoreManager;

        private ConcurrentDictionary<string, Timer> _tournamentTimers = new ConcurrentDictionary<string, Timer>();

        public TournamentManager(ITournamentDao tournamentDao, IScoreManager scoreManager)
        {
            _tournamentDao = tournamentDao;
            _scoreManager = scoreManager;
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

            if (_tournamentTimers.TryRemove(tournamentId, out var timer))
            {
                timer.Dispose();
                ProcessUsersInTournament(tournamentId); // +-elo
                _tournamentDao.DeleteTournamentName(tournamentId);
            }

            Console.WriteLine($"Tournament {tournamentId} has ended.");
        }

        private void ProcessUsersInTournament(string tournamentId)
        {
            var summedUpUserEntries = RetrieveSummedUpUserEntriesInTournament(tournamentId);
            if (summedUpUserEntries.Count == 0) return;
            List<HistorySchema> topUsers = new List<HistorySchema>();

            topUsers.Add(summedUpUserEntries[0]);

            for (int i = 1; i < summedUpUserEntries.Count; i++)
            {
                var currentUser = summedUpUserEntries[i];

                if (currentUser.Count > topUsers[0].Count)
                {
                    // wenn höher -> liste clearen und neuen user als highest speichern
                    topUsers.Clear();
                    topUsers.Add(currentUser);
                }
                else if (currentUser.Count == topUsers[0].Count)
                {
                    // wenn gleicher score -> beide in liste
                    topUsers.Add(currentUser);
                }
            }

            var otherUsers = summedUpUserEntries.Except(topUsers).ToList();

            if (topUsers.Count == 1)
            {
                // 1 gewinner
                Console.WriteLine($"Winner: {topUsers[0].Username} with count: {topUsers[0].Count}");
                _scoreManager.AddElo(2, topUsers[0].Username + "-sebToken");
            }
            else
            {
                // >1 gewinner
                Console.WriteLine("Multiple winners with equal counts:");
                foreach (var user in topUsers)
                {
                    Console.WriteLine($"{user.Username} with count: {user.Count}");
                    _scoreManager.AddElo(1, user.Username + "-sebToken");
                }
            }

            // alle anderen user (verlierer)
            if (otherUsers.Any())
            {
                Console.WriteLine("Loser(s):");
                foreach (var user in otherUsers)
                {
                    Console.WriteLine($"{user.Username} with count: {user.Count}");
                    _scoreManager.SubtractElo(user.Username + "-sebToken");
                }
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
