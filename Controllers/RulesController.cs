using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO;
using kroniiapi.Helper;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RulesController : ControllerBase
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IMegaHelper _uploadHelper;

        public RulesController(ICacheProvider cacheProvider, IMegaHelper uploadHelper)
        {
            _cacheProvider = cacheProvider;
            _uploadHelper = uploadHelper;
        }

        /// <summary>
        /// Upload the rules file
        /// </summary>
        /// <param name="file">the file</param>
        /// <returns>201: Uploaded / 400: File content inapproriate</returns>
        [HttpPost]
        public async Task<ActionResult> Upload([FromForm] IFormFile file)
        {
            (bool success, string message) = FileHelper.CheckDocExtension(file);
            if (!success)
            {
                return BadRequest(new ResponseDTO(400, message));
            }
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var name = file.FileName;
                var url = await _uploadHelper.Upload(stream, name, "Rules");
                await _cacheProvider.SetCache<string>("RulesURL", url);
            }
            return CreatedAtAction(nameof(Get), new ResponseDTO(201, "Uploaded"));
        }

        /// <summary>
        /// Get the link of the rules
        /// </summary>
        /// <returns>200: Ok with the link as message / 404: The link is not found (Not Uploaded)</returns>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            string url = await _cacheProvider.GetFromCache<string>("RulesURL");
            if (url is null)
            {
                return NotFound(new ResponseDTO(404, "The Rules is not found"));
            }
            return Ok(new ResponseDTO(200, url));
        }
    }
}