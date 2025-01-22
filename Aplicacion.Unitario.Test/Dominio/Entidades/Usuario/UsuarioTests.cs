using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsuarioDominio = Aplicacion.Dominio.Entidades.Usuario.Usuario;

namespace Aplicacion.Unitario.Test.Dominio.Entidades.Usuario
{
    public class UsuarioTests
    {
        [Fact]
        public void CrearUsuario_RetornaUsuario()
        {
            var nombre = "Pepe Mujica";
            var email = "pepe@gmail.com";
            var password = "123123123";

            var sut = UsuarioDominio.Crear(nombre, email, password);
            Assert.Equal(sut.NombreCompleto, nombre);
            Assert.Equal(sut.Email, email);
            Assert.Equal(sut.Password, password);
        }
    }
}
