// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using UkrGuru.SqlJson;

namespace UkrGuru.ApiHole
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        private readonly string _procprefix;
        private readonly string _apiholekey;
        private readonly DbService _db;

        public DefaultController(IOptions<AppSettings> appSettings, DbService db)
        {
            _procprefix = appSettings.Value.ApiProcPefix;
            _apiholekey = appSettings.Value.ApiHoleKey;
            _db = db;
        }

        private void CheckAccess()
        {
            if (!string.IsNullOrEmpty(_apiholekey) && Request.Headers["ApiHoleKey"] != _apiholekey)
            {
                throw new Exception("Access is denied");
            }
        }

        [HttpGet("{proc}")]
        public async Task<string> Get(string proc, string data = null)
        {
            try
            {
                CheckAccess();

                return await _db.FromProcAsync($"{_procprefix}{proc}", data);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}. Proc={proc}";
            }
        }

        [HttpPost("{proc}")]
        public async Task<dynamic> Post(string proc, [FromBody] dynamic data = null)
        {
            try
            {
                CheckAccess();

                return await _db.FromProcAsync<dynamic>($"{_procprefix}{proc}", (object)data == null ? null : data);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}. Proc={proc}";
            }
        }
    }
}