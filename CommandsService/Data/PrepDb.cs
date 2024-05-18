using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpcClient.ReturnAllPlatforms();

                seedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(),platforms);

            }


        }

        private static void seedData(ICommandRepo commandRepo,IEnumerable<Platform> platforms)
        {
            Console.WriteLine($"--> Seeding new Platforms...");

            foreach(Platform platform in platforms)
            {
                if(!commandRepo.ExternalPlatformExists(platform.ExternalID))
                {
                    commandRepo.CreatePlatform(platform);
                }
                commandRepo.SaveChanges();
            }
        }
    }
}
