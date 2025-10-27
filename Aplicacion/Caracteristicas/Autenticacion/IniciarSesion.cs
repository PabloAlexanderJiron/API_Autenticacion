using Aplicacion.Dominio.Entidades.Usuario;
using Aplicacion.Dominio.Entidades.Usuario.Errores;
using Aplicacion.DTOs;
using Aplicacion.Infraestructura.Persistencia;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Throw;
using UsuarioDominio = Aplicacion.Dominio.Entidades.Usuario.Usuario;

namespace Aplicacion.Caracteristicas.Autenticacion
{
    public class IniciarSesion
    {
        public record DatosInciarSesionDTO(string? Email, string? Password);
        public record Comando(DatosInciarSesionDTO Datos):IRequest<UsuarioDTO>;
        public class ValidadorComando:AbstractValidator<Comando>
        {
            public ValidadorComando()
            {
                RuleFor(x => x.Datos.Email).NotNull().EmailAddress().MaximumLength(360);
                RuleFor(x => x.Datos.Password).NotEmpty().MinimumLength(6).MaximumLength(30);
            }
        }
        public class Hanlder : IRequestHandler<Comando, UsuarioDTO>
        {
            private readonly IMapper mapper;
            private readonly ContextoDB contextoDB;

            public Hanlder(IMapper mapper, ContextoDB contextoDB)
            {
                this.mapper = mapper;
                this.contextoDB = contextoDB;
            }
            public async Task<UsuarioDTO> Handle(Comando request, CancellationToken cancellationToken)
            {
                // metodo para validar si usuario y contraseña son correctos
                var usuario = await contextoDB.Usuario.FirstOrDefaultAsync(x => x.Email == request.Datos.Email!.Trim().ToLower());
                usuario.ThrowIfNull(() => new ErroresUsuario.CredencialesIncorrectas());
                usuario.Throw(()=> new ErroresUsuario.CredencialesIncorrectas())
                    .IfFalse(x => BCrypt.Net.BCrypt.Verify(request.Datos.Password, x.Password));
                return mapper.Map<UsuarioDTO>(usuario);
            }
            public class MapRespuesta : Profile
            {
                public MapRespuesta()
                {
                    CreateMap<UsuarioDominio, UsuarioDTO>();
                }
            }
        }
    }
}
