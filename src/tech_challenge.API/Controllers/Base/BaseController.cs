using Microsoft.AspNetCore.Mvc;

namespace tech_challenge.API.Controllers.Base
{
    [ApiController]
    public abstract class BaseController<T> : ControllerBase
    {
        protected readonly ILogger<T> _logger;

        public BaseController(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}
