using System.Runtime.Serialization;

namespace SWEN_KOMP.Exceptions
{
    [Serializable]
    internal class UserNotAuthenticatedException : Exception
    {
        public UserNotAuthenticatedException()
        {
        }

        public UserNotAuthenticatedException(string? message) : base(message)
        {
        }

        public UserNotAuthenticatedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserNotAuthenticatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}