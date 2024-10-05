using DeployMe.API;
using DeployMe.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetValue<string>("ConnectionStringAzureSQL");

builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseSqlServer(connection, sqlOptions =>
    {
        sqlOptions.MigrationsHistoryTable("EFMigrationHistory", StudentDbContext.SQL_SCHEMA);
    }));


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
