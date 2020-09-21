using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureBlobCMS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHome _home;
        public HomeController(IHome home)
        {
            _home = home;
        }
        [HttpGet("GetBlobData/{blobFile}")]
        public async Task<IActionResult> GetData(string blobFile)
        {
            var result = await _home.GetData(blobFile);
            if (result!=null)
                return Ok(result);
            return NotFound();
        }
        [HttpGet("GetBlobDataByLang/{blobFile}/{lang}")]
        public async Task<IActionResult> GetDataByLang(string blobFile,string lang)
        {
            var result = await _home.GetDataByLang(blobFile,lang);
            if (result != null)
                return Ok(result);
            return NotFound();
        }
        [HttpPost("UpdateBlobData/{blobfile}/{lang}")]
        public async Task<IActionResult> WriteDataByLang(string blobFile,string lang,[FromBody] Object home)
        {
            if (lang==null||!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _home.WriteDataByLang(blobFile,lang,home);
            if (result)
                return Ok(result);

            return NotFound();
        }
        [HttpPost("UpdateBlobData/{blobFile}")]
        public async Task<IActionResult> WriteData(string blobFile, [FromBody] Object home)
        {
            if ( !ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _home.WriteData(blobFile, home);
            if (result)
                return Ok(result);

            return NotFound();
        }

        [HttpGet("create_dynamic_page")]
        public async Task<IActionResult> CreateDynamicFiles(string fileName,string moduleName)
        {
            if (fileName == null && moduleName == null)
            {
                return BadRequest();
            }
            var res =await _home.CreateDynamicFile(fileName, moduleName);
            if (res != null)
            {
                return Ok(res);
            }
            return Conflict("Already Exist");
        }

        [HttpGet("GetModules")]
        public async Task<object> GetModules()
        {
          return Ok(await _home.GetModules());
        }
    }
}
