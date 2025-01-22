using Aplicacion.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace API_Autenticacion.DbSemilla
{
    public static class DbSemillaExtension
    {
        public static void SemillaDataIncial(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var contexto = services.GetRequiredService<ContextoDB>();
                contexto.Database.Migrate();
            }
            catch (Exception)
            {

            }
        }
    }
}
