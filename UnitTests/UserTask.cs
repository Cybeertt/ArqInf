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
    public class Usertask: IClassFixture<IdentityArqInfContextFixture>
    {
		private readonly ApplicationDbContext _context;
        
		public Usertask(IdentityArqInfContextFixture context)
        {
            _context = context.DbContext;
        }

        
		User user = new User
        {
            FirstName = "Rafael",
            LastName = "Palma"
        };
		
		Occupation occupation = new Occupation
        {
            Id = 123,
            OccupationName = "Owner",
			PayPerHour = 1200.50
        };

        [Fact]
        public async Task CreateTask()
        {

            Occupation occupation = new Occupation
            {
                Id = 123,
                OccupationName = "Owner",
                PayPerHour = 1200.50
            };
            var mockUserManager = new Mock<UserManager<Occupation>>(new Mock<IUserStore<Occupation>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<Occupation>>().Object,
                new IUserValidator<Occupation>[0],
                new IPasswordValidator<Occupation>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<Occupation>>>().Object);

            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string userId) => new Occupation
					{
						Id = 123,
						OccupationName = "Owner",
						PayPerHour = 1200.50
					});

            var controller = new OccupationController(_context);

            var result = await controller.CreateOccupation(occupation);

            //Assert.IsType<NotFoundResult>(result);

        }
		
		[Fact]
        public async Task EditTask()
        {
            Occupation occupation = new Occupation
            {
                Id = 123,
                OccupationName = "Owner",
                PayPerHour = 1200.50
            };
            var mockUserManager = new Mock<UserManager<Occupation>>(new Mock<IUserStore<Occupation>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<Occupation>>().Object,
                new IUserValidator<Occupation>[0],
                new IPasswordValidator<Occupation>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<Occupation>>>().Object);

            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string userId) => new Occupation
					{
						OccupationName = "Owner",
						PayPerHour = 1200.50
					});

            var controller = new OccupationController(_context);


            var result2 = await controller.Edit(int.Parse("123"));

			Assert.IsType<NotFoundResult>(result2);

        }
		
		[Fact]
        public async Task DeleteTask()
        {

            var mockUserManager = new Mock<UserManager<Occupation>>(new Mock<IUserStore<Occupation>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<Occupation>>().Object,
                new IUserValidator<Occupation>[0],
                new IPasswordValidator<Occupation>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<Occupation>>>().Object);

            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync((string userId) => new Occupation
					{
						OccupationName = "Owner",
						PayPerHour = 1200.50
					});

            var controller = new OccupationController(_context);

            var result3 = await controller.Delete(int.Parse("123"));

			Assert.IsType<NotFoundResult>(result3);

        }
		
    }
}
