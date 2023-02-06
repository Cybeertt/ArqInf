// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ArqInf.Models;
using ArqInf.Data;
using Microsoft.EntityFrameworkCore;

namespace ArqInf.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;

        public LoginModel(SignInManager<User> signInManager, ILogger<LoginModel> logger, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Insira um email")]
            [EmailAddress(ErrorMessage = "Email inválido")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Insira uma password")]
            [DataType(DataType.Password, ErrorMessage = "Password inválida")]
            [StringLength(100, ErrorMessage = "Password necessita pelo menos 8 caracteres contendo: 1 letra maiúscula, 1 número e um carácter especial", MinimumLength = 8)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                User user = _context.Users.FirstOrDefault(x => x.Email == Input.Email);
                DateTime timeNow = DateTime.Now;
                if (result.Succeeded)
                {
                    if (user.VacationDaysGiven.Value.Month < timeNow.Month)
                    {
                        user.VacationDays = user.VacationDays + ((timeNow.Month - user.VacationDaysGiven.Value.Month) * 2);

                        user.VacationDaysGiven = timeNow;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    if (user.VacationStart != null && user.VacationEnd != null && user.VacationAccepted == true)
                    {
                        if (DateTime.Compare(user.VacationStart.Value, timeNow) < 0)
                        {
                            user.InVacation = true;                         
                            var assignmentContext = _context.UserAssignments.Include(a => a.Assignment.Assigner).Where(a => 0 == 0);
                            var assignmentList = assignmentContext.Where(a => a.User.Id == user.Id).Select(a => a.Assignment);
                            foreach (var item in assignmentList)
                            {
                                if (item != null)
                                {
                                    item.LimitDate = (DateTime)(item.LimitDate + (user.VacationEnd - user.VacationStart));
                                    _context.Assignment.Update(item);
                                }
                            }
                            _context.Users.Update(user);
                            await _context.SaveChangesAsync();
                        }
                        if (DateTime.Compare(user.VacationEnd.Value, timeNow) > 0 && user.InVacation.Value)
                        {
                            user.InVacation = false;
                            user.VacationAccepted = false;
                            user.VacationEnd = null;
                            user.VacationStart = null;
                           
                            _context.Users.Update(user);
                            await _context.SaveChangesAsync();
                        }
                    }
                    
                    _logger.LogInformation("Utilizador autenticado com sucesso");
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Dados inseridos inválidos");
                }
                if (result.RequiresTwoFactor)
                {
                    if (user.VacationDaysGiven.Value.Month < timeNow.Month)
                    {
                        user.VacationDays = user.VacationDays + ((timeNow.Month - user.VacationDaysGiven.Value.Month) * 2);

                        user.VacationDaysGiven = timeNow;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    if (user.VacationStart != null && user.VacationEnd != null && user.VacationAccepted == true)
                    {
                        if (DateTime.Compare(user.VacationStart.Value, timeNow) < 0)
                        {
                            user.InVacation = true;
                            var assignmentContext = _context.UserAssignments.Include(a => a.Assignment.Assigner).Where(a => 0 == 0);
                            var assignmentList = assignmentContext.Where(a => a.User.Id == user.Id).Select(a => a.Assignment);
                            foreach (var item in assignmentList)
                            {
                                if (item != null)
                                {
                                    item.LimitDate = (DateTime)(item.LimitDate + (user.VacationEnd - user.VacationStart));
                                    _context.Assignment.Update(item);
                                }
                            }
                            _context.Users.Update(user);
                            await _context.SaveChangesAsync();
                        }
                        if (DateTime.Compare(user.VacationEnd.Value, timeNow) > 0 && user.InVacation.Value)
                        {
                            user.InVacation = false;
                            user.VacationAccepted = false;
                            user.VacationEnd = null;
                            user.VacationStart = null;

                            _context.Users.Update(user);
                            await _context.SaveChangesAsync();
                        }
                    }
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
