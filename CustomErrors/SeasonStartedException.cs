using System;
using System.Runtime.Serialization;

namespace ASG_Leaderboard_Project
{
    [Serializable]
    public class SeasonStartedException : Exception
    {
        public SeasonStartedException()
        {
        }

        public SeasonStartedException(string message) : base(message)
        {
        }

        public SeasonStartedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SeasonStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}