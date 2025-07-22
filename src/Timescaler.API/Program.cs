using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Timescaler.Application.Ports;
using Timescaler.Application.Services;
using Timescaler.Application.Services.Interfaces;
using Timescaler.Application.Services.Parsing;
using Timescaler.Application.Validation;
using Timescaler.Infrastructure.Data;
using Timescaler.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TimescalerDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(TimescalerDbContext)));
});

builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<IRawValueRepository, RawValueRepository>();

builder.Services.AddScoped<IDataProcessingService, DataProcessingService>();
builder.Services.AddScoped<ICsvParser, CsvParser>();

builder.Services.AddScoped<IFileValidator, FileValidator>();
builder.Services.AddScoped<ICsvRowParser, CsvRowParser>();

builder.Services.AddScoped<IValidator<RawCsvRow>, CsvStructureValidator>();
builder.Services.AddScoped<IValidator<ParsedCsvRow>, CsvBusinessRulesValidator>();
builder.Services.AddScoped<IValidator<FileStructure>, FileStructureValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();