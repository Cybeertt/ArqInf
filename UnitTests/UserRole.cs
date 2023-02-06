using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class UserRole
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;


        public UserRole()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
        }

        private string _role;
        public async Task CreateRole(string role)
        {
            _role = role;
        }

        /*[Fact]
        public async Task AccessCreateRoleAuthorize()
        {
            // Act: request the "/Utentes" route
            UserAthentication userLogin = new UserAthentication(_client);
            userLogin.AuthenticateUser("antonio.pedro.mil@gmail.com", "Antonio.2022").Wait();

            // Act: request the "/Identity/Account/Manage" route
            //var httpResponse = await _client.GetAsync("/Users");
            var httpResponse = await _client.GetAsync("/Manager/CreateRole");
            // Assert: 
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact]
        public async Task AccessCreateRoleNotAuthorize()
        {
            // Act: request the "/Utentes" route
            UserAthentication userLogin = new UserAthentication(_client);
            userLogin.AuthenticateUser("antonio.pedro.mil@gmail.com", "Antonio.2022").Wait();

            // Act: request the "/Identity/Account/Manage" route
            //var httpResponse = await _client.GetAsync("/Users");
            var httpResponse = await _client.GetAsync("/Manager/CreateRole");
            // Assert: 
            Assert.NotEqual(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact]
        public async Task AccessAddUserToRoleAuthorize()
        {
            // Act: request the "/Utentes" route
            UserAthentication userLogin = new UserAthentication(_client);
            userLogin.AuthenticateUser("antonio.pedro.mil@gmail.com", "Antonio.2022").Wait();

            // Act: request the "/Identity/Account/Manage" route
            //var httpResponse = await _client.GetAsync("/Users");
            var httpResponse = await _client.GetAsync("/Manager/AddUserToRole");
            // Assert: 
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact]
        public async Task AccessAddUserToRoleNotAuthorize()
        {
            // Act: request the "/Utentes" route
            UserAthentication userLogin = new UserAthentication(_client);
            userLogin.AuthenticateUser("antonio.pedro.mil@gmail.com", "Antonio.2022").Wait();

            // Act: request the "/Identity/Account/Manage" route
            //var httpResponse = await _client.GetAsync("/Users");
            var httpResponse = await _client.GetAsync("/Manager/AddUserToRole");
            // Assert: 
            Assert.NotEqual(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact]
        public async Task AccessAdminManageAuthorize()
        {
            // Act: request the "/Utentes" route
            UserAthentication userLogin = new UserAthentication(_client);
            userLogin.AuthenticateUser("antonio.pedro.mil@gmail.com", "Antonio.2022").Wait();

            // Act: request the "/Identity/Account/Manage" route
            //var httpResponse = await _client.GetAsync("/Users");
            var httpResponse = await _client.GetAsync("/Manager/AdminManage");
            // Assert: 
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact]
        public async Task AccessAdminManageNotAuthorize()
        {
            // Act: request the "/Utentes" route
            UserAthentication userLogin = new UserAthentication(_client);
            userLogin.AuthenticateUser("antonio.pedro.mil@gmail.com", "Antonio.2022").Wait();

            // Act: request the "/Identity/Account/Manage" route
            //var httpResponse = await _client.GetAsync("/Users");
            var httpResponse = await _client.GetAsync("/Manager/AdminManage");
            // Assert: 
            Assert.NotEqual(HttpStatusCode.OK, httpResponse.StatusCode);
        }*/
    }
}