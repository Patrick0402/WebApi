using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Domain.Model;
using WebApi.Data.Repository.Employee;
using WebApi.Application.ViewModel;
using AutoMapper;
using WebApi.Domain.DTOs;
using Asp.Versioning;

namespace WebApi.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/employee")]
    // Versão v2 com authentication
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeInterface _employeeRepository;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeInterface employeeRepository, ILogger<EmployeeController> logger, IMapper mapper)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
        public IActionResult Get()
        {
            var employees = _employeeRepository.Get();
            var employeeDTOs = _mapper.Map<List<EmployeeDTO>>(employees);
            return Ok(employeeDTOs);
        }


        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee == null)
                return NotFound("Employee not found");

            var employeeDTO = _mapper.Map<EmployeeDTO>(employee);
            return Ok(employeeDTO);
        }

        [HttpGet]
        [Route("paginated")]
        [Authorize]
        public IActionResult GetPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 10)
            {
                pageNumber = 1;
                pageSize = 10;
            }

            var employees = _employeeRepository.GetPaginated(pageNumber, pageSize);
            var employeeDTOs = _mapper.Map<List<EmployeeDTO>>(employees);

            _logger.LogInformation($"Paginated request: Page {pageNumber}, Size {pageSize}, Total Results: {employeeDTOs.Count}");

            return Ok(employeeDTOs);
        }

        [HttpPost]
        [Authorize]
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