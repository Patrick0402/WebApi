using WebApi.Context;
using WebApi.Domain.Model;

namespace WebApi.Data.Repository.Employee
{
    public class EmployeeRepository : IEmployeeInterface
    {
        private readonly ConnectionContext _context;

        public EmployeeRepository(ConnectionContext context)
        {
            _context = context;
        }

        public void Add(EmployeeModel employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public List<EmployeeModel> Get()
        {
            return _context.Employees.ToList();
        }

        public EmployeeModel? GetById(int id)
        {
            return _context.Employees.Find(id);
        }

        public List<EmployeeModel> GetPaginated(int pageNumber, int pageSize)
        {
            return _context.Employees
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
