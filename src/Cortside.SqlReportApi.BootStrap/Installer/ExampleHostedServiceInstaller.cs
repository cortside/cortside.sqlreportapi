using Cortside.Common.BootStrap;
using Cortside.SqlReportApi.Configuration;
using Cortside.SqlReportApi.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.SqlReportApi.BootStrap.Installer {
    public class ExampleHostedServiceInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddSingleton(configuration.GetSection("ExampleHostedService").Get<ExampleHostedServiceConfiguration>());
            services.AddHostedService<ExampleHostedService>();
        }
    }
}
