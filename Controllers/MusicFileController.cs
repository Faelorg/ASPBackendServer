using InterfaceServer.Helpers;
using InterfaceServer.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InterfaceServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicFileController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        public MusicFileController(IWebHostEnvironment environment) { 
        
            this.environment = environment;
        
        }

        [HttpPut]
        public async Task<IActionResult> UploadMusic(IFormFile file)
        {
            APIResponse response = new APIResponse();
            try
            {
                string id = Guid.NewGuid().ToString();
                string path = this.environment.WebRootPath + "\\Upload\\" + id+".mp3";

                using (FileStream stream = System.IO.File.Create(path))
                {
                    await stream.CopyToAsync(stream);
                }

                response.ResponseCode = 200;
                response.Result = "Succes";
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage=ex.Message;
            }
            return Ok(response);
        }
    }
}
