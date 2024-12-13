using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectToDo.Models;
using ProjectToDo.Services.Dto;
using System.Security.Claims;

namespace ProjectToDo.Services
{
    public interface IUserService
    {
        System.Threading.Tasks.Task InsertUser (UserDto input);
        System.Threading.Tasks.Task UpdateUser (UserDto input);
        System.Threading.Tasks.Task DeleteUser (int id);
        System.Threading.Tasks.Task<List<UserDto>> GetUserList ();
        System.Threading.Tasks.Task<UserDto?> GetUserById(int id);
    }
    public class UserService : IUserService
    {
        private readonly ToDoDbContext _context;

        private ClaimsPrincipal UserClaim;
        public UserService(IHttpContextAccessor httpContextAccessor, ToDoDbContext context)
        {
            UserClaim = httpContextAccessor.HttpContext.User;
            _context = context;
        }

        public async System.Threading.Tasks.Task DeleteUser(int id)
        {
            try
            {
                var roleId = UserClaim.Claims.First(c => c.Type == "RoleId").Value;
                var cekRole = (from a in _context.Roles
                               where a.Id.ToString() == roleId
                               select a.Name).FirstOrDefault();
                if (cekRole == "Admin")
                {
                    var cekData = (from a in _context.Users
                                   where a.Id == id
                                   select a);
                    if (cekData.Any())
                    {
                        var delete = await cekData.FirstOrDefaultAsync();
                        _context.Users.Remove(delete);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("User ini tidak ada");
                    }
                }
                else
                {
                    throw new Exception("Role Anda bukan Admin");
                }

            }
            catch (Exception ex)
            { 
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserDto?> GetUserById(int id)
        {
            var getData = await _context.Users
                                        .Where(x => x.Id == id)
                                        .Include(p => p.Role)
                                        .Select(x => new UserDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            RoleId = x.RoleId,
                                            Username = x.Username,
                                            Role = x.Role.Name
                                        }).FirstOrDefaultAsync();
            return getData;
        }

        public async Task<List<UserDto>> GetUserList()
        {
            var getData = await _context.Users
                                        .Include(p => p.Role)
                                        .Select(x => new UserDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            RoleId = x.RoleId,
                                            Username = x.Username,
                                            Role = x.Role.Name
                                        }).ToListAsync();
            return getData;
        }

        public async System.Threading.Tasks.Task InsertUser(UserDto input)
        {
            try
            {
                var roleId = UserClaim.Claims.First(c => c.Type == "RoleId").Value;
                var cekRole = (from a in _context.Roles
                               where a.Id.ToString() == roleId
                               select a.Name).FirstOrDefault();
                if (cekRole == "Admin")
                {
                    var cekData = (from a in _context.Users
                                   where a.Username == input.Username
                                   select a).Any();
                    if (!cekData)
                    {
                        var passwordHasher = new PasswordHasher<User>();
                        var insertUser = new User
                        {
                            Name = input.Name,
                            Username = input.Username,
                            RoleId = input.RoleId,
                            Password = passwordHasher.HashPassword(new User(), input.Password),
                        };
                        await _context.Users.AddAsync(insertUser);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("User ini sudah ada");
                    }
                }
                else
                {
                    throw new Exception("Role Anda Bukan Admin");
                }
            }
            catch (Exception ex) 
            { 
                throw new Exception(ex.Message);
            }
        }

        public async System.Threading.Tasks.Task UpdateUser(UserDto input)
        {
            try
            {
                var cekData = (from a in _context.Users
                               where a.Id == input.Id
                               select a);
                if (cekData.Any())
                {
                    var update = await cekData.FirstOrDefaultAsync();
                    var passwordHasher = new PasswordHasher<User>();

                    update.Name = input.Name;
                    update.Username = input.Username;
                    update.RoleId = input.RoleId;
                    update.Password = passwordHasher.HashPassword(new User(), input.Password);
                    _context.Users.Update(update);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("User ini tidak ada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
