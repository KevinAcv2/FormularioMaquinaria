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
            if (model.Usuario == "admin" &&
               model.Password == "Maquinaria2026")
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