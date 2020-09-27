using System;
using System.Collections.Generic;
using System.Text;

namespace ExoParsev1
{
    public class MessageException : Exception
    {
        public MessageException(string message) : base(message)
        {
        }
    }
}
