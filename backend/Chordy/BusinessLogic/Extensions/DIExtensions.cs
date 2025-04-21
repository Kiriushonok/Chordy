using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Jwt;
using Chordy.BusinessLogic.Services;

namespace Chordy.BusinessLogic.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAuthorService, AuthorService>();
            serviceCollection.AddScoped<ICollectionService, CollectionService>();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<IJwtProvider, JwtProvider>();
            serviceCollection.AddScoped<ISongService, SongService>();
            return serviceCollection;
        }
    }
}
