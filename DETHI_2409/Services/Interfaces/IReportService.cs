using DETHI_2409.DTOs;

namespace DETHI_2409.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<ReportProject>> GetReportProject(ParameterProject parameterProject);
    }
}
