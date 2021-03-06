using System.Threading.Tasks;
using Cortside.SqlReportApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cortside.SqlReportApi.Data {

    public interface IDatabaseContext {
        DbSet<ReportGroup> ReportGroups { get; set; }
        DbSet<Report> Reports { get; set; }
        DbSet<ReportArgument> ReportArguments { get; set; }
        DbSet<ReportArgumentQuery> ReportArgumentQuerys { get; set; }
        DbSet<Subject> Subjects { get; set; }

        Task<int> SaveChangesAsync();

        DatabaseFacade Database { get; }
    }
}
