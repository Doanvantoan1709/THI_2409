using DETHI_2409.DTOs;
using DETHI_2409.Entities;
using DETHI_2409.Enums;
using DETHI_2409.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DETHI_2409.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly Dt2409Context _context;

        public ReportService(Dt2409Context context)
        {
            _context = context;
        }

        public async Task<List<ReportProject>> GetReportProject(ParameterProject parameterProject)
        {
            var query = from project in _context.Projects.AsQueryable().Where(x => x.IsActive == true)
                        join workItem in _context.WorkItems.AsQueryable()
                            on project.Id equals workItem.ProjectId into workItems
                        from workItem in workItems.DefaultIfEmpty()
                        select new
                        {
                            project,
                            workItem
                        };
 
            if (parameterProject.MinItems < 0) throw new ArgumentException("minItems phải là số nguyên không âm",nameof(parameterProject.MinItems));

            if (parameterProject.From.HasValue && parameterProject.To.HasValue && parameterProject.From > parameterProject.To)throw new ArgumentException("from không được muộn hơn to");


            if (parameterProject.From.HasValue)
            {
                var from = parameterProject.From.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                query = query.Where(x => x.workItem == null ||x.workItem.CreatedAt >= from);
            }

            if (parameterProject.To.HasValue)
            {
                var toExclusive = parameterProject.To.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                query = query.Where(x => x.workItem == null || x.workItem.CreatedAt < toExclusive);
            }

            query = query.Where(x => x.workItem == null || !x.workItem.IsDeleted);

            var rp = await query
                .GroupBy(
                    x => new
                    {
                        x.project.Id,
                        x.project.Code,
                        x.project.Name
                    }
                )
                .Select(
                    x => new ReportProject()
                    {
                        ProjectCode = x.Key.Code,
                        ProjectName = x.Key.Name,
                        TotalItems = x.Count(x => x.workItem != null),
                        OpenItems = x.Count(x => x.workItem != null && x.workItem.Status != EnumName.Done.ToString() && x.workItem.Status != EnumName.Cancelled.ToString()),
                        OverDueItems = x.Count(x => x.workItem != null && x.workItem.DueAt < DateTime.UtcNow && x.workItem.Status != EnumName.Done.ToString() && x.workItem.Status != EnumName.Cancelled.ToString()),
                        DoneItems = x.Count(x => x.workItem != null && x.workItem.Status == EnumName.Done.ToString()),
                        AverageCompletionHours =
                            Math.Round(
                                x.Where(x =>
                                    x.workItem != null &&
                                    x.workItem.Status == EnumName.Done.ToString() &&
                                    x.workItem.CompletedAt != null)
                                 .Select(x =>
                                    (double?)(
                                        (x.workItem.CompletedAt.Value -
                                         x.workItem.CreatedAt).TotalHours))
                                 .Average() ?? 0,
                                1)
                    }
                )
                .Where(x => x.TotalItems >= parameterProject.MinItems)
                .OrderBy(x => x.ProjectCode)
                .ToListAsync();

            return rp;
        }
    }
}
