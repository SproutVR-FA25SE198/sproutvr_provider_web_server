using System.Security.Claims;
using Common.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Common.Presentation.Middlewares;
public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserContext userContext)
    {
        ClaimsPrincipal user = context.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            userContext.SetUser(user);
        }

        await _next(context);
    }
}
