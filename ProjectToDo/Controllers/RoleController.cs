using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ProjectToDo.Services.Dto;
using ProjectToDo.Services;
using Microsoft.AspNetCore.Authorization;

namespace ProjectToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _iRoleService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserController> _logger;
        private readonly string _cacheKey = "RolesCache";

        public RoleController(IRoleService iRoleService, IMemoryCache cache, ILogger<UserController> logger)
        {
            _iRoleService = iRoleService;
            _cache = cache;
            _logger = logger;
        }

        // GET: api/<RoleController>
        [HttpGet]
        public async Task<IActionResult> GetRoleList()
        {
            if (!_cache.TryGetValue(_cacheKey, out List<RoleDto> Roles))
            {
                // Data tidak ditemukan di cache, ambil dari sumber data
                Roles = await _iRoleService.GetRoleList();

                // Simpan data ke cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(_cacheKey, Roles, cacheEntryOptions);
            }

            return Ok(Roles);
        }

        // GET api/<RoleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            if (!_cache.TryGetValue(_cacheKey, out RoleDto? Role))
            {
                // Data tidak ditemukan di cache, ambil dari sumber data
                Role = await _iRoleService.GetRoleById(id);

                // Simpan data ke cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(_cacheKey, Role, cacheEntryOptions);
            }

            return Ok(Role);
        }

        // POST api/<RoleController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RoleDto input)
        {
            try
            {
                await _iRoleService.InsertRole(input);
                if (_cache.TryGetValue(_cacheKey, out List<RoleDto> Roles))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Roles = await _iRoleService.GetRoleList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Roles, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Tambah Role Gagal!"
                };

                return StatusCode(500, obj);
            }
        }

        // PUT api/<RoleController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RoleDto input)
        {
            try
            {
                input.Id = id;
                await _iRoleService.UpdateRole(input);
                if (_cache.TryGetValue(_cacheKey, out List<RoleDto> Roles))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Roles = await _iRoleService.GetRoleList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Roles, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Ubah Role Gagal!"
                };

                return StatusCode(500, obj);
            }
        }

        // DELETE api/<RoleController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _iRoleService.DeleteRole(id);
                if (_cache.TryGetValue(_cacheKey, out List<RoleDto> Roles))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Roles = await _iRoleService.GetRoleList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Roles, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Hapus Role Gagal!"
                };

                return StatusCode(500, obj);
            }
        }
    }
}
