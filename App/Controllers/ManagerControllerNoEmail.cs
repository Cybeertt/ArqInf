using ArqInf.Data;
using ArqInf.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArqInf.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagerControllerNoEmail : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _context;
        public InputModel Input { get; set; }

        public ManagerControllerNoEmail(UserManager<User> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            _context = context;
        }
        /// <summary>
        ///  Executa a página corrente do index da aplicação
        /// </summary>
        /// <returns>View IActionResult do index da aplicação</returns>
        [Authorize(Roles="Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }
        /// <summary>
        ///  Executa a página corrente da criação do role
        /// </summary>
        /// <returns>View IActionResult do criação do role</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(Role role)
        {
            var roleExist = await roleManager.RoleExistsAsync(role.RoleName);
            if (!roleExist)
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role.RoleName));
            }
            return View();
        }
        /// <summary>
        ///  Executa a página corrente do menu de dar delete 
        /// </summary>
        /// <returns>View IActionResult da pagina de dar delete de qualque utilizador </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                foreach (var item in _context.UserAssignments)
                {
                    if (item.User != null)
                    {
                        if (item.User.Id == id)
                        {
                            _context.UserAssignments.Remove(item);
                        }
                    }
                }
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Redirect(Url.Action("AdminManage", "Manager"));
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }

        

        public class InputModel
        {
            public string UserName { get; set; }
            public string RoleName { get; set; }
        }

        /// <summary>
        ///  Executa a página corrente do profile do administrador
        /// </summary>
        /// <returns>View IActionResult da pagina do administrador</returns>

        public IActionResult AdminManage()
        {
            var occupationContext = _context.Users.Include(a => a.Occupation).Where(a => 0 == 0);
            return View(occupationContext);
        }

        public IActionResult EditUser()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(String id, String email, String password)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(password))
                {
                    List<string> passwordErrors = new List<string>();
                    var validators = userManager.PasswordValidators;
                    foreach (var validator in validators)
                    {
                        var result = await validator.ValidateAsync(userManager, user, password);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                passwordErrors.Add(error.Description);
                            }
                        }
                    }

                    if (passwordErrors.Count <= 1)
                    {
                        await EditPassword(user, password);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Password não é válida");
                    }

                }

                
                if (ModelState.ErrorCount <= 1)
                {
                    return Redirect(Url.Action("AdminManage", "Manager"));
                }
            }
            
            return View();
            
        }


        public async Task<IActionResult> EditEmail(User user, String email)
        {
            
            if (user != null)
            {
                var code = await userManager.GenerateChangeEmailTokenAsync(user, email);
                var result = await userManager.ChangeEmailAsync(user, email, code);
                user.UserName = email;
                await userManager.UpdateAsync(user);
            }
            return View();
        }


        public async Task<IActionResult> EditPassword(User user, String password)
        {
            if (user != null)
            {
                var token = userManager.GeneratePasswordResetTokenAsync(user).Result;
                var result = await userManager.ResetPasswordAsync(user, token, password);
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GiveAssignment(string? id)
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
            return View(_context.Assignment);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("GiveAssignment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiveAssignment(string id, string assignmentName)
        {
            Assignment assignment = _context.Assignment.FirstOrDefault(x => x.AssignmentName == assignmentName);
            UserAssignments userAssignments = new UserAssignments();
            User user = _context.Users.FindAsync(id).Result;
            var userContext = _context.Users.Include(a => a.Occupation).Where(a => a.Id == id);
            user.Occupation = userContext.Select(a => a.Occupation).FirstOrDefault();
            userAssignments.User = user;
            assignment.Budget = Math.Round((double)(user.Occupation.PayPerHour * assignment.AssignedHours), 2, MidpointRounding.AwayFromZero);
            _context.Assignment.Update(assignment);
            userAssignments.Assignment = assignment;
            _context.UserAssignments.Add(userAssignments);
            await _context.SaveChangesAsync();

            return Redirect(Url.Action("AdminManage", "Manager"));

        }

        public async Task<IActionResult> GiveOccupation(string? id)
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
            return View(_context.Occupation);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("GiveOccupation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiveOccupation(string id, string occupationName)
        {
            Occupation occupation = _context.Occupation.FirstOrDefault(x => x.OccupationName == occupationName);
            User user = await userManager.FindByIdAsync(id);
            user.Occupation = occupation;
            await userManager.UpdateAsync(user);
            return Redirect(Url.Action("AdminManage", "Manager"));

        }


        public IActionResult PendentVacations()
        {
            var occupationContext = _context.Users.Include(a => a.Occupation).Where(a => 0 == 0);
            return View(occupationContext);
        }
    }
}

