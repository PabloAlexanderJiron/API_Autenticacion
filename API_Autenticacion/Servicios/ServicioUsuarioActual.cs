using Aplicacion.Helper.Servicios;

namespace API_Autenticacion.Servicios
{
    public class ServicioUsuarioActual : IServicioUsuarioActual
    {
        private readonly IHttpContextAccessor _http;
        private readonly IConfiguration configuration;

        public ServicioUsuarioActual(IHttpContextAccessor _http, IConfiguration configuration)
        {
            this._http = _http;
            this.configuration = configuration;
        }
        public string Id => this._http.HttpContext?
                                     .User.FindFirst(x => x.Type == "id")?.Value
                                 ?? string.Empty;

        public string Email => this._http.HttpContext?
                                     .User.FindFirst(x => x.Type == "email")?.Value
                                 ?? string.Empty;

    }
}
