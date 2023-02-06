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
    public class UserVacation: IClassFixture<IdentityArqInfContextFixture>
    {
		private readonly ApplicationDbContext _context;

        DateTime date1 = new DateTime(2022, 4, 21, 7, 47, 0);

        User user;

        Assignment assignment;

        public UserVacation(IdentityArqInfContextFixture context)
        {
            _context = context.DbContext;

            user = new User()
            {
                FirstName = "Rafael",
                LastName = "Palma"
            };

            assignment = new Assignment()
            {
                Id = 123,
                AssignmentName = "adgijvn",
                LimitDate = date1.Date,
                AssignedHours = 20,
                FinishDate = date1.Date,
                Description = "aionfejn",
                Assigner = user
            };
        }


        [Fact]
        public async Task CreateVacation()
        {
            DateTime date1 = new DateTime(2022, 4, 21, 7, 47, 0);

            DateTime date2 = new DateTime(2022, 4, 27, 7, 47, 0);

            User user = new User
            {
                FirstName = "Rafael",
                LastName = "Palma"
            };

            Assignment assignment = new Assignment
            {
                Id = 123,
                AssignmentName = "adgijvn",
                LimitDate = date1.Date,
                AssignedHours = 20,
                FinishDate = date1.Date,
                Description = "aionfejn",
                Assigner = user
            };

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

            var controller = new UserController(_context);

            var controller2 = new AssignmentController(_context, mockUserManager.Object);

            

            var result = await controller.BookVacation(assignment.Id.ToString());

            //var result2 = await controller2.Create2(assignment);

            Assert.IsType<NotFoundResult>(result);

        }
		
		[Fact]
        public async Task EditVacation()
        {
            User user = new User
            {
                FirstName = "Rafael",
                LastName = "Palma"
            };

            var mockUserManager = new Mock<UserManager<Assignment>>(new Mock<IUserStore<Assignment>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<Assignment>>().Object,
                new IUserValidator<Assignment>[0],
                new IPasswordValidator<Assignment>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<Assignment>>>().Object);

            var controller = new UserController(_context);

            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => new Assignment
            {
               Id = 123,
               Assigner = user
            });

			var result2 = await controller.CancelVacationUser("123");

			Assert.IsType<NotFoundResult>(result2);

        }
		
		/*[Fact]
        public async Task DeleteVacation()
        {


            DateTime date1 = new DateTime(2022, 4, 21, 7, 47, 0);

            DateTime dateOnly = date1.Date;

            User user = new User
            {
                FirstName = "Rafael",
                LastName = "Palma"
            };

            Assignment assignment = new Assignment
            {

                Assigner = user
            };

            var mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            var controller = new AssignmentController(_context, mockUserManager.Object);

            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => null);

			var result3 = await controller.Delete(int.Parse("123"));

			Assert.IsType<NotFoundResult>(result3);

        }*/
		
		[Fact]
        public async Task ViewStatistic()
        {
            DateTime date1 = new DateTime(2022, 4, 21, 7, 47, 0);

            DateTime date2 = new DateTime(2022, 4, 27, 7, 47, 0);

            User user = new User
            {
                FirstName = "Rafael",
                LastName = "Palma"
            };

            Assignment assignment = new Assignment
            {
                Id = 123,
                AssignmentName = "adgijvn",
                LimitDate = date1.Date,
                AssignedHours = 20,
                FinishDate = date1.Date,
                Description = "aionfejn",
                Assigner = user
            };

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

            var controller = new StatisticsController(_context, mockUserManager.Object);

            //var controller2 = new AssignmentController(_context, mockUserManager.Object);            

            var result = controller.Index("a816910b-d96b-4590-948e-2393d4f562af");

            //var result2 = await controller2.Create2(assignment);

            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public async Task GeralStatistics()
        {
            DateTime date1 = new DateTime(2022, 4, 21, 7, 47, 0);

            DateTime date2 = new DateTime(2022, 4, 27, 7, 47, 0);

            User user = new User
            {
                FirstName = "Rafael",
                LastName = "Palma"
            };

            Assignment assignment = new Assignment
            {
                Id = 123,
                AssignmentName = "adgijvn",
                LimitDate = date1.Date,
                AssignedHours = 20,
                FinishDate = date1.Date,
                Description = "aionfejn",
                Assigner = user
            };

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

            var controller = new StatisticsController(_context, mockUserManager.Object);

            //var controller2 = new AssignmentController(_context, mockUserManager.Object);            

            var result = controller.GlobalStatistics();

            //var result2 = await controller2.Create2(assignment);

            Assert.IsType<ViewResult>(result);

        }

    }
}