using tech_challenge.Application.Exceptions;
using tech_challenge.Domain.Exceptions;
using System.Diagnostics;

namespace tech_challenge.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Requisicao rejeitada: {ErrorType}", ex.GetType().Name);
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await context.Response.WriteAsJsonAsync(new
                {
                    message = ex.Message
                });
            }
            catch(DomainException ex)
            {
                _logger.LogWarning("Requisicao rejeitada: {ErrorType}", ex.GetType().Name);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = ex.Message
                });
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogWarning("Requisicao rejeitada: {ErrorType}", ex.GetType().Name);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                Activity.Current?.SetStatus(ActivityStatusCode.Error, ex.GetType().Name);
                Activity.Current?.SetTag("error.type", ex.GetType().FullName);
                _logger.LogError(ex, "Falha inesperada na requisicao {CorrelationId}", context.TraceIdentifier);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Erro interno"
                });
            }
        }
    }
}
