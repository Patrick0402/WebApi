using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Data.Repository.Employee;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;
using WebApi.Application.Mapping;
using WebApi.Application.Swagger;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection; // Add this line

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(DomainToDTOMapping));

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
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrEmpty(origin))
                    return false;

                try
                {
                    var uri = new Uri(origin);
                    var host = uri.Host;
                    var port = uri.Port;

                    // Permitir localhost:8080
                    if ((host == "localhost" || host == "127.0.0.1") && port == 8080)
                        return true;

                    // Permitir IPs de 192.168.1.100 até 192.168.1.110, somente na porta 8080
                    if (host.StartsWith("192.168.1."))
                    {
                        if (int.TryParse(host.Split('.')[3], out int lastOctet))
                        {
                            if (lastOctet >= 100 && lastOctet <= 110 && port == 8080)
                                return true;
                        }
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
            })
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

builder.WebHost.UseUrls("http://0.0.0.0:5241");


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

app.UseCors("CorsPolicy");

app.UseAuthorization();

// Mapeia as rotas dos controllers (API endpoints)
app.MapControllers();

app.Run();
