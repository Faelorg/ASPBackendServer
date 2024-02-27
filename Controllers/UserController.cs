using InterfaceServer.Modal;
using InterfaceServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InterfaceServer.Controllers
{

    [Authorize]
    [EnableRateLimiting("fixedwindow")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            this._service = service;
        }
        [DisableRateLimiting]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }
        [HttpPost]
        public async Task<ActionResult> Create(UserModal modal)
        {
            return Ok(await _service.Create(modal));
        }
        [HttpDelete]
        public async Task<ActionResult> Remove(string id)
        {
            return Ok(await _service.Remove(id));
        }
        [HttpPut]
        public async Task<ActionResult> Update(UserModal user,string id)
        {
            return Ok(await _service.Update(user,id));
        }

    }
}
