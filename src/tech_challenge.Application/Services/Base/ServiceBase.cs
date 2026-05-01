using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace tech_challenge.Application.Services.Base
{
    [ExcludeFromCodeCoverage]
    public abstract class ServiceBase<T>
    {
        protected readonly ILogger<T> _logger;

        public ServiceBase(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}
