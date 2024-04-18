using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.Models.Schemas
{
    internal class TournamentInfoSchema
    {
        public int ParticipantCount { get; set; }
        public string LeaderUsername { get; set; }
        public DateTime StartTime { get; set; }

        public TournamentInfoSchema(int participants, string leaderUsername, DateTime startTime)
        {
            ParticipantCount = participants;
            LeaderUsername = leaderUsername;
            StartTime = startTime;
        }
    }
}
