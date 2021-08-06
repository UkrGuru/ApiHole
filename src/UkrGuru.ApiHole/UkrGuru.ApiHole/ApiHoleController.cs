using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using UkrGuru.ApiHole.Helpers;
using UkrGuru.SqlJson;

namespace UkrGuru.ApiHole
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        private readonly string _prefix;
        private readonly AuthService _auth;
        private readonly DbService _db;

        public DefaultController(IOptions<AppSettings> appSettings, AuthService auth, DbService db)
        {
            _prefix = appSettings.Value.ApiProcPefix;
            _auth = auth;
            _db = db;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] string apiholekey)
        {
            var response = _auth.Authenticate(apiholekey);

            if (response == null)
                return BadRequest(new { message = "ApiHoleKey is incorrect" });

            return Ok(response);
        }

        // GET: <proc>
        [HttpGet("{proc}")]
        [Authorize]
        public async Task<string> Get(string proc)
        {
            return await _db.FromProcAsync($"{_prefix}{proc}");
        }

        // GET <proc>/<id>
        [HttpGet("{proc}/{id}")]
        [Authorize]
        public async Task<string> Get(string proc, string id)
        {
            return await _db.FromProcAsync($"{_prefix}{proc}", id);
        }

        // POST <proc>
        [HttpPost("{proc}")]
        [Authorize]
        public async Task<string> Post(string proc, [FromBody] string item)
        {
            return await _db.FromProcAsync($"{_prefix}{proc}", item);
        }

        // PUT <proc>/<id>
        [HttpPut("{proc}/{id}")]
        [Authorize]
        public async Task Put(string proc, string id, [FromBody] string item)
        {
            await _db.ExecProcAsync($"{_prefix}{proc}", item);
        }

        // DELETE <proc>/<id>
        [HttpDelete("{proc}/{id}")]
        [Authorize]
        public async Task Delete(string proc, string id)
        {
            await _db.ExecProcAsync($"{_prefix}{proc}", id);
        }
    }
}