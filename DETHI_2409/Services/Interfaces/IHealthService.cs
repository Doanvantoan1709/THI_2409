using DETHI_2409.DTOs;

namespace DETHI_2409.Services.Interfaces
{
    public interface IHealthService
    {
        Task<HealthDto> HealthCheckDB();
    }
}
