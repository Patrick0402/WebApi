namespace WebApi.Domain.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string NameEmployee { get; set; } = string.Empty;
        public string? Photo { get; set; }

        public EmployeeDTO() { }

        public EmployeeDTO(string name, int age, string? photo)
        {
            NameEmployee = name;
            Photo = photo;
        }
    }
}