using DETHI_2409.DTOs;
using DETHI_2409.Entities;
using DETHI_2409.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DETHI_2409.Services.Implementations
{
    public class HealthService : IHealthService
    {
        private readonly Dt2409Context _context;

        public HealthService(Dt2409Context context)
        {
            _context = context;
        }

        public async Task<HealthDto> HealthCheckDB()
        {
            var res = new HealthDto();
            try
            {
                await _context.Database.OpenConnectionAsync();
                await _context.Database.CloseConnectionAsync();

                res.Status = "ok";
                res.DbConnected = true;
                return res;
            }
            catch
            {
                res.Status = "unavailable";
                res.DbConnected = false;
                return res;
            }
        }
    }
}
