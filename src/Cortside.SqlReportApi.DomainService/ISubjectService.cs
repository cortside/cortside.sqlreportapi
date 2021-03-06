using System.Threading.Tasks;
using Cortside.SqlReportApi.Dto.Dto;

namespace Cortside.SqlReportApi.DomainService {
    public interface ISubjectService {
        Task Save(SubjectDto subject);
    }
}
