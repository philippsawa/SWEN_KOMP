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
    internal class ScoreManager : IScoreManager
    {
        private readonly IScoreDao _scoreDao;

        public ScoreManager(IScoreDao scoreDao)
        {
            _scoreDao = scoreDao;
        }

        public void InsertUserStats(string token)
        {
            _scoreDao.InsertUserStats(token);
        }

        public List<UserStatsSchema> GetScoreboard()
        {
            return _scoreDao.GetScoreboard();
        }

        public UserStatsSchema GetSpecificUserStats(string token)
        {
            var stats = _scoreDao.GetUserStatsSchema(token);

            if(stats == null)
            {
                throw new UserNotFoundException();
            }

            return stats;
        }

        public void AddElo(int amount, string authToken)
        {
            _scoreDao.AddElo(amount, authToken);

        }
        public void SubtractElo(string authToken)
        {
            _scoreDao.SubtractElo(authToken);
        }
        public void AddPushUpCount(int amount, string authToken)
        {
            _scoreDao.AddPushUpCount(amount, authToken);
        }
    }
}
