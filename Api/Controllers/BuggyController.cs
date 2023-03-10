 using Api.Data;
using Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;

        }
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> AuthendicationError()
        {
            return "secret test";
        }
        [HttpGet("not-found")]
        public ActionResult<AppUsers> GetNotFound()
        {
            var thing = _context.Users.Find(-1);
            if (thing == null)
                return NotFound(); 
            return Ok(thing);
        }
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing = _context.Users.Find(-1).ToString();
            return thing;  
        }
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This is a bad requestt");
        }
    }
}
