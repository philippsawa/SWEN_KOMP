using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.Exceptions
{
    [Serializable]
    internal class NoTournamentException : Exception
    {
        public NoTournamentException()
        {
        }

        public NoTournamentException(string? message) : base(message)
        {
        }

        public NoTournamentException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoTournamentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
