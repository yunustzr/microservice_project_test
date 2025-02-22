using GatewayApi.Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();

builder.Configuration.AddJsonFile("ocelot.json", optional:false, reloadOnChange:true);
builder.Services.AddOcelot();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        });
}
);
var app = builder.Build();



app.UseHttpsRedirection();
app.UseMiddleware<IntercaptionMiddleware>();

app.UseCors();
app.UseAuthorization();

app.UseOcelot().Wait();
app.Run();
