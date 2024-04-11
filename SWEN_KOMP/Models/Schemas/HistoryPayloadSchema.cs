using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.Models.Schemas
{
    internal class HistoryPayloadSchema
    {
        public string Name {  get; set; }
        public int Count { get; set; }
        public int DurationInSeconds { get; set; }

        public HistoryPayloadSchema(string tournamentName, int count, int durationInSeconds)
        {
            Name = tournamentName;
            Count = count;
            DurationInSeconds = durationInSeconds;
        }
    }
}
