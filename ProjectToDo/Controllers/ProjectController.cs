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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _iProjectService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserController> _logger;
        private readonly string _cacheKey = "ProjectsCache";

        public ProjectController(IProjectService iProjectService, IMemoryCache cache, ILogger<UserController> logger)
        {
            _iProjectService = iProjectService;
            _cache = cache;
            _logger = logger;
        }

        // GET: api/<ProjectController>
        [HttpGet]
        public async Task<IActionResult> GetProjectList()
        {
            if (!_cache.TryGetValue(_cacheKey, out List<ProjectDto> Projects))
            {
                // Data tidak ditemukan di cache, ambil dari sumber data
                Projects = await _iProjectService.GetProjectList();

                // Simpan data ke cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(_cacheKey, Projects, cacheEntryOptions);
            }

            return Ok(Projects);
        }

        // GET api/<ProjectController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            if (!_cache.TryGetValue(_cacheKey, out ProjectDto? Project))
            {
                // Data tidak ditemukan di cache, ambil dari sumber data
                Project = await _iProjectService.GetProjectById(id);

                // Simpan data ke cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(_cacheKey, Project, cacheEntryOptions);
            }

            return Ok(Project);
        }

        // POST api/<ProjectController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectDto input)
        {
            try
            {
                await _iProjectService.InsertProject(input);
                if (_cache.TryGetValue(_cacheKey, out List<ProjectDto> Projects))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Projects = await _iProjectService.GetProjectList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Projects, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Tambah Project Gagal!"
                };

                return StatusCode(500, obj);
            }
        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProjectDto input)
        {
            try
            {
                input.Id = id;
                await _iProjectService.UpdateProject(input);
                if (_cache.TryGetValue(_cacheKey, out List<ProjectDto> Projects))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Projects = await _iProjectService.GetProjectList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Projects, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Ubah Project Gagal!"
                };

                return StatusCode(500, obj);
            }
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _iProjectService.DeleteProject(id);
                if (_cache.TryGetValue(_cacheKey, out List<ProjectDto> Projects))
                {
                    // Data tidak ditemukan di cache, ambil dari sumber data
                    Projects = await _iProjectService.GetProjectList();

                    // Simpan data ke cache
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Waktu hidup cache
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(_cacheKey, Projects, cacheEntryOptions);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                _logger.LogError(ex.Message);

                var obj = new
                {
                    msg = "Hapus Project Gagal!"
                };

                return StatusCode(500, obj);
            }
        }
    }
}
