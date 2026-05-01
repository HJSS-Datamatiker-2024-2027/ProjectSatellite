
using ProjectSatellite.APIClients;
using ProjectSatellite.DAL;
using StackExchange.Redis;
using System.Runtime.InteropServices;

namespace ProjectSatellite.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<IApiClient>(sp =>
            new BCApiClient("http://bcserver:7048/BC/api/gruppe6/apiGroup/v1.0/companies(c9e99b22-5515-f111-ac69-6045bdc8bf9f)/licenses", 
            "https://api.businesscentral.dynamics.com/v2.0/44833fc5-b393-4b9f-897a-1f876412ddb1/sandbox/api/gruppe6/apiGroup/v1.0/licenses", 
            configuration["TENANT_ID"], configuration["CLIENT_ID"], configuration["CLIENT_SECRET"])); //TODO: smid i .env

            builder.Services.AddSingleton(ConnectionMultiplexer.Connect(
                $"{configuration["REDIS_HOST"]}:{configuration["REDIS_PORT"]},password={configuration["REDIS_PASSWORD"]},abortConnect=false"
                ));

            builder.Services.AddScoped<IDatabase>(sp =>
                sp.GetRequiredService<ConnectionMultiplexer>().GetDatabase());

            builder.Services.AddScoped<IExtensionLicenseDAO, RedisExtensionLicenseDAO>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
