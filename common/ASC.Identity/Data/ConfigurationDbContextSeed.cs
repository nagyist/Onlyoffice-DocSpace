using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace ASC.Identity.Models.AccountViewModels;
public class ConfigurationDbContextSeed
{
    public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
    {
        //callbacks urls from config:
        var clientUrls = new Dictionary<string, string>();

        clientUrls.Add("Mvc", "https://localhost:5002");

        if (!context.Clients.Any())
        {
            foreach (var client in Config.GetClients(clientUrls))
            {
                context.Clients.Add(client.ToEntity());
            }
            await context.SaveChangesAsync();
        }
   
        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.GetIdentityResources())
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.ApiResources.Any())
        {
            foreach (var api in Config.GetApiResources())
            {
                context.ApiResources.Add(api.ToEntity());
            }

            await context.SaveChangesAsync();
        }
    }
}