using dex.monitor.Business.DataStores.MemoryStores.PairsStore;
using dex.monitor.Business.DataStores.MemoryStores.TokensStore;
using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.DataStores.Persistant.Internals;
using dex.monitor.Business.Domain;
using dex.monitor.Business.Services.Cex.Models;
using JasperFx;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dex.monitor.Business.DataStores;

internal static class DataStoresInjections
{
    public static IServiceCollection AddDataStores(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddScoped<IPersistantStore, MartenPersistantStore>()
            .AddSingleton<ITokensStore, InMemoryTokenStore>()
            .AddSingleton<IPairsStore, InMemoryPairsStore>()
            .AddMarten(options =>
            {
                options.Connection(configuration.GetConnectionString("default")!);
                options.AutoCreateSchemaObjects = AutoCreate.CreateOnly;
                options.DisableNpgsqlLogging = true;
            })
            .ApplyAllDatabaseChangesOnStartup().Services
            .ConfigureStore();

    private static IServiceCollection ConfigureStore(this IServiceCollection services)
    {
        return services
            .ConfigureMarten(options =>
            {
                options.RegisterDocumentType<ChainStatus>();
                options.RegisterDocumentType<DexSettings>();

                options.RegisterDocumentType<DexToken>();
                options.Schema.For<DexToken>().Identity(e => e.Code);
                
                options.RegisterDocumentType<CexToken>();
                options.Schema.For<CexToken>().Identity(e => e.Code);
            });
    }
}