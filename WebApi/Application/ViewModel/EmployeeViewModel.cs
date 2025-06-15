namespace WebApi.Application.ViewModel
{
    public class EmployeeViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
