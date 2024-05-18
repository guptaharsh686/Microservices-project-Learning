
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            if(builder.Environment.IsProduction())
            {
                Console.WriteLine("--> Using SQL Server DB");
                builder.Services.AddDbContext<AppDbContext>(opt =>
                    opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformConn")));
            }
            else
            {
                Console.WriteLine("--> Using InMem DB");
                builder.Services.AddDbContext<AppDbContext>(opt =>
                    opt.UseInMemoryDatabase("InMem")
                );
            }
            builder.Services.AddScoped<IPlatformRepo,PlatformRepo>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddGrpc();
            //Managed by builtin http client factory
            builder.Services.AddHttpClient<ICommandDataClient,HttpCommandDataClient>();

            builder.Services.AddSingleton<IMessageBusClient,MessageBusClient>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.MapGrpcService<GrpcPlatformService>();

            app.MapGet("/protos/platforms.proto", async context =>
            {
                await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //    endpoints.MapGrpcService<GrpcPlatformService>();
            //});

            PrepDb.PrepPopulation(app,builder.Environment.IsProduction());

            app.Run();
        }
    }
}
