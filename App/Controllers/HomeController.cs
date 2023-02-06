using ArqInf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ArqInf.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger,UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        /// <summary>
        ///  Executa a página corrente do index da aplicação
        /// </summary>
        /// <returns>View IActionResult do index da aplicação</returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///  Executa a página corrente das regras de privacidade da aplicação
        /// </summary>
        /// <returns>View IActionResult das regras de privacidade da aplicação</returns>
        public IActionResult Privacy()
        {
            return View();
        
        }

        //[HttpGet]
        //public IActionResult Profile()
        //{
        //    return View();
        //}


        //[HttpPost]
        //public async  Task<IActionResult> ProfilePic(IFormFile filepic)
        //{
        //    try
        //    {
        //    var user = await _userManager.GetUserAsync(User);
            
        //        string filename = filepic.FileName;
        //        filename = Path.GetFileName(filename);
        //        string uploadFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//img//clients", filename);

        //        var stream = new FileStream(uploadFile, FileMode.Create);
        //        await filepic.CopyToAsync(stream);
        //        user.ProfilePic = filepic.FileName;
        //        await _userManager.UpdateAsync(user);

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        Console.WriteLine("Error");
        //    }
        //    return RedirectToAction("Index");
           
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}