using Aplicacion.Caracteristicas.Autenticacion;
using FluentValidation.TestHelper;

namespace Aplicacion.Unitario.Test.Caracteristicas.Autenticacion
{
    public class IniciarSesionTests
    {
        [Fact]
        public async Task ValidarComando_DatosCorrectos()
        {
            var comando = new IniciarSesion.Comando(new IniciarSesion.DatosInciarSesionDTO("pep@gmail.com", "12345678"));

            var sut = await new IniciarSesion.ValidadorComando().TestValidateAsync(comando);
            sut.ShouldNotHaveValidationErrorFor(x => x.Datos.Email);
            sut.ShouldNotHaveValidationErrorFor(x => x.Datos.Password);
        }

        [Fact]
        public async Task ValidarComando_DatosIncorrectos()
        {
            var comando = new IniciarSesion.Comando(new IniciarSesion.DatosInciarSesionDTO("wsewefc", ""));

            var sut = await new IniciarSesion.ValidadorComando().TestValidateAsync(comando);
            sut.ShouldHaveValidationErrorFor(x => x.Datos.Email);
            sut.ShouldHaveValidationErrorFor(x => x.Datos.Password);
        }
    }
}
