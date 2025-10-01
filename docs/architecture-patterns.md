```mermaid
graph TD;
    app[OpsPortal.Application]
    contracts[OpsPortal.Contracts]
    domain[OpsPortal.Domain]
    infra[OpsPortal.Infrastructure]
    webapi[OpsPortal.WebApi]

    app-->domain
    app-->contracts
    infra-->domain
    infra-->app
    webapi-->app
    webapi-->infra
    webapi-->contracts
```