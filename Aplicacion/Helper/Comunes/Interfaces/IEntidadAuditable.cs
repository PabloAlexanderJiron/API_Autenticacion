using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Helper.Comunes.Interfaces
{
    public interface IEntidadAuditable
    {
        public DateTimeOffset FechaCreacionLog { get; set; }
        public DateTimeOffset FechaModificacionLog { get; set; }
    }
}
