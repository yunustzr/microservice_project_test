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
using Elasticsearch.Net;
using AuthenticationApi.Infrastructure.Repositories;
using AuthenticationApi.Infrastructure.Interceptors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// Kafka Config Servisini ekle
builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection("KafkaConfig"));



// Elasticsearch Client'ı tanımla
builder.Services.AddSingleton<IElasticClient>(sp =>
{
    var configuration = builder.Configuration;
    var esUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
    var esUsername = configuration["Elasticsearch:Username"];
    var esPassword = configuration["Elasticsearch:Password"];

    var settings = new ConnectionSettings(new Uri(esUri))
        .BasicAuthentication(esUsername, esPassword)
        .DisableAutomaticProxyDetection()
        .DisableDirectStreaming()
        .DefaultIndex("audit_logs");

    return new ElasticClient(settings);
});


/*
builder.Services.AddSingleton<IElasticClient>(sp => 
{
    var configuration = builder.Configuration;
    var esUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
    var esUsername = configuration["Elasticsearch:Username"];
    var esPassword = configuration["Elasticsearch:Password"];
    var settings = new ConnectionSettings(new Uri(esUri))
        .BasicAuthentication(esUsername, esPassword)
        .DefaultIndex("audit_logs")
        .DisableDirectStreaming() // Request/Response verilerini loglama
        .EnableDebugMode()
        .ServerCertificateValidationCallback(CertificateValidations.AllowAll); // SSL doğrulamasını kapat

    return new ElasticClient(settings);
});
*/


// ✅ Logging Servisini Ekle
builder.Services.AddLogging();




/*
// Elasticsearch Client'ı ekle
builder.Services.AddSingleton<IElasticClient>(sp =>
{
    var configuration = builder.Configuration;
    var logger = sp.GetRequiredService<ILogger<Program>>(); 

    var esUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
    var esUsername = configuration["Elasticsearch:Username"];
    var esPassword = configuration["Elasticsearch:Password"];

    var settings = new ConnectionSettings(new Uri("http://elasticsearch:9200"))
        .DefaultIndex("audit_logs")
        .DisableDirectStreaming()
        .EnableDebugMode(apiCallDetails =>
        {
            logger.LogInformation("===[Elasticsearch API Call]===");
            logger.LogInformation($"Request: {apiCallDetails.HttpMethod} {apiCallDetails.Uri}");

            if (apiCallDetails.RequestBodyInBytes != null)
            {
                logger.LogInformation("Request Body: {RequestBody}", Encoding.UTF8.GetString(apiCallDetails.RequestBodyInBytes));
            }

            var statusCode = apiCallDetails.HttpStatusCode.HasValue
            ? apiCallDetails.HttpStatusCode.Value.ToString()
            : "null";

            logger.LogInformation("Response: {StatusCode} {DebugInfo}", statusCode, apiCallDetails.DebugInformation);

            if (apiCallDetails.ResponseBodyInBytes != null)
            {
                logger.LogInformation("Response Body: {ResponseBody}", Encoding.UTF8.GetString(apiCallDetails.ResponseBodyInBytes));
            }
        })
        .DisablePing()
        .ServerCertificateValidationCallback((sender, cert, chain, errors) => true)
        .BasicAuthentication("elastic", "personel0660")
        .EnableHttpCompression();


    return new ElasticClient(settings);
});
*/




// Kafka Log Consumer Servisini ekle
builder.Services.AddHostedService<KafkaAuditLogConsumerService>();

// MySQL bağlantısı için Entity Framework Core ayarları
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    .AddInterceptors(new SelectAuditInterceptor(builder.Services.BuildServiceProvider()))
);





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

builder.Services.AddScoped<ILdapConfigurationRepository,LdapConfigurationRepository>();
builder.Services.AddScoped<ILdapConfigService,LdapConfigService>();







builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();

builder.Services.AddScoped<IUserGroupService, UserGroupService>();
builder.Services.AddScoped<IUserGroupRepository, UserGroupRepository>();

builder.Services.AddScoped<IGroupRoleService, GroupRoleService>();
builder.Services.AddScoped<IGroupRoleRepository, GroupRoleRepository>();


builder.Services.AddScoped<IRoleService,RoleService>();
builder.Services.AddScoped<IRoleRepository,RoleRepository>();

builder.Services.AddScoped<IPermissionService,PermissionService>();
builder.Services.AddScoped<IPermissionRepository,PermissionRepository>();

builder.Services.AddScoped<IModuleService,ModuleService>();
builder.Services.AddScoped<IModuleRepository,ModuleRepository>();

builder.Services.AddScoped<IPageService,PageService>();
builder.Services.AddScoped<IPageRepository,PageRepository>();

builder.Services.AddScoped<IRefreshTokenRepository,RefreshTokenRepository>();

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

// JWT Authentication Güncellemesi
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
            RoleClaimType = ClaimTypes.Role
        };

        // Token versiyon kontrolü için event handler
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var userIdClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier);
                
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    context.Fail("Geçersiz token");
                    return;
                }

                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    context.Fail("Kullanıcı bulunamadı");
                    return;
                }

                var tokenVersionClaim = context.Principal.FindFirst("tokenVersion");
                if (tokenVersionClaim == null || !int.TryParse(tokenVersionClaim.Value, out var tokenVersion))
                {
                    context.Fail("Token versiyon bilgisi eksik");
                    return;
                }

                if (user.TokenVersion != tokenVersion)
                {
                    context.Fail("Eski token versiyonu");
                    return;
                }
            }
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
builder.Services.AddMemoryCache();

var app = builder.Build();

// ✅ Loglamanın çalıştığını test etmek için
var loggerTest = app.Services.GetRequiredService<ILogger<Program>>();
loggerTest.LogInformation("Elasticsearch Logger Test Mesajı");


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
