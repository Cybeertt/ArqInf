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
    public class ManagerController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        public InputModel Input { get; set; }

        public ManagerController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _context = context;
            _emailSender = emailSender;
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
                string email = user.Email;
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    string emailBody = "Caro " + email + ", a tua conta ArqInf foi eliminada.\r\n Em caso de dúvida contacte o administrador.";
                    emailBody = emailBody.Replace("\r\n", "<br/>");
                    await _emailSender.SendEmailAsync(email, "Eliminação do ArqInf", emailBody);
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

        /// <summary>
        ///  Executa a página corrente do email sender
        /// </summary>
        /// <returns>View IActionResult do email sender</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("SendEmailUsers")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailUsers(String subject, String emailBody)
        {
            emailBody = emailBody.Replace("\r\n", "<br/>");
            foreach (User user in _context.Users)
            {
                await _emailSender.SendEmailAsync(user.Email, subject, emailBody);
            }
            return Redirect(Url.Action("AdminManage", "Manager"));
        }

        [HttpPost, ActionName("SendEmailAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailAdmin(String subject, String emailBody)
        {
            var user = userManager.FindByNameAsync(User.Identity.Name).Result;
            var userInfo = "Utilizador: " + user.Email + "\r\n";
            emailBody = userInfo + emailBody;
            emailBody = emailBody.Replace("\r\n", "<br/>");
            await _emailSender.SendEmailAsync("ArqInfEng@gmail.com", subject, emailBody);
            return Redirect("/Identity/Account/Manage");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddUserToRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserToRole(InputModel Input)
        {
            var roleExist = await roleManager.FindByNameAsync(Input.RoleName);
            var userExist = await userManager.FindByNameAsync(Input.UserName);
            if (roleExist != null && userExist != null)
            {
                var oldRole = await userManager.GetRolesAsync(userExist);
                if(oldRole.Count > 0)
                {
                    await userManager.RemoveFromRoleAsync(userExist, oldRole.FirstOrDefault());
                }
                await userManager.AddToRoleAsync(userExist, Input.RoleName);
                Occupation occupation = _context.Occupation.FirstOrDefault(a => a.OccupationName == "Admin");
                if (occupation != null)
                {
                    userExist.Occupation = occupation;
                    await userManager.UpdateAsync(userExist);
                }
                
            }
            return View();
        }


        [Authorize(Roles = "Admin")]
        public IActionResult SendEmail(string id)
        {
            TempData["userId"] = id;
            return Redirect(Url.Action("SendEmailUser", "User"));

        }


        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("SendEmailUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailUser(string id, String subject, String emailBody)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                emailBody = emailBody.Replace("\r\n", "<br/>");
                await _emailSender.SendEmailAsync(user.Email, subject, emailBody);
                return Redirect(Url.Action("AdminManage", "Manager"));
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("InviteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteUser(String email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                string emailBody = "Caro " + email + ", foste convidado a juntar-te ao ArqInf.\r\n Segue o seguinte link para entrar: https://arqinfeng.azurewebsites.net/.";
                emailBody = emailBody.Replace("\r\n", "<br/>");
                await _emailSender.SendEmailAsync(email, "Convite AqInf", emailBody);
                return Redirect(Url.Action("AdminManage", "Manager"));
            }
            return View();
        }

        public IActionResult InviteUser()
        {
            return View();
        }

        public class InputModel
        {
            public string UserName { get; set; }
            public string RoleName { get; set; }
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        public IActionResult AdminManage()
        {
            var occupationContext = _context.Users.Include(a => a.Occupation).Where(a => a.Occupation.OccupationName != "Admin");
            return View(occupationContext);
        }

        public IActionResult EditUser()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("EditUser")]
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

                if (!string.IsNullOrEmpty(email))
                {
                    if (new EmailAddressAttribute().IsValid(email))
                    {
                        await EditEmail(user, email);
                        
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email não é válido");
                    }
                }
                if (ModelState.ErrorCount <= 1)
                {
                    return Redirect(Url.Action("AdminManage", "Manager"));
                }
            }
            
            return View();
            
        }


        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("EditEmail")]
        [ValidateAntiForgeryToken]
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

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("EditPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword(User user, String password)
        {
            if (user != null)
            {
                var token = userManager.GeneratePasswordResetTokenAsync(user).Result;
                var result = await userManager.ResetPasswordAsync(user, token, password);
            }
            return View();
        }

        [Authorize(Roles = "Admin,ProjectManager")]
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

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost, ActionName("GiveAssignment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiveAssignment(string id, string assignmentName)
        {
            Assignment assignment = _context.Assignment.FirstOrDefault(x => x.AssignmentName == assignmentName);
            UserAssignments userAssignments = new UserAssignments();
            User user = _context.Users.FindAsync(id).Result;
            bool assignmentAdded = false;
            foreach (var item in _context.UserAssignments.Include(a => a.Assignment).Where(a => a.User.Id == id))
            {
                if (item.Assignment != null)
                {
                    if (item.Assignment.Id == assignment.Id)
                    {
                        assignmentAdded = true;
                    }
                }    
            }
            if (!assignmentAdded)
            {
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
            else
            {
                ModelState.AddModelError(string.Empty, "Tarefa já foi atribuida a um utilizador.");
            }
            TempData["userId"] = id;
            return View(_context.Assignment.ToList());
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("AcceptVacation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptVacation(string id)
        {
            User user = _context.Users.FindAsync(id).Result;
            if (user.VacationStart != null && user.VacationEnd != null)
            {
                user.VacationAccepted = true;

                user.VacationDays = (int?)(user.VacationDays - (user.VacationEnd - user.VacationStart).Value.TotalDays);
            }
            user.VacationPendent = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            await _emailSender.SendEmailAsync(user.Email, "Confirmação da conta ArqInf",
               $"<html> <body> <p>Olá {user.Email},</p> <p>O teu pedido de férias foi aceite.</p> Cumprimentos.");

            return Redirect(Url.Action("AdminManage", "Manager"));

        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("RejectVacation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectVacation(string id)
        {
            User user = _context.Users.FindAsync(id).Result;
            user.InVacation = false;
            user.VacationPendent = false;
            user.VacationStart = null;
            user.VacationEnd = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            await _emailSender.SendEmailAsync(user.Email, "Confirmação da conta ArqInf",
               $"<html> <body> <p>Olá {user.Email},</p> <p>O teu pedido de férias não foi aceite, tenta noutro intervalo de tempo</p> Cumprimentos.");

            return Redirect(Url.Action("AdminManage", "Manager"));

        }

        [Authorize(Roles = "Admin")]
        public IActionResult PendentVacations()
        {
            var occupationContext = _context.Users.Include(a => a.Occupation).Where(a => 0 == 0);
            return View(occupationContext);
        }
    }
}

