using Aplicacion.Dominio.Comunes;
using Aplicacion.Helper.Servicios;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Infraestructura.Persistencia.Comunes
{
    public class InterceptorEntidadAuditable : SaveChangesInterceptor
    {
        private readonly IServicioUsuarioActual usuarioActual;

        public InterceptorEntidadAuditable(IServicioUsuarioActual usuarioActual)
        {
            this.usuarioActual = usuarioActual;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            GestionarAuditoria(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            GestionarAuditoria(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void GestionarAuditoria(DbContext? contexto)
        {
            if (contexto is null) return;
            foreach (var item in contexto.ChangeTracker.Entries<EntidadAuditable>())
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        item.Entity.FechaCreacionLog = DateTimeOffset.Now;
                        item.Entity.FechaModificacionLog = DateTimeOffset.Now;
                        break;
                    case EntityState.Modified:
                        item.Entity.FechaModificacionLog = DateTimeOffset.Now;
                        break;
                }
            }
        }
    }
}
