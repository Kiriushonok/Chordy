using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Chordy.DataAccess
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAcess(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<IAuthorRepository, AuthorRepository>();
            serviceCollection.AddDbContext<AppContext>(x =>
            {
                x.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
            });
            return serviceCollection;
        }
    }
}
