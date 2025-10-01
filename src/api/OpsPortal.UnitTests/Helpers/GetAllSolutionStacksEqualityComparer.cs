using System.Diagnostics.CodeAnalysis;
using OpsPortal.Application.Features.SolutionStacks.Queries;

namespace OpsPortal.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class GetAllSolutionStacksEqualityComparer : IEqualityComparer<GetAllSolutionStacks>
{
    public bool Equals(GetAllSolutionStacks? x, GetAllSolutionStacks? y)
    {
        return x == y;
    }

    public int GetHashCode(GetAllSolutionStacks obj)
    {
        return obj.GetHashCode();
    }
}
