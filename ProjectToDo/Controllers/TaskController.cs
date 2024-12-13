using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ProjectToDo.Services.Dto;
using ProjectToDo.Services;
using Microsoft.AspNetCore.Authorization;

namespace ProjectToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _iTaskService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserController> _logger;
        private readonly string _cacheKey = "TasksCache";

        public TaskController(ITaskService iTaskService, IMemoryCache cache, ILogger<UserController> logger)
        {
            _iTaskService = iTaskService;
            _cache = cache;
            _logger = logger;
        }

        // GET: api/<TaskController>
        [HttpGet]
        public async Task<IActionResult> GetTaskList()
        {
            if (!_cache.TryGetValue(_cacheKey, out List<TaskDto> Tasks))
            {
                // Data tidak ditemukan di cache, ambil dari sumber data
                Tasks = await _iTaskService.GetTaskList();

                // Simpan data ke cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(_cacheKey, Tasks, cacheEntryOptions);
            }

            return Ok(Tasks);
        }

        // GET api/<TaskController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            if (!_cache.TryGetValue(_cacheKey, out TaskDto? Task))
            {
                // Data tidak ditemukan di cache, ambil dari sumber data
                Task = await _iTaskService.GetTaskById(id);

                // Simpan data ke cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(_cacheKey, Task, cacheEntryOptions);
            }

            return Ok(Task);
        }

        // POST api/<TaskController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TaskDto input)
        {
            try
            {
                await _iTaskService.InsertTask(input);
                if (_cache.TryGetValue(_cacheKey, out List<TaskDto> Tasks))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Tasks = await _iTaskService.GetTaskList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Tasks, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Tambah Task Gagal! " + ex.Message
                };

                return StatusCode(500, obj);
            }
        }

        // PUT api/<TaskController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TaskDto input)
        {
            try
            {
                input.Id = id;
                await _iTaskService.UpdateTask(input);
                if (_cache.TryGetValue(_cacheKey, out List<TaskDto> Tasks))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Tasks = await _iTaskService.GetTaskList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Tasks, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Ubah Task Gagal! " + ex.Message
                };

                return StatusCode(500, obj);
            }
        }

        // DELETE api/<TaskController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _iTaskService.DeleteTask(id);
                if (_cache.TryGetValue(_cacheKey, out List<TaskDto> Tasks))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Tasks = await _iTaskService.GetTaskList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Tasks, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Hapus Task Gagal! " + ex.Message
                };

                return StatusCode(500, obj);
            }
        }
    }
}
