using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Services;
using WebApi.Domain.Model;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult Auth(string username, string password)
        {
            if (username == "admin" && password == "password")
            {
                var token = TokenService.GenerateToken(new EmployeeModel("admin", 30, null));
                return Ok(token);
            }

            return Unauthorized("Credenciais inválidas.");
        }
    }
}
