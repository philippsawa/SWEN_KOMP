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
        public int StartTime { get; set; }

        public TournamentInfoSchema(int participants, string leaderUsername, int startTime)
        {
            ParticipantCount = participants;
            LeaderUsername = leaderUsername;
            StartTime = startTime;
        }
    }
}
