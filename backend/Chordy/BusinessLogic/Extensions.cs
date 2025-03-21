using Chordy.BusinessLogic.Interfaces;
using Chordy.BusinessLogic.Services;

namespace Chordy.BusinessLogic
{
    public static class Extensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAuthorService, AuthorService>();
            return serviceCollection;
        }
    }
}
