using System.Runtime.Serialization;

namespace SWEN_KOMP.Exceptions
{
    [Serializable]
    internal class EmptyHistoryException : Exception
    {
        public EmptyHistoryException()
        {
        }

        public EmptyHistoryException(string? message) : base(message)
        {
        }

        public EmptyHistoryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EmptyHistoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}