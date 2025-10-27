using API_Autenticacion.DbSemilla;
using API_Autenticacion.Middleware;
using API_Autenticacion.Servicios;
using Aplicacion;
using Aplicacion.Helper.Servicios;
using dotenv.net;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//DotEnv.Load();
Env.Load();
Console.WriteLine("SECRETO_JWT: " + Environment.GetEnvironmentVariable("SECRETO_JWT"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("*", 
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IServicioUsuarioActual, ServicioUsuarioActual>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});
var secret = Environment.GetEnvironmentVariable("SECRETO_JWT");
if (string.IsNullOrEmpty(secret))
{
    throw new Exception("La variable SECRETO_JWT no está configurada en Program.cs");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Autenticacion", Version = "v1" });

    // Definir el esquema de seguridad
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor ingrese el token JWT con el prefijo 'Bearer' (Ejemplo: 'Bearer 12345abcdef')",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Requerir el token en cada operaci�n
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AgregarAplicacion(builder.Configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "3000")); // Configura el puerto
});
var meterProvider = Sdk.CreateMeterProviderBuilder()
    .AddPrometheusExporter()
    .AddMeter("Microsoft.AspNetCore.Hosting")
    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
    .AddMeter("Microsoft.AspNetCore.Http.Connections")
    .AddMeter("Microsoft.AspNetCore.Routing")
    .AddMeter("Microsoft.AspNetCore.Diagnostics")
    .AddMeter("Microsoft.AspNetCore.RateLimiting")
    .Build();
builder.Services.AddSingleton(meterProvider);
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.SemillaDataIncial();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExcepcionMiddleware>();
app.UseCors("*");
app.MapControllers();

app.Run();
