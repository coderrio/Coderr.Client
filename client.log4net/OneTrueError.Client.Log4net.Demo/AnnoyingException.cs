using System;
using System.Runtime.Serialization;

namespace OneTrueError.Client.Log4net.Demo
{
    [Serializable]
    public class AnnoyingException : Exception
    {
        public AnnoyingException(string message) : base(message)
        {
        }

        public AnnoyingException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AnnoyingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}