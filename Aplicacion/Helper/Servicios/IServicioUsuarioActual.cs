using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Helper.Servicios
{
    public interface IServicioUsuarioActual
    {
        public string Id { get; }
        public string Email { get; }
    }
}
