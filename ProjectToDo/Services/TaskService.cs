using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProjectToDo.Models;
using ProjectToDo.Services.Dto;
using System.Security.Claims;
using Task = ProjectToDo.Models.Task;

namespace ProjectToDo.Services
{
    public interface ITaskService
    {
        System.Threading.Tasks.Task InsertTask(TaskDto input);
        System.Threading.Tasks.Task UpdateTask(TaskDto input);
        System.Threading.Tasks.Task DeleteTask(int id);
        System.Threading.Tasks.Task<List<TaskDto>> GetTaskList();
        System.Threading.Tasks.Task<TaskDto?> GetTaskById(int id);
    }
    public class TaskService : ITaskService
    {
        private readonly ToDoDbContext _context;

        private ClaimsPrincipal UserClaim;
        public TaskService(IHttpContextAccessor httpContextAccessor, ToDoDbContext context)
        {
            UserClaim = httpContextAccessor.HttpContext.User;
            _context = context;
        }

        public async System.Threading.Tasks.Task DeleteTask(int id)
        {
            try
            {
                var cekData = (from a in _context.Tasks
                               where a.Id == id
                               select a);
                if (cekData.Any())
                {
                    var delete = await cekData.FirstOrDefaultAsync();
                    _context.Tasks.Remove(delete);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Task tidak ada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TaskDto?> GetTaskById(int id)
        {
            var getData = await _context.Tasks
                                        .Where(x => x.Id == id)
                                        .Include(t => t.User)
                                        .Include(t => t.Project)
                                        .Include(t => t.Status)
                                        .Select(x => new TaskDto
                                        {
                                            Id = x.Id,
                                            UserId = x.User.Id,
                                            UserName = x.User.Username,
                                            ProjectId = x.Project.Id,
                                            ProjectName = x.Project.Name,
                                            StatusId = x.Status.Id,
                                            Status = x.Status.Name,
                                            Title = x.Title,
                                            Description = x.Description,
                                            ExpiredDate = x.ExpiredDate,
                                        }).FirstOrDefaultAsync();
            return getData;
        }

        public async Task<List<TaskDto>> GetTaskList()
        {
            var getData = await _context.Tasks
                                        .Include(t => t.User)
                                        .Include(t => t.Project)
                                        .Include(t => t.Status)
                                        .Select(x => new TaskDto
                                        {
                                            Id = x.Id,
                                            UserId = x.User.Id,
                                            UserName = x.User.Username,
                                            ProjectId = x.Project.Id,
                                            ProjectName = x.Project.Name,
                                            StatusId = x.Status.Id,
                                            Status = x.Status.Name,
                                            Title = x.Title,
                                            Description = x.Description,
                                            ExpiredDate = x.ExpiredDate,
                                        }).ToListAsync();
            return getData;
        }

        public async System.Threading.Tasks.Task InsertTask(TaskDto input)
        {
            try
            {
                var roleId = UserClaim.Claims.First(c => c.Type == "RoleId").Value;
                var cekRole = (from a in _context.Roles
                               where a.Id.ToString() == roleId
                               select a.Name).FirstOrDefault();
                if (cekRole == "PM" && cekRole == "QA")
                {
                    var insertTask = new Task
                    {
                        UserId = input.UserId,
                        ProjectId = input.ProjectId,
                        StatusId = input.StatusId,
                        Title = input.Title,
                        Description = input.Description,
                        ExpiredDate = input.ExpiredDate,
                    };
                    await _context.Tasks.AddAsync(insertTask);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Role Anda bukan PM atau QA");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async System.Threading.Tasks.Task UpdateTask(TaskDto input)
        {
            try
            {
                var cekData = (from a in _context.Tasks
                               where a.Id == input.Id
                               select a);
                if (cekData.Any())
                {
                    var update = await cekData.FirstOrDefaultAsync();

                    update.UserId = input.UserId;
                    update.ProjectId = input.ProjectId;
                    update.StatusId = input.StatusId;
                    update.Title = input.Title;
                    update.Description = input.Description;
                    update.ExpiredDate = input.ExpiredDate;
                    _context.Tasks.Update(update);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Task tidak ada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

}
