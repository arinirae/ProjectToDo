using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectToDo.Models;
using ProjectToDo.Services.Dto;

namespace ProjectToDo.Services
{
    public interface IStatusService
    {
        System.Threading.Tasks.Task InsertStatus(StatusDto input);
        System.Threading.Tasks.Task UpdateStatus(StatusDto input);
        System.Threading.Tasks.Task DeleteStatus(int id);
        System.Threading.Tasks.Task<List<StatusDto>> GetStatusList();
        System.Threading.Tasks.Task<StatusDto?> GetStatusById(int id);
    }
    public class StatusService : IStatusService
    {
        private readonly ToDoDbContext _context;
        public StatusService(ToDoDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task DeleteStatus(int id)
        {
            try
            {
                var cekData = (from a in _context.Status
                               where a.Id == id
                               select a);
                if (cekData.Any())
                {
                    var delete = await cekData.FirstOrDefaultAsync();
                    _context.Status.Remove(delete);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Status tidak ada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async System.Threading.Tasks.Task<StatusDto?> GetStatusById(int id)
        {
            var getData = await (from a in  _context.Status
                                 where a.Id == id
                                 select new StatusDto
                                 {
                                     Id = a.Id,
                                     Name = a.Name
                                 }).FirstOrDefaultAsync();
            return getData;
        }

        public async System.Threading.Tasks.Task<List<StatusDto>> GetStatusList()
        {
            var getData = await (from a in _context.Status
                                 select new StatusDto
                                 {
                                     Id = a.Id,
                                     Name = a.Name
                                 }).ToListAsync();
            return getData;
        }

        public async System.Threading.Tasks.Task InsertStatus(StatusDto input)
        {
            try
            {
                var cekData = (from a in _context.Status
                               where a.Name == input.Name
                               select a).Any();
                if (!cekData)
                {
                    var insertStatus= new Status
                    {
                        Name = input.Name,
                    };
                    await _context.Status.AddAsync(insertStatus);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Status ini sudah ada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async System.Threading.Tasks.Task UpdateStatus(StatusDto input)
        {
            try
            {
                var cekData = (from a in _context.Status
                               where a.Id == input.Id
                               select a);
                if (cekData.Any())
                {
                    var update = await cekData.FirstOrDefaultAsync();

                    update.Name = input.Name;
                    _context.Status.Update(update);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Status ini tidak ada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
