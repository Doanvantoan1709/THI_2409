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

        public bool HealthCheckDB()
        {
            try
            {
                _context.Database.OpenConnection();
                _context.Database.CloseConnection();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
