using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticationApi.Configurations;
using AuthenticationApi.Infrastructure;
using AuthenticationApi.Services;
using System.Threading.RateLimiting;
using AuthenticationApi.Templates;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using SharedLibrary.Kafka;
using Nest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


// Kafka Config Servisini ekle
builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection("KafkaConfig"));

// Elasticsearch Client'ı tanımla
builder.Services.AddSingleton<IElasticClient>(sp =>
{
    var settings = new ConnectionSettings(new Uri(builder.Configuration["Elasticsearch:Uri"] ?? "http://localhost:9200"));
    return new ElasticClient(settings);
});

// Kafka Log Consumer Servisini ekle
builder.Services.AddHostedService<KafkaAuditLogConsumerService>();

// MySQL bağlantısı için Entity Framework Core ayarları
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));





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

/*
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
*/

// Kafka consumer background service
builder.Services.AddHostedService<KafkaAuditLogConsumerService>();

// LDAP Config ve Servisi
builder.Services.Configure<LdapConfig>(builder.Configuration.GetSection("Ldap"));
builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<ILdapService, LdapService>();
builder.Services.AddScoped<IEmailService, EmailService>();




// Diğer servisler
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserTempRepository, UserTempRepository>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IPolicyService,PolicyService>();
// Startup.cs veya Program.cs
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPolicyRepository,PolicyRepository>();

builder.Services.AddScoped<IRoleService,RoleService>();
builder.Services.AddScoped<IRoleRepository,RoleRepository>();

builder.Services.AddScoped<IPermissionService,PermissionService>();
builder.Services.AddScoped<IPermissionRepository,PermissionRepository>();

builder.Services.AddScoped<IModuleService,ModuleService>();
builder.Services.AddScoped<IModuleRepository,ModuleRepository>();

builder.Services.AddScoped<IPageService,PageService>();
builder.Services.AddScoped<IPageRepository,PageRepository>();

builder.Services.AddScoped<IConditionService,ConditionService>();
builder.Services.AddScoped<IConditionRepository,ConditionRepository>();

builder.Services.AddScoped<IAuditLogService,AuditLogService>();
builder.Services.AddScoped<IAuditLogRepository,AuditLogRepository>();

builder.Services.AddScoped<ILoginLogRepository, LoginLogRepository>();
builder.Services.AddScoped<ILoginLogService, LoginLogService>();

builder.Services.AddSingleton<PasswordHasher>();
// ITokenService artık Scoped olarak eklendi
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddSingleton<TemplateHelper>();

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();



// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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



builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

// JWT Configurations
var jwtConfig = builder.Configuration.GetSection("JwtConfig");
var secretKey = jwtConfig["Secret"]!;
var issuer = jwtConfig["Issuer"]!;
var audience = jwtConfig["Audience"]!;

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            RoleClaimType = ClaimTypes.Role // Rolleri doğru claim'den oku
        };
    });


builder.Services.AddHttpContextAccessor();
/*
// HttpContextAccessor ve DbContext

// Global Filter  ile loglama işlemini yapmış olduk.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditLogActionFilter>();
});
*/

builder.Services.AddAuthorization();

var app = builder.Build();



// Rate Limiting Middleware
//app.UseRateLimiter();

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
