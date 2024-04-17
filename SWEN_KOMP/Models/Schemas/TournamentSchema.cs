using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.Models.Schemas
{
    internal class TournamentSchema
    {
        public Timer Countdown { get; set; }
        public DateTime TournamentStart { get; set; }

        public TournamentSchema(Timer countdown)
        {
            Countdown = countdown;
            TournamentStart = DateTime.Now;
        }

        public void Dispose()
        {
            Countdown?.Dispose();
        }
    }
}
