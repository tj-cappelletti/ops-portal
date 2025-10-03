using System.Diagnostics.CodeAnalysis;
using OpsPortal.Application.Features.SolutionStacks.Queries;

namespace OpsPortal.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class GetSolutionStackByIdEqualityComparer : IEqualityComparer<GetSolutionStackById>
{
    public bool Equals(GetSolutionStackById? x, GetSolutionStackById? y)
    {
        return x == y;
    }

    public int GetHashCode(GetSolutionStackById obj)
    {
        return obj.GetHashCode();
    }
}
