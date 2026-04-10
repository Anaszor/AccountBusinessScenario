using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Mappings;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStatementRepository, StatementRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(GenerateStatementsHandler).Assembly);
}); 
builder.Services.AddValidatorsFromAssembly(typeof(GenerateStatementsValidator).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();