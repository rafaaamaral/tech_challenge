using Microsoft.AspNetCore.Mvc;
using tech_challenge.API.Requests.Usuarios;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Usuarios;

namespace tech_challenge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly IUsuarioService _usuarioService;

        public AuthController(TokenService tokenService, IUsuarioService usuarioService)
        {
            _tokenService = tokenService;
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var usuario = await _usuarioService.ObterPorEmailAsync(request.Login);

            if (usuario is null)
                return Unauthorized("Usuário ou senha inválidos.");

            var senhaValida = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha);

            if (!senhaValida)
                return Unauthorized("Usuário ou senha inválidos.");

            var token = _tokenService.GerarToken(usuario);

            return Ok(new
            {
                accessToken = token
            });
        }
    }
}
