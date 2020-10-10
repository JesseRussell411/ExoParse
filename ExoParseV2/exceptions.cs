using System;
using System.Collections.Generic;
using System.Text;
using ExoParseV2.elements;

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
    class NotRedefinableException : ExecutionException
    {
        public IElement Item { get; }
        public NotRedefinableException(IElement item) : base($"{item} is not redefinable.")
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