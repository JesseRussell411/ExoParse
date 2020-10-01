using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParseV2
{
    public class MessageException : Exception
    {
        public MessageException(string message) : base(message)
        {
        }
    }
    class ParsingException : MessageException
    {
        public ParsingException(string message) : base(message) { }
    }
    class ExecutionException : MessageException
    {
        public ExecutionException(string message) : base(message) { }
    }
    class NotDefinableException : ExecutionException
    {
        public IElement Item { get; }
        public NotDefinableException(IElement item) : base($"{item} is not definable.")
        {
            Item = item;
        }
    }

    class GenericCommandException : MessageException
    {
        public GenericCommandException(string message)
            : base(message) { }
    }
}