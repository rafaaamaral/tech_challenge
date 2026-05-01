using System;
using System.Collections.Generic;
using System.Text;

namespace tech_challenge.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
