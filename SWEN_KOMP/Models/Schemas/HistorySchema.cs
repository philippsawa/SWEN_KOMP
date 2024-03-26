using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.Models.Schemas
{
    internal class HistorySchema
    {
        public int Count { get; set; }
        public int Duration { get; set; }
        public string Username { get; set; }

        public HistorySchema(int count, int duration, string username)
        {
            Count = count;
            Duration = duration;
            Username = username;
        }
    }
}
