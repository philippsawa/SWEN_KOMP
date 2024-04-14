using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.Models.Schemas
{
    internal class EloCheatSchema
    {
        public int EloAmountToAdd { get; set; }

        public EloCheatSchema(int eloAmount)
        {
            EloAmountToAdd = eloAmount;
        }
    }
}