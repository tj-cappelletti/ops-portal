using System.Security.Claims;
using MediatR;
using OpsPortal.Application.Auditing;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Application.Http;
using OpsPortal.Domain.Constants;

namespace OpsPortal.Application.Common.Behaviors;

public class ActionContextBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentHttpContext _currentHttpContext;

    public ActionContextBehavior(ICurrentHttpContext currentHttpContext)
    {
        _currentHttpContext = currentHttpContext;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not IActionContextAware actionContextAware) return await next(cancellationToken);

        if (_currentHttpContext.IsAuthenticated())
        {
            var userId = _currentHttpContext.GetClaim(ClaimTypes.NameIdentifier);
            var userEmail = _currentHttpContext.GetClaim(ClaimTypes.Email);
            var displayName = _currentHttpContext.GetClaim(ClaimTypes.Name);

            var actorInfo = new ActorInfo(
                Guid.TryParse(userId, out var id) ? id : SystemDefaults.SystemUsers.Unknown.Id,
                displayName ?? SystemDefaults.SystemUsers.Unknown.DisplayName,
                userId ?? SystemDefaults.SystemUsers.Unknown.Identifier,
                userEmail ?? SystemDefaults.SystemUsers.Unknown.Email);

            actionContextAware.ActionContext = new ActionContext(
                actorInfo,
                _currentHttpContext.GetIpAddress(),
                _currentHttpContext.GetUserAgent(),
                _currentHttpContext.GetSessionId());
        }
        else
        {
            // TODO: Determine if we can identify the type of unauthenticated system action (e.g., Scheduler, Migration)
            // TODO: Switch to Unknown user after authentication is implemented
            actionContextAware.ActionContext = ActionContext.CreateSystemActionContext(SystemUserType.System, "Unauthenticated Request");
        }

        return await next(cancellationToken);
    }
}
