using Aplicacion.Dominio.Comunes;
using Aplicacion.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Dominio.Entidades.Usuario
{
    public class Usuario: EntidadAuditable
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public static Usuario Crear(string nombreCompleto, string email, string password)
        {
            return new Usuario
            {
                NombreCompleto = nombreCompleto.Trim(),
                Email = email.Trim().ToLower(),
                Password = password
            };
        }
    }
}
