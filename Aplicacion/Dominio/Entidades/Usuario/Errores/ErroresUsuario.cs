using Aplicacion.Helper.Comunes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Dominio.Entidades.Usuario.Errores
{
    public class ErroresUsuario
    {
        public class EmailExistente : ExcepcionDominio
        {
            public EmailExistente() : base("El email ya se encuentra registrado!")
            {
            }
        }
        public class CredencialesIncorrectas: ExcepcionDominio
        {
            public CredencialesIncorrectas():base("Credenciales incorrectas!")
            {
            }
        }
    }
}
