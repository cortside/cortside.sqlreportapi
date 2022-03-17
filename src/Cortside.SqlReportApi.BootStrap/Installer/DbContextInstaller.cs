using Cortside.Common.BootStrap;
using Cortside.SqlReportApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.SqlReportApi.BootStrap.Installer {
    public class DbContextInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddDbContext<DatabaseContext>(opt => {
                opt.UseSqlServer(configuration.GetSection("SqlReport").GetValue<string>("ConnectionString"));
                opt.EnableSensitiveDataLogging();
            });

            // for DbContextCheck
            services.AddTransient<DbContext, DatabaseContext>();
        }
    }
}
