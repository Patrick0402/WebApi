using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Data.Repository.Employee;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;
using WebApi.Application.Mapping;
using WebApi.Application.Swagger;
using Asp.Versioning.ApiExplorer;
using WebApi.Infrastructure.Extensions; // ou o caminho que você salvou



var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddAutoMapper(typeof(DomainToDTOMapping));

string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? configuration["Database:Host"] ?? "localhost";
string dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? configuration["Database:Port"] ?? "5432";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? configuration["Database:Name"] ?? "WebApi";
string dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? configuration["Database:User"] ?? "postgres";
string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? configuration["Database:Password"] ?? "123";

string connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};";

builder.Services.AddDbContext<ConnectionContext>(options =>
{
    options.UseNpgsql(connectionString);
});


// Registrando o repositório
builder.Services.AddTransient<IEmployeeInterface, EmployeeRepository>();

// **Registrar os controllers no DI!**
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioning().AddMvc().AddApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<SwaggerDefaultValues>();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
    {
        new OpenApiSecurityScheme
        {
        Reference = new OpenApiReference
            {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,

        },
        new List<string>()
        }
    });

});

builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCors", policy =>
    {
        policy.WithOrigins("http://localhost:8080")  // Frontend Vue rodando localmente
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    options.AddPolicy("ProductionCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:8080")  // Substituir pelo domínio do frontend em produção
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

//Autenticação JWT
var key = Encoding.ASCII.GetBytes(WebApi.Key.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error-development");
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var version = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in version.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Web Api - {description.GroupName.ToUpper()}");
        }
    });
}

else
{
    app.UseExceptionHandler("/error");
}


if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors");
    app.ApplyMigrations();
}
else
{
    app.UseCors("ProductionCors");
}

app.UseAuthorization();

// Mapeia as rotas dos controllers (API endpoints)
app.MapControllers();

app.Run();
