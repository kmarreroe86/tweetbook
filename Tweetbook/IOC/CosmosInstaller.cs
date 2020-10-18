using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tweetbook.Domain;
using Tweetbook.Services;

namespace Tweetbook.IOC
{
    public class CosmosInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
             var cosmosStoreSettings = new CosmosStoreSettings(
                 configuration["CosmosSettings:DatabaseName"],
                 configuration["CosmosSettings:AccountUri"],
                 configuration["CosmosSettings:AccountKey"],
                 new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp}
                 );

             services.AddCosmosStore<CosmosPostDto>(cosmosStoreSettings);
        }
    }
}