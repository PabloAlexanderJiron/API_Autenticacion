using Aplicacion.Dominio.Entidades.Usuario;
using Aplicacion.Infraestructura.Persistencia.Comunes;
using Aplicacion.Infraestructura.Persistencia.Configuracion;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Infraestructura.Persistencia
{
    public partial class ContextoDB:DbContext
    {
        private readonly InterceptorEntidadAuditable interceptorEntidadAuditable;

        public ContextoDB() { }
        public ContextoDB(
            DbContextOptions<ContextoDB> options,
            InterceptorEntidadAuditable interceptorEntidadAuditable
            ) : base(options)
        {
            this.interceptorEntidadAuditable = interceptorEntidadAuditable;
        }

        public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioConfiguracion());

            OnModelCreatingPartial(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(interceptorEntidadAuditable);
            base.OnConfiguring(optionsBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
