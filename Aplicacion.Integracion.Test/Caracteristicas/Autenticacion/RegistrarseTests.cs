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

namespace Aplicacion.Integracion.Test.Caracteristicas.Autenticacion;

[Collection(nameof(SliceFixture))]
public class RegistrarseTests
{
    private readonly SliceFixture sliceFixture;

    public RegistrarseTests(SliceFixture sliceFixture)
    {
        this.sliceFixture = sliceFixture;
    }

    [Fact]
    public async Task Registrarse_RetornaUsuario()
    {
        await sliceFixture.ResetCheckpoint();

        var comando = new Registrarse.Comando(new Registrarse.DatosRegistrarseDTO("Pepe Mujica","pepe@gmail.com","qazwsx"));
        var sut = await sliceFixture.SendAsync(comando);

        sut.Id.ShouldNotBe(0);
        var usuarioBD = await sliceFixture.FindOrDefaultAsync<Usuario>(sut.Id);
        usuarioBD.ShouldNotBeNull();
    }

    [Fact]  
    public async Task Registrarse_EmailExistente_RetornaError()
    {
        await sliceFixture.ResetCheckpoint();

        var comando = new Registrarse.Comando(new Registrarse.DatosRegistrarseDTO("Pepe Mujica", "pepe@gmail.com", "qazwsx"));

        await sliceFixture.InsertAsync(Usuario.Crear("Pepe",comando.Datos.Email!, "12312312312"));

        Should.Throw<ErroresUsuario.EmailExistente>(async () =>
            await sliceFixture.SendAsync(comando)
        );
    }
}
