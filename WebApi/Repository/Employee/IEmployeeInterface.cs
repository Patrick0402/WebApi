using WebApi.Model;

namespace WebApi.Repository.Employee
{
    public interface IEmployeeInterface
    {
        void Add(EmployeeModel employee);
        List<EmployeeModel> Get();
    }
}
