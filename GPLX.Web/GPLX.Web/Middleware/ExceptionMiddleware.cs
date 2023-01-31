using System;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GPLX.Web.Middleware
{
    public class ExceptionMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionMiddleware> _logger;
            public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
                _next = next;
            }
            public async Task InvokeAsync(HttpContext httpContext)
            {
                try
                {
                    await _next(httpContext).ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Something went wrong: {Ex}", ex);
                    await HandleExceptionAsync(httpContext, ex).ConfigureAwait(true);
                }
            }
            private async Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.OK;

                if (exception is AuthenticationException)
                {
                    context.Response.Redirect("/Account/Login");
                    return;
                }

                if (exception is DataNotFoundException)
                {
                    await context.Response.WriteAsJsonAsync(new DataNotFoundSearchResponse
                    {
                        Code = (int) HttpStatusCode.NoContent,
                        Draw = ((DataNotFoundException) exception).Draw,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    }).ConfigureAwait(true);
                    return;
                }
                
                
                await context.Response.WriteAsJsonAsync(new ExceptionResultResponse() {
                    Code = (int) HttpStatusCode.InternalServerError,
                    Message = "Có lỗi xảy ra!",
                    RawExceptionMessage = exception.GetBaseException().StackTrace,
                    InnerExceptionMessage = exception.InnerException?.StackTrace
                }).ConfigureAwait(true);
            }
        }
    }