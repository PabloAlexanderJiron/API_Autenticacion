using Aplicacion.Helper.Comunes;
using System.Data.SqlClient;
using System.Net;

namespace API_Autenticacion.Middleware
{
    public class ExcepcionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExcepcionMiddleware> logger;
        public record ExcepcionRespuesta(HttpStatusCode StatusCode, string Mensaje, IDictionary<string, string[]>? Errores = default);

        public ExcepcionMiddleware(RequestDelegate next, ILogger<ExcepcionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, e.Message);
                await ManejarExcepcion(context, e);
            }
        }

        private async Task ManejarExcepcion(HttpContext context, Exception excepcion)
        {
            var respuesta = excepcion switch
            {
                ExcepcionValidacion e => new ExcepcionRespuesta(HttpStatusCode.BadRequest, e.Message, e.Errors),
                ExcepcionDominio e => new ExcepcionRespuesta(HttpStatusCode.BadRequest, e.Message),
                SqlException e => ManejarExcepcionSql(e),
                Exception e => ManejarExcepcionNoControlada(e)
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)respuesta.StatusCode;
            await context.Response.WriteAsJsonAsync(respuesta);
        }

        private ExcepcionRespuesta ManejarExcepcionSql(SqlException excepcion)
        {
            foreach (SqlError error in excepcion.Errors)
            {
                if (error.Number == 50000)
                    return new ExcepcionRespuesta(HttpStatusCode.BadRequest, excepcion.Message);
            }
            this.logger.LogError(excepcion, "Error a nivel de base de datos");
            return new ExcepcionRespuesta(HttpStatusCode.InternalServerError, excepcion.Message ?? "Error comuniquese con sistemas!");
        }

        private ExcepcionRespuesta ManejarExcepcionNoControlada(Exception excepcion)
        {
            this.logger.LogError(excepcion, "Excepción no controlada");
            return new ExcepcionRespuesta(HttpStatusCode.InternalServerError, excepcion.Message ?? "Error comuniquese con sistemas!");
        }
    }
}