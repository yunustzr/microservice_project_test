using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticationApi.Configurations;
using AuthenticationApi.Infrastructure;
using AuthenticationApi.Infrastructure.Repositories;
using AuthenticationApi.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MySQL bağlantısı için Entity Framework Core ayarları
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});

// Rate Limiter Servisleri
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var key = context.Request.Headers["X-Forwarded-For"].ToString();
        if (string.IsNullOrEmpty(key))
        {
            key = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
});

// LDAP Config ve Servisi
builder.Services.Configure<LdapConfig>(builder.Configuration.GetSection("Ldap"));
builder.Services.AddScoped<ILdapService, LdapService>();

// Yerel kullanıcı servisleri ve repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Authentication Service
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddSingleton<PasswordHasher>();

var app = builder.Build();

// Security Headers Middleware (CSP ayarları; Swagger için inline style izni)
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    string csp;
    if (path.StartsWithSegments("/swagger"))
    {
        csp = "default-src 'self'; style-src 'self' 'unsafe-inline'";
    }
    else
    {
        csp = "default-src 'self'";
    }
    context.Response.Headers.Append("Content-Security-Policy", csp);
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    await next();
});

// Rate Limiting Middleware
app.UseRateLimiter();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Veritabanı migrasyonlarını uygula ve seed işlemi (ilk admin kullanıcısı)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

}

app.Run();
