using Microsoft.EntityFrameworkCore;
using Timescaler.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<TimescalerDbContext>
    (options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(TimescalerDbContext)));
    });

var app = builder.Build();

app.Run();