using System;
using System.Net.Http;
using System.Threading.Tasks;
using ArqInf.Controllers;
using ArqInf.Data;
using ArqInf.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Text.RegularExpressions;

namespace UnitTests
{
    public class UserTest : IClassFixture<IdentityArqInfContextFixture>
    {
		private readonly ApplicationDbContext _context;
        
		public UserTest(IdentityArqInfContextFixture context)
        {
            _context = context.DbContext;
        }
		
		
		[Fact]
        public async Task InvalidEmail()
        {

            string mail = "rafaelbpalmagmail.com";

            string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";

            Regex.IsMatch(mail, regex, RegexOptions.IgnoreCase);

        }
		
		[Fact]
        public async Task InvalidGender()
        {
            User user1 = new User();
			
			user1.Gender = 'M';
			
			Assert.NotEqual('F', user1.Gender);

        }

		[Fact]
        public async Task InvalidPhoneNumber()
        {
            User user1 = new User();

            int n;

            user1.PhoneNumber = "91564315";

            n = user1.PhoneNumber.Length;

            Assert.True(12 >= n && n <= 9);

        }
		
		[Fact]
        public async Task InvalidBirthDate()
        {
            User user1 = new User();
			
			var today = DateTime.Today;
            // Calculate the age.
            DateTime birthdate = new DateTime(2004, 7, 26);
			
			user1.BirthDate = birthdate;

            var age = today.Subtract(user1.BirthDate).TotalDays;

            var years = (age / 365);

            Assert.False(Math.Round(years) > 18);

        }
		
		[Fact]
        public async Task InvalidCreateDate()
        {
            User user1 = new User();
			
			var today = DateTime.Today;
            // Calculate the age.
            DateTime birthdate = new DateTime(2004, 7, 26);
			
			user1.BirthDate = birthdate;
			
			
            var tomorrow = today.AddDays(1);
            Assert.NotEqual(tomorrow.Date.ToString(), DateTime.Now.Date.ToString());


        }

        [Fact]
        public async Task UpdateUser()
        {
            var mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string userId) => null);

            var controller = new ManagerControllerNoEmail(mockUserManager.Object, _context);

            User user = new User
            {
                Id = "123",
                FirstName = "Rafael",
                LastName = "Palma"
            };

            var result = await controller.EditUser("123", "rafaelbpalma@gmail.com", "Escola@123");

            Assert.IsType<NotFoundResult>(result);

        }
		
		[Fact]
        public async Task UpdateEmail()
        {
            var mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(new Mock<IRoleStore<IdentityRole>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<IdentityRole>>().Object,
                new IRoleValidator<IdentityRole>[0],
                new IPasswordValidator<IdentityRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string userId) => null);

            mockRoleManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string roleId) => null);

            var controller = new ManagerControllerNoEmail(mockUserManager.Object, _context);

            User user = new User
            {
                Id = "123",
                FirstName = "Rafael",
                LastName = "Palma"
            };

            var result2 = await controller.EditEmail(user, "201600649@estudantes.ips.pt");

			Assert.IsType<NotFoundResult>(result2);

        }
		
		[Fact]
        public async Task UpdatePassword()
        {
            var mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(new Mock<IRoleStore<IdentityRole>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<IdentityRole>>().Object,
                new IRoleValidator<IdentityRole>[0],
                new IPasswordValidator<IdentityRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string userId) => null);

            mockRoleManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string roleId) => null);

            var controller = new ManagerControllerNoEmail(mockUserManager.Object, _context);

            User user = new User
            {
                Id = "123",
                FirstName = "Rafael",
                LastName = "Palma"
            };

            var result3 = await controller.EditPassword(user, "Escola@Ips");

			Assert.IsType<NotFoundResult>(result3);

        }
		
		[Fact]
        public async Task DeleteUser()
        {
            var mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(new Mock<IRoleStore<IdentityRole>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<IdentityRole>>().Object,
                new IRoleValidator<IdentityRole>[0],
                new IPasswordValidator<IdentityRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string userId) => null);

            mockRoleManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string roleId) => null);

            var controller = new ManagerControllerNoEmail(mockUserManager.Object, _context);

            User user = new User
            {
                Id = "123",
                FirstName = "Rafael",
                LastName = "Palma"
            };

            var result4 = await controller.DeleteUser("123");
			
			Assert.IsType<NotFoundResult>(result4);

        }
    }
}
