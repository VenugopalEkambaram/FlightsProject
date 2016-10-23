using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace FlightsApi.Filters
{
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private static readonly CacheControlHeaderValue CacheControlHeaderValue = new CacheControlHeaderValue()
        {
            NoCache = true,
            NoStore = true
        };

        public ExceptionHandlerAttribute()
        {
            //Inject any logger like NLog, log4net
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            //log the exception

            base.OnException(actionExecutedContext);

            var exception = actionExecutedContext.Exception;       

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message);
            actionExecutedContext.Response.Headers.CacheControl = CacheControlHeaderValue;
        }
    }
}