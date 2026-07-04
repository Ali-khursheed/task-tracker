using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskTracker.Helpers;
using TaskTracker.Repositories;
using TaskTracker.Services;
namespace TaskTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.

      builder.Services.AddControllers()
.AddJsonOptions(options =>
{
options.JsonSerializerOptions.PropertyNameCaseInsensitive=true;
});
      // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
      builder.Services.AddEndpointsApiExplorer();
           
      builder.Services.AddScoped<UserRepository>();
      builder.Services.AddScoped<JwtHelper>();
      builder.Services.AddScoped<AuthService>();
      builder.Services.AddSingleton<Data.DbConnectionFactory>();
      builder.Services.AddScoped<TaskRepository>();
      builder.Services.AddScoped<NotificationRepository>();
      builder.Services.AddScoped<TaskService>();
      builder.Services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title="TaskTracker API", Version="v1" });

        // This adds the Authorize button to Swagger UI
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Description="Enter: Bearer {your token}",
          Name="Authorization",
          In=ParameterLocation.Header,
          Type=SecuritySchemeType.ApiKey,
          Scheme="Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
      });

      var jwtSettings = builder.Configuration.GetSection("JwtSettings");
      var secretKey = jwtSettings["SecretKey"]!;

      builder.Services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
      })
.AddJwtBearer(options =>
{
  options.TokenValidationParameters=new TokenValidationParameters
  {
    ValidateIssuer=true,
    ValidateAudience=true,
    ValidateLifetime=true,
    ValidateIssuerSigningKey=true,
    ValidIssuer=jwtSettings["Issuer"],
    ValidAudience=jwtSettings["Audience"],
    IssuerSigningKey=new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(secretKey)
      )
  };
});

      builder.Services.AddCors(options =>
      {
        options.AddPolicy("AllowAngular", policy =>
        {
          policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
      });

      var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
