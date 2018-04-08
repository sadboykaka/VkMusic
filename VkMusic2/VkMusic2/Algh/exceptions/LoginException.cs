using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Algh.exceptions
{
    [System.Serializable]
    public class LoginException : Exception
    {
        public LoginException() { }

        public LoginException(string message) : base(message) { }

        public LoginException(string message, Exception inner) : base(message, inner) { }

        protected LoginException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
