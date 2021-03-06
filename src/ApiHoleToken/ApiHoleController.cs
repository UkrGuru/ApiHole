// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using UkrGuru.ApiHole.Helpers;
using UkrGuru.SqlJson;

namespace UkrGuru.ApiHole
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly DbService _db;
        private readonly string _procprefix;

        public DefaultController(AuthService auth, DbService db, IOptions<AppSettings> appSettings)
        {
            _auth = auth;
            _db = db;
            _procprefix = appSettings.Value.ApiProcPefix;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] string apiholekey)
        {
            var response = _auth.Authenticate(apiholekey);

            if (response == null)
                return BadRequest(new { message = "ApiHoleKey is incorrect" });

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{proc}")]
        public async Task<string> Get(string proc, string data = null)
        {
            try
            {
                return await _db.FromProcAsync($"{_procprefix}{proc}", data);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}. Proc={proc}";
            }
        }

        [Authorize]
        [HttpPost("{proc}")]
        public async Task<dynamic> Post(string proc, [FromBody] dynamic data = null)
        {
            try
            {
                return await _db.FromProcAsync<dynamic>($"{_procprefix}{proc}", (object)data == null ? null : data);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}. Proc={proc}";
            }
        }
    }
}