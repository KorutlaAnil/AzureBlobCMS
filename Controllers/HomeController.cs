using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureBlobCMS.Interfaces;
using AzureBlobCMS.Models;
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
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var result = await _home.GetData();
            if (result!=null)
                return Ok(result);
            return NotFound();
        }
        [HttpGet("{lang}")]
        public async Task<IActionResult> GetDataByLang(string lang)
        {
            var result = await _home.GetDataByLang(lang);
            if (result != null)
                return Ok(result);
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> WriteDataByLang(string lang, Home home)
        {
            if (lang==null||!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _home.WriteDataByLang(lang,home);
            if (result)
                return Ok(result);

            return NotFound();
        }
    }
}
