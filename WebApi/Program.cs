using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Repository.Employee;

var builder = WebApplication.CreateBuilder(args);

// Adicionando o DbContext no container de serviços
builder.Services.AddDbContext<ConnectionContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


// Registrando o repositório
builder.Services.AddTransient<IEmployeeInterface, EmployeeRepository>();

// **Registrar os controllers no DI!**
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapeia as rotas dos controllers (API endpoints)
app.MapControllers();

app.Run();
