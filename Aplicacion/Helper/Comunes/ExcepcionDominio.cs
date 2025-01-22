using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Helper.Comunes
{
    public class ExcepcionDominio: Exception
    {
        public ExcepcionDominio(string mensaje) : base(mensaje) { }
    }
}
