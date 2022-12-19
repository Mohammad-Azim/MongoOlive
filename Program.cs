using AutoMapper;
using CreateProjectOlive.Mapping;
using CreateProjectOlive.Models;
using CreateProjectOlive.Services;
using CreateProjectOlive.UnitOfWork;

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

        builder.Services.AddSingleton(typeof(IUnitOfWork), typeof(UnitOfWork));
        builder.Services.AddSingleton(typeof(IProjectService), typeof(ProjectService));
        builder.Services.AddAutoMapper(typeof(ProjectProfile));

        builder.Services.Configure<ProjectDataBaseConfig>(builder.Configuration.GetSection("MongoDb"));
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