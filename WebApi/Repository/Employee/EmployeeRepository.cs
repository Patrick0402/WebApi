using WebApi.Context;
using WebApi.Model;

namespace WebApi.Repository.Employee
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
    }
}
