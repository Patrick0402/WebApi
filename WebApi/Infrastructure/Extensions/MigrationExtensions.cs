using Microsoft.EntityFrameworkCore;
using WebApi.Context;

namespace WebApi.Infrastructure.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ConnectionContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectionContext>();
        dbContext.Database.Migrate();
    }
}
