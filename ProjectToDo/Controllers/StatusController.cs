using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ProjectToDo.Services.Dto;
using ProjectToDo.Services;

namespace ProjectToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _iStatusService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserController> _logger;
        private readonly string _cacheKey = "StatussCache";

        public StatusController(IStatusService iStatusService, IMemoryCache cache, ILogger<UserController> logger)
        {
            _iStatusService = iStatusService;
            _cache = cache;
            _logger = logger;
        }

        // GET: api/<StatusController>
        [HttpGet]
        public async Task<IActionResult> GetStatusList()
        {
            if (!_cache.TryGetValue(_cacheKey, out List<StatusDto> Statuss))
            {
                // Data tidak ditemukan di cache, ambil dari sumber data
                Statuss = await _iStatusService.GetStatusList();

                // Simpan data ke cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(_cacheKey, Statuss, cacheEntryOptions);
            }

            return Ok(Statuss);
        }

        // GET api/<StatusController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatusById(int id)
        {
            if (!_cache.TryGetValue(_cacheKey, out StatusDto? Status))
            {
                // Data tidak ditemukan di cache, ambil dari sumber data
                Status = await _iStatusService.GetStatusById(id);

                // Simpan data ke cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(_cacheKey, Status, cacheEntryOptions);
            }

            return Ok(Status);
        }

        // POST api/<StatusController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StatusDto input)
        {
            try
            {
                await _iStatusService.InsertStatus(input);
                if (_cache.TryGetValue(_cacheKey, out List<StatusDto> Statuss))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Statuss = await _iStatusService.GetStatusList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Statuss, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Tambah Status Gagal!"
                };

                return StatusCode(500, obj);
            }
        }

        // PUT api/<StatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] StatusDto input)
        {
            try
            {
                input.Id = id;
                await _iStatusService.UpdateStatus(input);
                if (_cache.TryGetValue(_cacheKey, out List<StatusDto> Statuss))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Statuss = await _iStatusService.GetStatusList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Statuss, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Ubah Status Gagal!"
                };

                return StatusCode(500, obj);
            }
        }

        // DELETE api/<StatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _iStatusService.DeleteStatus(id);
                if (_cache.TryGetValue(_cacheKey, out List<StatusDto> Statuss))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Statuss = await _iStatusService.GetStatusList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Statuss, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Hapus Status Gagal!"
                };

                return StatusCode(500, obj);
            }
        }
    }
}
