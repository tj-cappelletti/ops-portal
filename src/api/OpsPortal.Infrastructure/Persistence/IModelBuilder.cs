using Microsoft.EntityFrameworkCore;

namespace OpsPortal.Infrastructure.Persistence;

internal interface IModelBuilder
{
    void BuildModel(ModelBuilder modelBuilder);
}
