
using CRUD_API.Context;
using CRUD_API.Services;
using Microsoft.EntityFrameworkCore;

namespace CRUD_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler =
                    System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<CrudContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", builder =>
                {
                    builder.WithOrigins(
                         "http://localhost:4200",
                         "https://agreeable-bush-0a65fb500.7.azurestaticapps.net"
                     )
                     .AllowAnyHeader()
                     .AllowAnyMethod();
                });
                //options.AddPolicy("AllowFrontend",
                //policy =>
                //{
                //    policy.WithOrigins("http://localhost:4200")
                //          .AllowAnyHeader()
                //          .AllowAnyMethod();
                //});
            });
            builder.Services.AddHttpClient<EmailService>();
            builder.Services.AddScoped<BlobService>();
            builder.Services.AddScoped<AiService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}


            //app.UseCors("AllowAll");
            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseStaticFiles();


            app.MapControllers();

            app.Run();
        }
    }
}
