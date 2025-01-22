using Aplicacion.Helper.Comunes.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Dominio.Comunes
{
    public abstract class EntidadBase: IEntidad
    {
        public int Id { get; set; }
        public bool Inactivo { get; set; } = false;
        public bool Eliminado { get; set; }

        private readonly List<IEventoDominio> eventos = new();

        public IReadOnlyCollection<IEventoDominio> ObtenerEventosDominios() => this.eventos.AsReadOnly();

        public void LimpiarEventosDominios() => this.eventos.Clear();

        protected void AgregarEventoDominio(IEventoDominio eventoDominio) => this.eventos.Add(eventoDominio);
    }
}
