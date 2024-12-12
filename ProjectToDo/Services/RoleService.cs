using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectToDo.Models;
using ProjectToDo.Services.Dto;

namespace ProjectToDo.Services
{
    public interface IRoleService
    {
        System.Threading.Tasks.Task InsertRole(RoleDto input);
        System.Threading.Tasks.Task UpdateRole(RoleDto input);
        System.Threading.Tasks.Task DeleteRole(int id);
        System.Threading.Tasks.Task<List<RoleDto>> GetRoleList();
        System.Threading.Tasks.Task<RoleDto?> GetRoleById(int id);
    }
    public class RoleService : IRoleService
    {
        private readonly ToDoDbContext _context;
        public RoleService(ToDoDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task DeleteRole(int id)
        {
            try
            {
                var cekData = (from a in _context.Roles
                               where a.Id == id
                               select a);
                if (cekData.Any())
                {
                    var delete = await cekData.FirstOrDefaultAsync();
                    _context.Roles.Remove(delete);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async System.Threading.Tasks.Task<RoleDto?> GetRoleById(int id)
        {
            var getData = await (from a in  _context.Roles 
                                 where a.Id == id
                                 select new RoleDto
                                 {
                                     Id = a.Id,
                                     Name = a.Name
                                 }).FirstOrDefaultAsync();
            return getData;
        }

        public async System.Threading.Tasks.Task<List<RoleDto>> GetRoleList()
        {
            var getData = await (from a in _context.Roles
                                 select new RoleDto
                                 {
                                     Id = a.Id,
                                     Name = a.Name
                                 }).ToListAsync();
            return getData;
        }

        public async System.Threading.Tasks.Task InsertRole(RoleDto input)
        {
            try
            {
                var cekData = (from a in _context.Roles
                               where a.Name == input.Name
                               select a).Any();
                if (!cekData)
                {
                    var insertRole= new Role
                    {
                        Name = input.Name,
                    };
                    await _context.Roles.AddAsync(insertRole);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Role ini sudah ada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async System.Threading.Tasks.Task UpdateRole(RoleDto input)
        {
            try
            {
                var cekData = (from a in _context.Roles
                               where a.Id == input.Id
                               select a);
                if (cekData.Any())
                {
                    var update = await cekData.FirstOrDefaultAsync();

                    update.Name = input.Name;
                    _context.Roles.Update(update);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Role tidak ada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
