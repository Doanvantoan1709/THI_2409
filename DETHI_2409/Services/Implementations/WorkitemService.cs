using DETHI_2409.DTOs;
using DETHI_2409.Entities;
using DETHI_2409.Enums;
using DETHI_2409.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace DETHI_2409.Services.Implementations
{
    public class WorkitemService : IWorkitemService
    {
        private readonly Dt2409Context _context;

        public WorkitemService(Dt2409Context context)
        {
            _context = context;
        }

        public async Task<WorkItemCustom> GetWorkItemsAsync(FilterWorkItems filter, PagingWorkItems paging, SortWorkItems sort)
        {
            // query
            var query = from workItem in _context.WorkItems.AsQueryable()
                        join project in _context.Projects.AsQueryable()
                            on workItem.ProjectId equals project.Id
                        join developer in _context.Developers.AsQueryable()
                            on workItem.AssigneeId equals developer.Id into developers
                                from developer in developers.DefaultIfEmpty()
                        select new
                        {
                            workItem,
                            project,
                            developer
                        };

            // filter
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(x => x.workItem.Title.ToLower().Contains(filter.Keyword.ToLower()));
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                List<string> lstStatus = filter.Status.Split(",").ToList();
                query = query.Where(x => x.workItem.Status.Contains(filter.Status.Distinct().ToString()));
            }

            if (!string.IsNullOrEmpty(filter.Priority))
            {
                query = query.Where(x => x.workItem.Priority.Contains(filter.Priority.Distinct().ToString()));
            }

            if (!string.IsNullOrEmpty(filter.ProjectCode))
            {
                query = query.Where(x => x.project.Code.ToLower().Contains(filter.ProjectCode.ToLower()));
            }

            if (filter.AssigneeId.HasValue)
            {
                query = query.Where(x => x.workItem.AssigneeId > 0 && x.workItem.AssigneeId == filter.AssigneeId);
            }

            if (filter.Overdue.HasValue == true)
            {
                if(filter.Overdue == true)
                {
                    query = query.Where(x => x.workItem.DueAt < DateTime.UtcNow);

                }
                else
                {
                    query = query.Where(x => x.workItem.DueAt > DateTime.UtcNow);
                }
            }

            // Paging
                if (paging.Page <= 0 || paging.PageSize <= 0) throw new Exception("Page, PageSize khong nho hon 0");
            query = query
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize);

            // Sort
            if (!string.IsNullOrEmpty(sort.Sort))
            {
                var isDesc = sort.Sort.StartsWith("-");
                var sortField = isDesc ? sort.Sort[1..] : sort.Sort;

                query = sortField switch
                {
                    "createdAt" => isDesc
                        ? query.OrderByDescending(x => x.workItem.CreatedAt)
                        : query.OrderBy(x => x.workItem.CreatedAt),

                    "dueAt" => isDesc
                        ? query.OrderByDescending(x => x.workItem.DueAt)
                        : query.OrderBy(x => x.workItem.DueAt),

                    "priority" => isDesc
                        ? query.OrderByDescending(x => x.workItem.Priority)
                        : query.OrderBy(x => x.workItem.Priority),

                    _ => query.OrderByDescending(x => x.workItem.CreatedAt)
                };
            }

            query = query
                .OrderBy(x =>
                    x.workItem.Priority == EnumPriority.Urgent.ToString() ? 1 :
                    x.workItem.Priority == EnumPriority.High.ToString() ? 2 :
                    x.workItem.Priority == EnumPriority.Normal.ToString() ? 3 :
                    x.workItem.Priority == EnumPriority.Low.ToString() ? 4 :
                    5
                );


            var list = await query
                .Select(
                    x => new WorkItems()
                    {
                        Id = x.workItem.Id,
                        Code = x.workItem.Code,
                        Title = x.workItem.Title,
                        Status = x.workItem.Status,
                        Priority = x.workItem.Priority,
                        ProjectCode = x.project.Code,
                        ProjectName = x.project.Name,
                        AssigneeName = x.developer.FullName,
                        DueAt = x.workItem.DueAt,
                        CreatedAt = x.workItem.CreatedAt,
                        Labels = x.workItem.Labels.Select(x => x.Name).ToArray()
                    }
                )
                .ToListAsync();

            WorkItemCustom res = new WorkItemCustom()
            {
                Page = paging.Page,
                PageSize = paging.PageSize,
                Total = list.Count(),
                Items = list
            };

            return res;
        }

        public async Task DeleteWorkItemAsync(int id)
        {
            var findWordItem = await _context.WorkItems.FirstOrDefaultAsync(x => x.Id == id);
            if (findWordItem == null) throw new Exception($"Id: {id} khong ton tai");

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (findWordItem.Status != EnumName.Todo.ToString() && findWordItem.Status != EnumName.Cancelled.ToString())
                {
                    throw new Exception($"Chi item Todo - Cancelled moi duoc phep xoa");
                }

                findWordItem.IsDeleted = true;
                findWordItem.DeletedAt = DateTime.UtcNow;
                findWordItem.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateWorkItemAsync(CreateWorkItem createWorkItem)
        {

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<WorkItemDetailDto> GetWorkItemDetailAsync(int id)
        {
            var findWordItem = await _context.WorkItems.FirstOrDefaultAsync(x => x.Id == id);
            if (findWordItem == null) throw new Exception($"Id: {id} khong ton tai");

            var item = new WorkItemDetail()
            {
                Id = findWordItem.Id,
                Code = findWordItem.Code,
                Title = findWordItem.Title,
                Description = findWordItem.Description,
                Status = findWordItem.Status,
                Priority = findWordItem.Priority,
                DueAt = findWordItem.DueAt,
                CreatedAt = findWordItem.CreatedAt,
                UpdatedAt = findWordItem.UpdatedAt,
                CompletedAt = findWordItem.CompletedAt
            };

            //var proj = new ProjectDetail()
            //{
            //    Code = findWordItem.Project.Code,
            //    Name = findWordItem.Project.Name
            //};

            //var dev = new DeveloperDetail()
            //{
            //    Id = findWordItem.Assignee.Id,
            //    FullName = findWordItem.Assignee.FullName,
            //    Code = findWordItem.Assignee.Code
            //};

            //var labelArr = findWordItem.Labels
            //    .OrderBy(x => x.Name).ToArray()
            //    .Select(x => x.Name).ToArray();

            //var hisArr = findWordItem.WorkItemHistories
            //    .Select(
            //        x => new WorkItemHistoryDetail()
            //        {
            //            FromStatus = x.FromStatus,
            //            ToStatus = x.ToStatus,
            //            Note = x.Note,
            //            Changeby = x.ChangedBy,
            //            CreatedAt = x.CreatedAt
            //        }
            //    )
            //    .OrderBy(y => y.CreatedAt)
            //    .ThenBy(y => y.Id)
            //    .ToArray();

            WorkItemDetailDto workItemDetailDto = new WorkItemDetailDto()
            {
                Item = item,
                //Project = proj,
                //Assignee = dev,
                //Labels = labelArr,
                //History = hisArr
            };


            return workItemDetailDto;

        }
    }
}
