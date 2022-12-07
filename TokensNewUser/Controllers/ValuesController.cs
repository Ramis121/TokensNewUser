using Domain.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TokensNewUser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly List<User> users;
        public ValuesController()
        {
            this.users = new List<User>();
        }

        [HttpGet]
        public IActionResult GetUser()
        {
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetIdUser(string id)
        {
            var res = users.FirstOrDefault(a => a.Id == id);
                if (res is not null)
                {
                    return Ok(res);
                }
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<User> Get(User user)
        {
            if (user.Id != null)
            {
                users.Add(user);
                return Ok(user);
            }
            return StatusCode(500);
        }
        
    }
}
