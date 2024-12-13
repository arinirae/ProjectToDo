using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectToDo.Models;
using ProjectToDo.Services;
using ProjectToDo.Services.Dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectToDo.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _iUserService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserController> _logger;
        private readonly string _cacheKey = "UsersCache";

        public UserController(IUserService iUserService, IMemoryCache cache, ILogger<UserController> logger)
        {
            _iUserService = iUserService;
            _cache = cache;
            _logger = logger;
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            if (!_cache.TryGetValue(_cacheKey, out List<UserDto> users))
            {
                // Data tidak ditemukan di cache, ambil dari sumber data
                users = await _iUserService.GetUserList();

                // Simpan data ke cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(_cacheKey, users, cacheEntryOptions);
            }

            return Ok(users);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (!_cache.TryGetValue(_cacheKey, out UserDto? user))
            {
                // Data tidak ditemukan di cache, ambil dari sumber data
                user = await _iUserService.GetUserById(id);

                // Simpan data ke cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(_cacheKey, user, cacheEntryOptions);
            }

            return Ok(user);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto input)
        {
            try
            {
                await _iUserService.InsertUser(input);
                if (_cache.TryGetValue(_cacheKey, out List<UserDto> users))
                {
                    users = await _iUserService.GetUserList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, users, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex) 
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Tambah User Gagal! " + ex.Message
                };

                return StatusCode(500, obj);
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDto input)
        {
            try
            {
                input.Id = id;
                await _iUserService.UpdateUser(input);
                if (_cache.TryGetValue(_cacheKey, out List<UserDto> users))
                {
                    users = await _iUserService.GetUserList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, users, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Ubah User Gagal! " + ex.Message
                };

                return StatusCode(500, obj);
            }
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _iUserService.DeleteUser(id);
                if (_cache.TryGetValue(_cacheKey, out List<UserDto> users))
                {
                    users = await _iUserService.GetUserList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, users, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Hapus User Gagal! " + ex.Message
                };

                return StatusCode(500, obj);
            }
        }
    }
}
