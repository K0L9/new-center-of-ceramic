using CenterOfCeramic.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CenterOfCeramic.Data
{
    public static class AppBuilderExtensions
    {
        internal static async Task<IApplicationBuilder> Initialize(this IApplicationBuilder app, Microsoft.Extensions.Configuration.IConfiguration _configuration)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>();

            foreach (var initializer in initializers)
            {
                await initializer.InitializeAsync();
            }

            return app;
        }
    }
}
