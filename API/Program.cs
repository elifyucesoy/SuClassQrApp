using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SuClassQrApp.Business;
using SuClassQrApp.DataAccess;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Database context
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? "Server=LAPTOP-FQOV39PF\\SQLEXPRESS;Database=SuClassQrDb;Trusted_Connection=True;TrustServerCertificate=True;";

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Dependency injection for services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IQrService, QrService>();

            // JWT Authentication
            var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-secret-key-min-32-characters-long-here";
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Swagger'a otomatik yönlendirme
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
