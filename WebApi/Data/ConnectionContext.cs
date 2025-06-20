using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Model;

namespace WebApi.Context
{
    public class ConnectionContext : DbContext
    {
        public ConnectionContext(DbContextOptions<ConnectionContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
    }
}
