
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Xunit;

namespace IntegrationTests
{

    public class RoleControllerTest
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public RoleControllerTest()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
        }



        [Fact]
        public async Task Role_Management_User_login()
        {
            var httpResponse = await _client.GetAsync("/Identity/Account/Manage");

            // Assert: 
            Assert.Equal(HttpStatusCode.Redirect, httpResponse.StatusCode);
            Assert.StartsWith("http://localhost/Identity/Account/Login",
                httpResponse.Headers.Location.OriginalString);

        }

        [Fact]
        public async Task Role_Management_User_CannotAccessPage()
        {
            UserAthentication userLogin = new UserAthentication(_client);
            userLogin.AuthenticateUser("rafaelbpalma@gmail.com", "Escola@123").Wait();

            // Act: request the "/Identity/Account/Manage" route
            //var httpResponse = await _client.GetAsync("/Users");
            var httpResponse = await _client.GetAsync("/Identity/Account/Manage");
            var httpResponse2 = await _client.GetAsync("/Manager/CreateRole/a816910b-d96b-4590-948e-2393d4f562af");
            // Assert: 
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.NotEqual(HttpStatusCode.OK, httpResponse2.StatusCode);
        }

        [Fact]
        public async Task Role_Management_User_CanAccessPage()
        {
            UserAthentication userLogin = new UserAthentication(_client);
            userLogin.AuthenticateUser("ricardocabrito@gmail.com", "Escola@123").Wait();

            // Act: request the "/Identity/Account/Manage" route
            //var httpResponse = await _client.GetAsync("/Users");
            var httpResponse = await _client.GetAsync("/Identity/Account/Manage");
            var httpResponse2 = await _client.GetAsync("/Manager/CreateRole/a816910b-d96b-4590-948e-2393d4f562af");
            // Assert: 
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, httpResponse2.StatusCode);
        }

    }

}
