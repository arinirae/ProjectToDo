using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectToDo.Models;
using ProjectToDo.Services.Dto;

namespace ProjectToDo.Services
{
    public interface IProjectService
    {
        System.Threading.Tasks.Task InsertProject(ProjectDto input);
        System.Threading.Tasks.Task UpdateProject(ProjectDto input);
        System.Threading.Tasks.Task DeleteProject(int id);
        System.Threading.Tasks.Task<List<ProjectDto>> GetProjectList();
        System.Threading.Tasks.Task<ProjectDto?> GetProjectById(int id);
    }
    public class ProjectService : IProjectService
    {
        private readonly ToDoDbContext _context;
        public ProjectService(ToDoDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task DeleteProject(int id)
        {
            try
            {
                var cekData = (from a in _context.Projects
                               where a.Id == id
                               select a);
                if (cekData.Any())
                {
                    var delete = await cekData.FirstOrDefaultAsync();
                    _context.Projects.Remove(delete);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProjectDto?> GetProjectById(int id)
        {
            var getData = await _context.Projects
                                        .Where(x => x.Id == id)
                                        .Include(p => p.User)
                                        .Select(x => new ProjectDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            UserPMId = x.UserPMId,
                                            UserPM = x.User.Name
                                        }).FirstOrDefaultAsync();
            return getData;
        }

        public async Task<List<ProjectDto>> GetProjectList()
        {
            var getData = await _context.Projects
                                        .Include(p => p.User)
                                        .Select(x => new ProjectDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            UserPMId = x.UserPMId,
                                            UserPM = x.User.Name
                                        }).ToListAsync();
            return getData;
        }

        public async System.Threading.Tasks.Task InsertProject(ProjectDto input)
        {
            try
            {
                var cekData = (from a in _context.Projects
                               where a.Name == input.Name
                               select a).Any();
                if (!cekData)
                {
                    var passwordHasher = new PasswordHasher<Project>();
                    var insertProject = new Project
                    {
                        Name = input.Name,
                        UserPMId = input.UserPMId
                    };
                    await _context.Projects.AddAsync(insertProject);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Project ini sudah ada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async System.Threading.Tasks.Task UpdateProject(ProjectDto input)
        {
            try
            {
                var cekData = (from a in _context.Projects
                               where a.Id == input.Id
                               select a);
                if (cekData.Any())
                {
                    var update = await cekData.FirstOrDefaultAsync();
                    var passwordHasher = new PasswordHasher<Project>();

                    update.Name = input.Name;
                    update.UserPMId = input.UserPMId;
                    _context.Projects.Update(update);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Project ini tidak ada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
