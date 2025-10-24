using OpsPortal.Application.Auditing;

namespace OpsPortal.Application.Common.Interfaces;

public interface IActionContextAware
{
    ActionContext? ActionContext { get; set; }
}
