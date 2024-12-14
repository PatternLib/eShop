using Microsoft.AspNetCore.Mvc;

namespace EShopOnContainers.WebMvc.Controllers;

public class CatalogController : Controller
{

    public IActionResult Index()
    {
        return View();
    }

}
