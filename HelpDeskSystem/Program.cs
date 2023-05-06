using Microsoft.EntityFrameworkCore;
using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using AutoMapper;

namespace HelpDeskSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddCors();
            builder.Services.AddDbContext<TodoContext>(opt =>
                opt.UseInMemoryDatabase("TodoList"));
            builder.Services.AddDbContext<EF_DataContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Ef_Postgres_Db")));
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(
              options => options.WithOrigins("http://localhost:4200")
                          .AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
            );

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}

