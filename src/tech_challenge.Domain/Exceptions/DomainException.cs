using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace tech_challenge.Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
