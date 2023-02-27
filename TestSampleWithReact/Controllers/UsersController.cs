using Microsoft.AspNetCore.Mvc;

namespace TestSampleWithReact.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
