using System.Security.Claims;
using Common.Domain.Entities;

namespace Common.Application.Abstractions;
public interface IUserContext
{
    CurrentUser? GetCurrentUser();
    void SetUser (ClaimsPrincipal user);

}
