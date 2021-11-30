// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.Resource;
using System;
using System.Threading.Tasks;
using UkrGuru.SqlJson;

namespace UkrGuru.ApiHole
{
    [Authorize]
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        private readonly DbService _db;
        private readonly string _procprefix;

        public DefaultController(DbService db, IOptions<AppSettings> appSettings)
        {
            _db = db;
            _procprefix = appSettings.Value.ApiProcPefix;
        }

        // The Web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API
        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        [HttpGet("{proc}")]
        public async Task<string> Get(string proc, string data = null)
        {
            try
            {
                HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

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
                HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

                return await _db.FromProcAsync<dynamic>($"{_procprefix}{proc}", (object)data == null ? null : data);
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}. Proc={proc}";
            }
        }
    }
}