using Microsoft.EntityFrameworkCore;

namespace OpsPortal.Infrastructure.Persistence.ModelBuilders;

internal interface IModelBuilder
{
    void BuildModel(ModelBuilder modelBuilder);
}
