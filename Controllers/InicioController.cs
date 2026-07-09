using Microsoft.AspNetCore.Mvc;

namespace Maquinarias.Controllers
{
    public class InicioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
