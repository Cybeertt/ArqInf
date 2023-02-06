// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ArqInf.Data;
using ArqInf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArqInf.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment webHostEnvironment;  

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            webHostEnvironment = hostEnvironment;
    }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone(ErrorMessage = "Número de telemóvel inválido")]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Employee number")]
            public int EmployeeNumber { get; set; }

            [StringLength(15, ErrorMessage = "Primeiro nome precisa conter entre 1 e 15 letras", MinimumLength = 1)]
            [RegularExpression("[a-zA-Z]", ErrorMessage = "Nome apenas pode conter letras")]
            public string FirstName { get; set; }

            [StringLength(15, ErrorMessage = "Último nome precisa conter entre 1 e 15 letras", MinimumLength = 1)]
            [RegularExpression("[a-zA-Z]", ErrorMessage = "Nome apenas pode conter letras")]
            public string LastName { get; set; }

            [DataType(DataType.Date, ErrorMessage = "Data de nascimento inválida")]
            public DateTime BirthDate { get; set; }
            public char Gender { get; set; }
            public IFormFile ProfilePic { get; set; }

        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                EmployeeNumber = user.EmployeeNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return NotFound($"Impossível encontrar utilizador com o ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossível encontrar utilizador com o ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            //if (Input.PhoneNumber != phoneNumber)
            //{
            //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //    if (!setPhoneResult.Succeeded)
            //    {
            //        StatusMessage = "Unexpected error when trying to set phone number.";
            //        return RedirectToPage();
            //    }
            //}
            string uniqueFileName = UploadedFile(Input);


            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.BirthDate = Input.BirthDate;
            user.EmployeeNumber = Input.EmployeeNumber;
            user.Gender = Input.Gender;
            user.PhoneNumber = Input.PhoneNumber;
            user.ProfilePic = uniqueFileName;
            var result = await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            StatusMessage = "O seu perfil foi atualizado.";
            return RedirectToPage();
        }

        private string UploadedFile(InputModel model)
        {
            string uniqueFileName = null;

            if (model.ProfilePic != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/clients");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePic.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfilePic.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
