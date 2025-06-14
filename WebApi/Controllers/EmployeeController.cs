using Microsoft.AspNetCore.Mvc;
using WebApi.Model;
using WebApi.Repository.Employee;
using WebApi.ViewModel;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeInterface _employeeRepository;

        public EmployeeController(IEmployeeInterface employeeRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        [HttpPost]
        public IActionResult Add([FromBody] EmployeeViewModel employeeView)
        {
            var employee = new EmployeeModel(employeeView.Name, employeeView.Age, null);
            _employeeRepository.Add(employee);
            return Ok();
        }

        [HttpGet]

        public IActionResult Get()
        {
            var employee = _employeeRepository.Get();
            return Ok(employee);
        }


    }
}