using Aplicacion.Dominio.Entidades.Usuario;
using Aplicacion.Infraestructura.Persistencia.Comunes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Infraestructura.Persistencia.Configuracion
{
    public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> entity)
        {
            AuditableConfiguracion.Configurar(entity);

            entity.Property(a => a.NombreCompleto)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(a => a.Email)
                .HasMaxLength(360)
                .IsRequired();

            entity.Property(x => x.Password)
                .HasMaxLength(2048)
                .IsRequired();
        }

    }
}
