using dex.monitor.Business.DataStores.MemoryStores;
using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.DataStores.Persistant.Internals;
using dex.monitor.Business.Domain;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace dex.monitor.Business.DataStores;

internal static class DataStoresInjections
{
    public static IServiceCollection AddDataStores(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddScoped<IPersistantStore, MartenPersistantStore>()
            .AddSingleton<ITokensStore, InMemoryTokenStore>()
            .AddMarten(options =>
            {
                options.Connection(configuration.GetConnectionString("default")!);
                options.AutoCreateSchemaObjects = AutoCreate.CreateOnly;
                options.DisableNpgsqlLogging = true;
            })
            .ApplyAllDatabaseChangesOnStartup()
            .OptimizeArtifactWorkflow().Services
            .ConfigureStore();

    private static IServiceCollection ConfigureStore(this IServiceCollection services)
    {
        return services
            .ConfigureMarten(options =>
            {
                options.RegisterDocumentType<ChainStatus>();
                options.RegisterDocumentType<DexSettings>();
            });
    }
}