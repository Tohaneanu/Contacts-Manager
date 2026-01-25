using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(temp => temp.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }
                //var dbConnectionDescriptor = services.SingleOrDefault(
                //    d => d.ServiceType ==
                //        typeof(DbConnection));
                //if (dbConnectionDescriptor != null)
                //{
                //    services.Remove(dbConnectionDescriptor);
                //}
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                });
            });
        }
    }
}
