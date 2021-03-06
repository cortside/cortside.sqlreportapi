using System;
using System.Linq;
using Cortside.SqlReportApi.Data;
using Cortside.SqlReportApi.WebApi.IntegrationTests.Helpers.HotDocsMock;
using Cortside.SqlReportApi.WebApi.IntegrationTests.Helpers.IDSMock;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Cortside.SqlReportApi.WebApi.IntegrationTests {
    public class TestWebApplicationFactory<Startup> : WebApplicationFactory<Startup> where Startup : class {
        private IdsMock idsMock;
        protected override IHostBuilder CreateHostBuilder() {
            var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.integration.json", optional: false, reloadOnChange: true)
                 .Build();

            var server = new SampleWireMock()
                .ConfigureBuilder();
            this.idsMock = new IdsMock(server.mockServer);

            var authConfig = configuration.GetSection("IdentityServer");
            authConfig["Authority"] = idsMock.mockServer.Urls.First();
            authConfig["BaseUrl"] = $"{idsMock.mockServer.Urls.First()}/connect/token";
            authConfig["RequireHttpsMetadata"] = "false";

            var policyServerConfig = configuration.GetSection("PolicyServer");
            var policyserverTokenClient = policyServerConfig.GetSection("TokenClient");
            policyserverTokenClient["Authority"] = server.mockServer.Urls.First();
            policyServerConfig["PolicyServerUrl"] = server.mockServer.Urls.First();

            //var section = configuration.GetSection("HealthCheckHostedService");
            //section["Checks:1:Value"] = server.mockServer.Urls.First();
            //section["Checks:2:Value"] = server.mockServer.Urls.First();

            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => {
                    builder.AddConfiguration(configuration);
                })
                .ConfigureWebHostDefaults(webbuilder => {
                    webbuilder
                    .UseStartup<Startup>()
                    .UseSerilog();
                });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            builder.ConfigureServices(services => {
                // Remove the app's DbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<DatabaseContext>));

                if (descriptor != null) {
                    services.Remove(descriptor);
                }

                var dbName = $"DBNAME_{Guid.NewGuid()}";
                // Add DbContext using an in-memory database for testing.
                services.AddDbContext<DatabaseContext>(options => {
                    options.UseInMemoryDatabase(dbName);
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (DbContext).
                using (var scope = sp.CreateScope()) {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<DatabaseContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<TestWebApplicationFactory<Startup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}
