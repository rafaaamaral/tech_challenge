using tech_challenge.Application.Interfaces.Services;
using System.Security.Claims;
using tech_challenge.Application.Common.Interfaces;

namespace tech_challenge.API.Services
{
    public class UsuarioLogadoService : IUsuarioLogadoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioLogadoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UniqueCode
        {
            get
            {
                var userId = _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                return Guid.TryParse(userId, out var id) ? id : null;
            }
        }
        public string? Nome =>
            _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.Name)?
                .Value;

        public string? Email =>
            _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.Email)?
                .Value;

        public string? Perfil =>
            _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.Role)?
                .Value;
    }
}
