using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpsPortal.Domain.Entities
{
    public enum AuthenticationType
    {
        Local,
        SingleSignOn,
        ApiKey,     // API key authentication
        System      // System/service accounts
    }
}
