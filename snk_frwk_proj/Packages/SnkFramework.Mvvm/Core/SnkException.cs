using System;

namespace SnkFramework.Mvvm.Core
{
    public class SnkException : System.Exception
    {
        public SnkException()
        {
        }

        public SnkException(string message)
            : base(message)
        {
        }

        public SnkException(string messageFormat, params object?[] messageFormatArguments)
            : base(string.Format(messageFormat, messageFormatArguments))
        {
        }

        // the order of parameters here is slightly different to that normally expected in an exception
        // - but this order allows us to put string.Format in place
        public SnkException(Exception innerException, string messageFormat, params object?[] formatArguments)
            : base(string.Format(messageFormat, formatArguments), innerException)
        {
        }

        public SnkException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}