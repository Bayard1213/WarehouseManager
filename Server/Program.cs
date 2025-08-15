
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using WarehouseManager.Server.Abstractions.Repositories;
using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Middleware;
using WarehouseManager.Server.Models;
using WarehouseManager.Server.Repositories;
using WarehouseManager.Server.Services;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                }));

            builder.Services.AddScoped<IBalanceRepository, BalanceRepository>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IDocumentReceiptRepository, DocumentReceiptRepository>();
            builder.Services.AddScoped<IDocumentShipmentRepository, DocumentShipmentRepository>();
            builder.Services.AddScoped<IMeasureRepository, MeasureRepository>();
            builder.Services.AddScoped<IReceiptResourceRepository, ReceiptResourceRepository>();
            builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
            builder.Services.AddScoped<IShipmentResourceRepository, ShipmentResourceRepository>();

            builder.Services.AddScoped<IBalanceService, BalanceService>();
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IMeasureService, MeasureService>();
            builder.Services.AddScoped<IReceiptService, ReceiptService>();
            builder.Services.AddScoped<IResourceService, ResourceService>();
            builder.Services.AddScoped<IShipmentService, ShipmentService>();

            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowClient", policy =>
                {
                    policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            app.UseCors("AllowClient");

            app.UseSwagger();
            app.UseSwaggerUI();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.Run();
        }
    }
}
