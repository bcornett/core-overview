using Microsoft.AspNetCore.Mvc;

namespace CoreDemo.Controllers 
{
    [Route("api/basic")]
    public class BasicController : Controller
    {
        public IActionResult Get()
        {
            return Json(new
            {
                Message = "Hello World from MVC!"
            });
        }
    }
}