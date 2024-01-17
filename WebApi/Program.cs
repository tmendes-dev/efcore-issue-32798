
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebApi.Context;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApiContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ApiContext"),
                            c =>
                                c.EnableRetryOnFailure(
                                        maxRetryCount: 3,
                                        maxRetryDelay: TimeSpan.FromSeconds(2),
                                        errorNumbersToAdd: null))
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors()
                        .ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuting, LogLevel.Debug))));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
