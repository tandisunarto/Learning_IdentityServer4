using IdentityServerHost.Quickstart.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Auth.Extensions
{
    public static class IdentityServer4Extension
    {
        public static IIdentityServerBuilder SetupIdentityServer4(
            this IServiceCollection services,
            string connectionString
        )
        {
            return services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
            .AddTestUsers(TestUsers.Users)
            // this adds the config data from DB (clients, resources, CORS)
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlite(connectionString);
            })
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlite(connectionString);

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
            });
        }
    }
}