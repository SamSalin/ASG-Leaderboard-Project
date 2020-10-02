using System;
using System.Runtime.Serialization;

namespace ASG_Leaderboard_Project
{
    [Serializable]
    public class OutOfRangeException : Exception
    {
        public OutOfRangeException()
        {
        }

        public OutOfRangeException(string message) : base(message)
        {
        }

        public OutOfRangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}