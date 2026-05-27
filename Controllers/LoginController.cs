using Microsoft.AspNetCore.Mvc;
using Maquinarias.Models;

namespace Maquinarias.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (model.Usuario == "Daniel" &&
               model.Password == "IngDaniel")
            {
                HttpContext.Session.SetString("Admin", "SI");

                return RedirectToAction(
                    "Index",
                    "Admin");
            }

            ViewBag.Error = "Credenciales incorrectas";

            return View();
        }
    }
}