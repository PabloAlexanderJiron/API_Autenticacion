using Aplicacion.Dominio.Comunes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Infraestructura.Persistencia.Comunes
{
   public static class AuditableConfiguracion
    {
        public static void Configurar<TEntity>(EntityTypeBuilder<TEntity> entity)
        where TEntity : EntidadAuditable
        {
            entity.HasKey(e => e.Id);

            entity.Property(
                    e => e.FechaCreacionLog)
                .IsRequired();

            entity.Property(
                    e => e.FechaModificacionLog)
                .IsRequired();
        }
    }
}
