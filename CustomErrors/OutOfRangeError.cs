using System;
using System.Runtime.Serialization;

namespace ASG_Leaderboard_Project
{
    [Serializable]
    public class OutOfRangeError : Exception
    {
        public OutOfRangeError()
        {
        }

        public OutOfRangeError(string message) : base(message)
        {
        }

        public OutOfRangeError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OutOfRangeError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}