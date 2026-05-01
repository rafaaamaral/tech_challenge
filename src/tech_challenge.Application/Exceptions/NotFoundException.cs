using System;
using System.Collections.Generic;
using System.Text;

namespace tech_challenge.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"{name} ({key}) não foi encontrado.")
        {
        }
    }
}
