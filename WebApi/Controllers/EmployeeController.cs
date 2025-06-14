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
        public IActionResult Add([FromForm] EmployeeViewModel employeeView)
        {
            string relativePath = string.Empty;

            if (employeeView.Photo != null)
            {
                var fileName = $"{string.Concat(employeeView.Name
                                        .Normalize(System.Text.NormalizationForm.FormD)
                                        .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                                        .Where(char.IsLetterOrDigit))
                                        .ToUpperInvariant()}_" +
                               $"{DateTime.Now:yyyyMMddHHmmssfff}" +
                               $"{Path.GetExtension(employeeView.Photo.FileName)}";

                relativePath = Path.Combine("Storage", fileName);

                // Cria a pasta se não existir
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Storage"));

                using Stream stream = new FileStream(
                    Path.Combine(Directory.GetCurrentDirectory(), relativePath),
                    FileMode.Create
                );
                employeeView.Photo.CopyTo(stream);
            }

            var employee = new EmployeeModel(employeeView.Name, employeeView.Age, relativePath);
            _employeeRepository.Add(employee);

            return Ok();
        }


        [HttpGet]

        public IActionResult Get()
        {
            var employee = _employeeRepository.Get();
            return Ok(employee);
        }

                [HttpPost]
        [Route("{id}/download")]
        public IActionResult DownloadPhoto(int id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            if (string.IsNullOrEmpty(employee.Photo) || !System.IO.File.Exists(employee.Photo))
            {
                return NotFound("Photo not found");
            }

            var dataBytes = System.IO.File.ReadAllBytes(employee.Photo);

            return File(dataBytes, "image/png");
        }
    }
}