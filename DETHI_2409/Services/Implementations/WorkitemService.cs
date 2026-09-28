using DETHI_2409.DTOs;
using DETHI_2409.Entities;
using DETHI_2409.Enums;
using DETHI_2409.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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
                if (filter.Overdue == true)
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
            var findWordItem = await _context.WorkItems
                .FirstOrDefaultAsync(x => x.Id == id);

            if (findWordItem.IsDeleted == true) throw new Exception($"Work Item co Id: {id} da bi xoa truoc do");

            if (findWordItem == null) throw new Exception($"Id: {id} khong ton tai");

            if (findWordItem.Status != EnumName.Todo.ToString() && findWordItem.Status != EnumName.Cancelled.ToString())
            {
                throw new Exception($"Chi item Todo - Cancelled moi duoc phep xoa");
            }

            findWordItem.IsDeleted = true;
            findWordItem.DeletedAt = DateTime.UtcNow;
            findWordItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task CreateWorkItemAsync(CreateWorkItem createWorkItem)
        {

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var getprojectId = await _context.Projects
                    .Where(x => x.Code == createWorkItem.ProjectCode)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();

                if (getprojectId == 0) throw new Exception($"ProjectCode: {createWorkItem.ProjectCode} khong ton tai");

                var labelNames = createWorkItem.Labels?
                    .Select(x => x.Trim().ToLower())
                    .Distinct()
                    .ToArray() ?? [];

                var labels = await _context.Labels
                    .Where(x => labelNames.Contains(x.Name.ToLower()))
                    .ToListAsync();

                var now = DateTime.UtcNow;

                WorkItem workItem = new WorkItem()
                {
                    Code = "",
                    Title = createWorkItem.Title,
                    Description = createWorkItem.Description,
                    Status = createWorkItem.Status,
                    Priority = createWorkItem.Priority,
                    ProjectId = getprojectId,
                    AssigneeId = createWorkItem.AssigneeId,
                    DueAt = createWorkItem.DueAt,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CompletedAt = null,
                    IsDeleted = false,
                    DeletedAt = null,
                    Labels = labels
                };

                await _context.WorkItems.AddAsync(workItem);
                await _context.SaveChangesAsync(); // sinh Id

                workItem.Code = $"WI-{now.Year}-{workItem.Id:D6}"; // lay Id

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<WorkItemDetailDto> GetWorkItemDetailAsync(int id)
        {
            var res = await _context.WorkItems
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(
                    x => new WorkItemDetailDto()
                    {
                        Item = new WorkItemDetail()
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Title = x.Title,
                            Description = x.Description,
                            Status = x.Status,
                            Priority = x.Priority,
                            DueAt = x.DueAt,
                            CreatedAt = x.CreatedAt,
                            UpdatedAt = x.UpdatedAt,
                            CompletedAt = x.CompletedAt
                        },

                        Project = x.Project == null
                            ? null
                            : new ProjectDetail()
                            {
                                Code = x.Project.Code,
                                Name = x.Project.Name
                            },

                        Assignee = x.Assignee == null
                            ? null
                            : new DeveloperDetail()
                            {
                                Id = x.Assignee.Id,
                                FullName = x.Assignee.FullName,
                                Code = x.Assignee.Code
                            },

                        Labels = x.Labels
                            .OrderBy(l => l.Name)
                            .Select(l => l.Name)
                            .ToArray(),

                        History = x.WorkItemHistories
                            .OrderBy(h => h.CreatedAt)
                            .ThenBy(h => h.Id)
                            .Select(h => new WorkItemHistoryDetail
                            {
                                FromStatus = h.FromStatus,
                                ToStatus = h.ToStatus,
                                Note = h.Note,
                                Changeby = h.ChangedBy,
                                CreatedAt = h.CreatedAt
                            })
                            .ToArray()
                    }
                )
                .FirstOrDefaultAsync();

            if (res == null) throw new Exception($"Id: {id} khong ton tai");

            return res;

        }

        public async Task AssignTasks(int id, ParameterItem parameterItem)
        {
            var workItem = await _context.WorkItems.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

            if (workItem == null) throw new Exception("Work Item này đã bị xóa. Vui lòng phân công cho Work Item khác");

            if (workItem.Status == EnumName.Done.ToString() && workItem.Status == EnumName.Done.ToString())
            {
                throw new Exception($"Không thể giao việc cho Work Item có trạng thái HỦY/HOÀN THÀNH");
            }

            Developer? devExsists = null;

            if (parameterItem.AssigneeId.HasValue)
            {
                devExsists = await _context.Developers
                    .FirstOrDefaultAsync(x =>
                        x.Id == parameterItem.AssigneeId.Value &&
                        x.IsActive);

                if (devExsists == null)
                {
                    throw new Exception(
                        $"Assignee Id: {parameterItem.AssigneeId} không tồn tại hoặc không active");
                }
            }


            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                workItem.AssigneeId = parameterItem.AssigneeId;
                workItem.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var history = new WorkItemHistory()
                {
                    WorkItemId = workItem.Id,
                    FromStatus = workItem.Status,
                    ToStatus = workItem.Status,
                    ChangedBy = devExsists.Code,
                    Note = string.IsNullOrWhiteSpace(parameterItem.Note) ? null : parameterItem.Note,
                    CreatedAt = DateTime.UtcNow
                };

                _context.WorkItemHistories.Add(history);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<HistoryDto[]> GetHistoryAsync(long id, DateTime? from, DateTime? to)
        {
            if (id < 1) throw new Exception("ID không nhỏ hơn 1");

            var workItem = await _context.WorkItems
                .Where(x => x.Id == id && x.IsDeleted == false)
                .Select(x => new
                {
                    x.Id, x.IsDeleted
                })
                .FirstOrDefaultAsync();
            if (workItem == null || workItem.IsDeleted == true) throw new Exception("Công việc không tồn tại hoặc đã bị xóa");

            var history = await _context.WorkItemHistories
                .Where(x => x.WorkItemId == workItem.Id)
                .Select(
                    x => new HistoryDto()
                    {
                        Id = x.Id,
                        FromStatus = x.FromStatus,
                        ToStatus = x.ToStatus,
                        Note = x.Note,
                        ChangedBy = x.ChangedBy,
                        CreatedAt = x.CreatedAt.Date
                    }
                )
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToArrayAsync();

            if (from.HasValue && to.HasValue)
            {
                if (to < from)
                {
                    throw new Exception("from không được phép muộn hơn to");
                }
            }

            if (from.HasValue)
            {
                history = history.Where(x => x.CreatedAt >= from).ToArray();
            }

            if (to.HasValue)
            {
                history = history.Where(x => x.CreatedAt <= to).ToArray();
            }

            return history;

        }

        public async Task<HistoryDto> WriteAndDisplayNoteAsync(long id, NoteHistory note)
        {
            var workItem = await _context.WorkItems.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (workItem == null || workItem.IsDeleted == true) throw new Exception("Không ghi chú cho công việc không tồn tại hoặc đã bị xóa");

            if (string.IsNullOrEmpty(note.note)) throw new Exception("note phải là chuỗi và không được để trống");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var history = new WorkItemHistory()
                {
                    WorkItemId = workItem.Id,
                    FromStatus = workItem.Status,
                    ToStatus = workItem.Status,
                    Note = note.note.Trim(),
                    ChangedBy = "api",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Add(history);
                await _context.SaveChangesAsync();

                workItem.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var res = new HistoryDto()
                {
                    Id = history.Id,
                    FromStatus = history.FromStatus,
                    ToStatus = history.ToStatus,
                    Note = history.Note,
                    ChangedBy = history.ChangedBy,
                    CreatedAt = history.CreatedAt
                };

                return res;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
