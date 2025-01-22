using Aplicacion.Dominio.Entidades.Usuario.Errores;
using Aplicacion.DTOs;
using Aplicacion.Helper.Servicios;
using Aplicacion.Infraestructura.Persistencia;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Throw;
using UsuarioDominio = Aplicacion.Dominio.Entidades.Usuario.Usuario;

namespace Aplicacion.Caracteristicas.Autenticacion
{
    public class IniciarSesionConJWT
    {
        public record Consulta():IRequest<UsuarioDTO>;
        public class Hanlder : IRequestHandler<Consulta, UsuarioDTO>
        {
            private readonly IMapper mapper;
            private readonly ContextoDB contextoDB;
            private readonly IServicioUsuarioActual usuarioActual;

            public Hanlder(IMapper mapper, ContextoDB contextoDB, IServicioUsuarioActual usuarioActual)
            {
                this.mapper = mapper;
                this.contextoDB = contextoDB;
                this.usuarioActual = usuarioActual;
            }
            public async Task<UsuarioDTO> Handle(Consulta request, CancellationToken cancellationToken)
            {
                var usuario = await contextoDB.Usuario.FindAsync(int.Parse(usuarioActual.Id));
                usuario.ThrowIfNull(() => new ErroresUsuario.CredencialesIncorrectas());
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
