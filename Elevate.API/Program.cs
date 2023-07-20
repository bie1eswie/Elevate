using Elevate.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Elevate.Extentions.Identity;
using Elevate.Extentions.Extentions;
using Elevate.Data.HumanAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServiceDependencies();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    string[] methods = { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" };
    options.AddPolicy("AllowFrontend", builder => builder.WithOrigins("http://localhost:4200", "https://gentle-moss-058576610.3.azurestaticapps.net").AllowAnyHeader().WithMethods(methods));
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<HumanAPIContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
