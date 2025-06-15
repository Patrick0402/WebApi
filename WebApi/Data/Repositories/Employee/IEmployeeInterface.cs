using WebApi.Domain.DTOs;
using WebApi.Domain.Model;

namespace WebApi.Data.Repository.Employee
{
    public interface IEmployeeInterface
    {
        void Add(EmployeeModel employee);
        List<EmployeeModel> Get();
        EmployeeModel? GetById(int id);
        List<EmployeeModel> GetPaginated(int pageNumber, int pageSize);
    }
}
