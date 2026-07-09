using Microsoft.AspNetCore.Mvc;
using FormularioMaquinaria.Models;

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

                // Guardar el nombre del usuario
                HttpContext.Session.SetString("NombreUsuario", model.Usuario);

                return RedirectToAction(
                    "Index",
                    "Admin");
            }

            ViewBag.Error = "Credenciales incorrectas";

            return View();
        }
    }
}