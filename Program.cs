using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SuClassQrApp.DataAccess;
using SuClassQrApp.Business;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------------
// 1) DbContext kaydı
// --------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------------
// 2) Business servisleri
// --------------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IQrService, QrService>();

// --------------------------
// 3) JWT Authentication
// --------------------------
var jwtSecret = builder.Configuration["Jwt:Secret"];
var key = Encoding.ASCII.GetBytes(jwtSecret ?? throw new InvalidOperationException("Jwt:Secret not configured"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// --------------------------
// 4) MVC + Swagger
// --------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------
// 5) CORS
// --------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// --------------------------
// 6) Middleware pipeline
// --------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ana sayfayı swagger'a yönlendir
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
