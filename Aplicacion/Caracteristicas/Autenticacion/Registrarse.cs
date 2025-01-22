using Aplicacion.Dominio.Entidades.Usuario.Errores;
using Aplicacion.DTOs;
using Aplicacion.Infraestructura.Persistencia;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsuarioDominio = Aplicacion.Dominio.Entidades.Usuario.Usuario;

namespace Aplicacion.Caracteristicas.Autenticacion
{
    public class Registrarse
    {
        public record DatosRegistrarseDTO(string? NombreCompleto, string? Email, string? Password);
        public record Comando(DatosRegistrarseDTO Datos): IRequest<UsuarioDTO>;
        public class ValidadorComando : AbstractValidator<Comando>
        {
            public ValidadorComando()
            {
                RuleFor(x => x.Datos.NombreCompleto).NotNull().NotEmpty().MaximumLength(100);
                RuleFor(x => x.Datos.Email).NotNull().EmailAddress().MaximumLength(360);
                RuleFor(x => x.Datos.Password).NotEmpty().MinimumLength(6).MaximumLength(30);
            }
        }
        public class Handler : IRequestHandler<Comando, UsuarioDTO>
        {
            private readonly ContextoDB contextoDB;
            private readonly IMapper mapper;

            public Handler(ContextoDB contextoDB, IMapper mapper)
            {
                this.contextoDB = contextoDB;
                this.mapper = mapper;
            }
            public async Task<UsuarioDTO> Handle(Comando request, CancellationToken cancellationToken)
            {
                var existeUsuario = await contextoDB.Usuario
                    .FirstOrDefaultAsync(x => x.Email.ToLower() == request.Datos.Email!.ToLower().Trim());
                if (existeUsuario != null)
                    throw new ErroresUsuario.EmailExistente();
                var usuario = UsuarioDominio.Crear(
                    request.Datos.NombreCompleto!,
                    request.Datos.Email!,
                    BCrypt.Net.BCrypt.HashPassword(request.Datos.Password)
                );
                contextoDB.Usuario.Add(usuario);
                await contextoDB.SaveChangesAsync(cancellationToken);
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
