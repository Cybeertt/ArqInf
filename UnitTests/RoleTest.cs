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

namespace UnitTests
{
    public class RoleTest : IClassFixture<IdentityArqInfContextFixture>
    {

        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        /*public class InputModel
        {

            public string UserName { get; set; }
            public string RoleName { get; set; }


        }

        InputModel Input = new InputModel()
        {
            UserName = "Ricardo",
            RoleName = "Admin"
        };*/

        public RoleTest(IdentityArqInfContextFixture context, IEmailSender emailSender)
        {
            _context = context.DbContext;
            _emailSender = emailSender;
        }

        [Fact]
        public async Task AddRoleSystem()
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

            var controller = new ManagerController(mockUserManager.Object, mockRoleManager.Object, _context, _emailSender);


            Role role = new Role
            {
                RoleName = "Project Manager"
            };

        


        var result = await controller.CreateRole(role);
       // var result2 = await controller.AddUserToRole(Input);

        Assert.IsType<NotFoundResult>(result);
        //Assert.IsType<NotFoundResult>(result2);

        }
    }
}
