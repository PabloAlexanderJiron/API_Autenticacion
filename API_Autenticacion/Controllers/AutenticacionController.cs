using API_Autenticacion.Controllers.Interfaces;
using Aplicacion.Caracteristicas.Autenticacion;
using Aplicacion.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Autenticacion.Controllers
{
    public class AutenticacionController : ApiBaseController
    {
        private readonly IConfiguration configuration;

        public AutenticacionController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpPost("registrarse")]
        public async Task<ActionResult> Registrarse(Registrarse.DatosRegistrarseDTO request)
        {
            var usuario = await Mediator.Send(new Registrarse.Comando(request));
            return Ok(usuario);
        }

        [HttpPost("iniciar-sesion")]
        public async Task<ActionResult> IniciarSesion(IniciarSesion.DatosInciarSesionDTO request)
        {
            var usuario = await Mediator.Send(new IniciarSesion.Comando(request));
            return Ok(new
            {
                usuario,
                JWT = GenerarJWT(usuario)
            });
        }

        [HttpGet("iniciar-sesion-jwt")]
        [Authorize]
        public async Task<ActionResult> IniciarSesionJWT()
        {
            var usuario = await Mediator.Send(new IniciarSesionConJWT.Consulta());
            return Ok(new
            {
                usuario,
                JWT = GenerarJWT(usuario)
            });
        }

        private string GenerarJWT(UsuarioDTO datos)
        {
            var claims = new[]
            {
        new Claim("id", datos.Id.ToString()),
        new Claim("email", datos.Email),
    };

            var secret = Environment.GetEnvironmentVariable("SECRETO_JWT");
            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("SECRETO_JWT no está configurado en AutenticacionController");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(5);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
