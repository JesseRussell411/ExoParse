﻿using System;
using System.Collections.Generic;
using System.Text;
using ExoParseV2.elements;
using ExoParseV2.theUniverse.Commands;

namespace ExoParseV2
{
    public class MessageException : Exception
    {
        public MessageException(string message) : base(message)
        {
        }
    }
    public class ParsingException : MessageException
    {
        public ParsingException(string message) : base(message) { }
    }
    public class ExecutionException : MessageException
    {
        public ExecutionException(string message) : base(message) { }
    }
    public class NotRedefinableException : ExecutionException
    {
        public IElement Item { get; }
        public NotRedefinableException(IElement item) : base($"{item} is not redefinable.")
        {
            Item = item;
        }
    }
    public class GenericCommandException : MessageException
    {
        public GenericCommandException(string message)
            : base(message) { }
    }
}