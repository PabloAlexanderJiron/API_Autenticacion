using Aplicacion.Caracteristicas.Autenticacion;
using FluentValidation.TestHelper;

namespace Aplicacion.Unitario.Test.Caracteristicas.Autenticacion
{
    public class RegistrarseTests
    {
        [Fact]
        public async Task ValidarComando_DatosCorrectos()
        {
            var comando = new Registrarse.Comando(new Registrarse.DatosRegistrarseDTO("Pepe Mujica","pep@gmail.com","12345678"));

            var sut = await new Registrarse.ValidadorComando().TestValidateAsync(comando);
            sut.ShouldNotHaveValidationErrorFor(x => x.Datos.NombreCompleto);
            sut.ShouldNotHaveValidationErrorFor(x => x.Datos.Email);
            sut.ShouldNotHaveValidationErrorFor(x => x.Datos.Password);
        }

        [Fact]
        public async Task ValidarComando_DatosIncorrectos()
        {
            var comando = new Registrarse.Comando(new Registrarse.DatosRegistrarseDTO("", "wsewefc", ""));

            var sut = await new Registrarse.ValidadorComando().TestValidateAsync(comando);
            sut.ShouldHaveValidationErrorFor(x => x.Datos.NombreCompleto);
            sut.ShouldHaveValidationErrorFor(x => x.Datos.Email);
            sut.ShouldHaveValidationErrorFor(x => x.Datos.Password);
        }

    }
}
