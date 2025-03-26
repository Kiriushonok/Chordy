using Chordy.DataAccess.Repositories.Implementations;
using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAcess(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<IAuthorRepository, AuthorRepository>();
            serviceCollection.AddScoped<ICollectionRepository, CollectionRepository>();
            serviceCollection.AddDbContext<ChordyDbContext>(x =>
            {
                x.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
            });
            return serviceCollection;
        }
    }
}
