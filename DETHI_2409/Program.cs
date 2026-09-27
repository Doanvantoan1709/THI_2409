using DETHI_2409.Entities;
using DETHI_2409.Services.Implementations;
using DETHI_2409.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


//AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Add services to the container.   
builder.Services.AddDbContext<Dt2409Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DETHI_2409")));


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddTransient<IWorkitemService, WorkitemService>();
builder.Services.AddTransient<IHealthService, HealthService>();
builder.Services.AddTransient<IReportService, ReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
