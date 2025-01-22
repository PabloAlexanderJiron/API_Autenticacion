using Aplicacion.Caracteristicas.Autenticacion;
using Aplicacion.Dominio.Entidades.Usuario;
using Aplicacion.Dominio.Entidades.Usuario.Errores;
using Aplicacion.Integracion.Test.Comun;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Integracion.Test.Caracteristicas.Autenticacion
{
    [Collection(nameof(SliceFixture))]
    public class IniciarSesionTests
    {
        private readonly SliceFixture sliceFixture;

        public IniciarSesionTests(SliceFixture sliceFixture)
        {
            this.sliceFixture = sliceFixture;
        }

        [Fact]
        public async Task IniciarSesion_RetornaUsuario()
        {
            await sliceFixture.ResetCheckpoint();

            var nombre = "Pepe Mujica";
            var email = "pepe@gmail.com";
            var password = "Test123";

            await sliceFixture.SendAsync(new Registrarse.Comando(new Registrarse.DatosRegistrarseDTO(nombre, email, password)));

            var comando = new IniciarSesion.Comando(new IniciarSesion.DatosInciarSesionDTO(email, password));
            var sut = await sliceFixture.SendAsync(comando);
            sut.NombreCompleto.ShouldBe(nombre);
            sut.Email.ShouldBe(email);
        }

        [Fact]
        public async Task IniciarSesion_CredencialesIncorrectas()
        {
            await sliceFixture.ResetCheckpoint();

            var email = "pepe@gmail.com";
            var password = "Test123";

            var comando = new IniciarSesion.Comando(new IniciarSesion.DatosInciarSesionDTO(email, password));
            Should.Throw<ErroresUsuario.CredencialesIncorrectas>(async()=>
                await sliceFixture.SendAsync(comando)
            );
        }
    }
}
