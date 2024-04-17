using SWEN_KOMP.DAL.Scores;
using SWEN_KOMP.Exceptions;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.BLL.Scores
{
    // user stats und scores methoden
    internal class ScoreManager : IScoreManager
    {
        private readonly IScoreDao _scoreDao;

        // Konstruktor init
        public ScoreManager(IScoreDao scoreDao)
        {
            _scoreDao = scoreDao;
        }

        // stats für neuen user
        public void InsertUserStats(string token)
        {
            _scoreDao.InsertUserStats(token);
        }

        // gesamtes scoreboard
        public List<UserStatsSchema> GetScoreboard()
        {
            return _scoreDao.GetScoreboard();
        }

        // spezifische stats eines users
        public UserStatsSchema GetSpecificUserStats(string token)
        {
            var stats = _scoreDao.GetUserStatsSchema(token);

            // keine stats --> ex
            if (stats == null)
            {
                throw new UserNotFoundException();
            }

            return stats;
        }

        // + elo
        public void AddElo(int amount, string authToken)
        {
            _scoreDao.AddElo(amount, authToken);
        }

        // - elo
        public void SubtractElo(string authToken)
        {
            _scoreDao.SubtractElo(authToken);
        }

        // + pushups
        public void AddPushUpCount(int amount, string authToken)
        {
            _scoreDao.AddPushUpCount(amount, authToken);
        }
    }
}
