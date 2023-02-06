using Microsoft.AspNetCore.Mvc;
using ArqInf.Models;
using ArqInf.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArqInf.Controllers
{
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _context;
        
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        ///  Executa a página corrente do profile do utilizador
        /// </summary>
        /// <returns>View IActionResult da profile corrente</returns>
        public IActionResult Profile()
        {
            return View();
        }
        
        /// <summary>
        ///  Executa a página corrente do email sender
        /// </summary>
        /// <returns>View IActionResult da página de email sender</returns>
        public IActionResult SendEmail()
        {
            return View();
        }

        public IActionResult SendEmailAdmin()
        {
            return View();
        }
        public IActionResult SendEmailUser()
        {
            return View();
        }

        public async Task<IActionResult> BookVacation(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            TempData["userId"] = id;
            return View(_context.Users);
        }

        [HttpPost, ActionName("BookVacation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookVacation(string id, DateTime VacationStart, DateTime VacationEnd)
        {
            User user = _context.Users.FindAsync(id).Result;
            user.VacationPendent = true;
            user.VacationStart = VacationStart;
            user.VacationEnd = VacationEnd;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Redirect(Url.Content("/Identity/Account/Manage"));

        }

        public async Task<IActionResult> CancelVacationUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            TempData["userId"] = id;
            return View(_context.Users);
        }

        [HttpPost, ActionName("CancelVacation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelVacation(string id)
        {
            User user = _context.Users.FindAsync(id).Result;
            if (user.InVacation == false)
            {
                if(user.VacationPendent == false && user.VacationStart != null && user.VacationEnd != null)
                {
                    user.VacationDays = (int?)(user.VacationDays + (user.VacationEnd - user.VacationStart).Value.TotalDays);
                }
                user.VacationPendent = false;
                user.VacationStart = null;
                user.VacationEnd = null;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            return Redirect(Url.Content("/Identity/Account/Manage"));

        }
    }
}
