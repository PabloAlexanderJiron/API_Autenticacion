using Aplicacion.Helper.Comunes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Dominio.Comunes
{
    public abstract class EntidadAuditable: EntidadBase, IEntidadAuditable
    {
        public DateTimeOffset FechaCreacionLog { get; set; }
        public DateTimeOffset FechaModificacionLog { get; set; }
    }
}
