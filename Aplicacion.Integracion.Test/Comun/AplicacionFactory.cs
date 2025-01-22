using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Infraestructura.Persistencia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using dotenv.net;

namespace Aplicacion.Integracion.Test.Comun
{
    internal class AplicacionFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(cfg =>
            {
                var integrationConfig = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .Build();

                cfg.AddConfiguration(integrationConfig);
            });
            
            builder.ConfigureServices((builder, services) =>
            {
                
                services
                    .Remove<DbContextOptions<ContextoDB>>()
                    .AddDbContext<ContextoDB>((sp, options) =>
                    options.UseNpgsql(Environment.GetEnvironmentVariable("URI_DB_TEST") ?? 
                        "Server=localhost;Database=autenticacion_test;Port=5432;User Id=postgres;Password=admin",
                            builder => builder.MigrationsAssembly(typeof(ContextoDB).Assembly.FullName)));
            });
        }
    }
}