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

        public UserStatsSchema GetSpecificUserStats(string token)
        {
            var stats = _scoreDao.GetUserStatsSchema(token);

            if(stats == null)
            {
                throw new UserNotFoundException();
            }

            return stats;
        }
    }
}
