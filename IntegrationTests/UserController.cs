using ArqInf.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class UserController
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public UserController()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
        }

        [Fact]
        public async Task Index_WhenNonAuthenticatedUser_IsRediretctedToLoginPage()
        {
            // Act: request the "/Utentes" route
            var httpResponse = await _client.GetAsync("/Identity/Account/Manage");

            // Assert: 
            Assert.Equal(HttpStatusCode.Redirect, httpResponse.StatusCode);
            Assert.StartsWith("http://localhost/Identity/Account/Login",
                httpResponse.Headers.Location.OriginalString);
        }

        [Fact]
        public async Task Index_WhenAuthenticatedUser_CanAccessPage()
        {
            UserAthentication userLogin = new UserAthentication(_client);
            userLogin.AuthenticateUser("rafaelbpalma@gmail.com", "Escola@123").Wait();

            // Act: request the "/Identity/Account/Manage" route
            //var httpResponse = await _client.GetAsync("/Users");
            var httpResponse = await _client.GetAsync("/Identity/Account/Manage");
            // Assert: 
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact]
        public void UserData()
        {
            User user1 = new User();
            int n;
            var today = DateTime.Today;
            // Calculate the age.
            DateTime birthdate = new DateTime(2004, 7, 26);
            
            user1.Gender = 'M';
            user1.PhoneNumber = "91564315";
            user1.BirthDate = birthdate;

            var age = today.Subtract(user1.BirthDate).TotalDays;

            var years = (age / 365);

            n = user1.PhoneNumber.Length;

            Assert.NotEqual('F', user1.Gender);
            Assert.True(12 >= n && n <= 9);
            var tomorrow = today.AddDays(1);
            Assert.NotEqual(tomorrow.Date.ToString(), DateTime.Now.Date.ToString());
            Assert.False(Math.Round(years) > 18);
        }
        
        /*[Fact]
        public async Task Index_WhenNonRegistedUser_IsRediretctedToRegisterPage()
        {
            // Act: request the "/Identity/Account/Register" route
            var httpResponse = await _client.GetAsync("/Identity/Account/Register");

            // Assert: 
            Assert.Equal(HttpStatusCode.Redirect, httpResponse.StatusCode);
            Assert.StartsWith("http://localhost/Identity/Account/Register",
                httpResponse.Headers.Location.OriginalString);
        }

        [Fact]
        public async Task Index_WhenRegistedUser_CanAccessPage()
        {
            UserRegister userRegister = new UserRegister(_client);
            userRegister.RegisterUser("pedro@gmail.pt", "Aula@123", "Aula@123").Wait();

            // Act: request the "/Utentes" route
            var httpResponse = await _client.GetAsync("/Identity/Account/Register");

            // Assert: 
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }*/
    }
}
