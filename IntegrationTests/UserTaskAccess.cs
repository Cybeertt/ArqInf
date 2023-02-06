using ArqInf.Controllers;
using ArqInf.Data;
using ArqInf.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;

namespace IntegrationTests
{

    public class UserTaskAccess
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public UserTaskAccess()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
        }



        [Fact]
        public async Task Acesss_Tasks_User_login()
        {
            var httpResponse = await _client.GetAsync("/Identity/Account/Manage");

            // Assert: 
            Assert.Equal(HttpStatusCode.Redirect, httpResponse.StatusCode);
            Assert.StartsWith("http://localhost/Identity/Account/Login",
                httpResponse.Headers.Location.OriginalString);

        }

        [Fact]
        public async Task Acesss_Tasks_User_CanAccessPage()
        {
            UserAthentication userLogin = new UserAthentication(_client);
            userLogin.AuthenticateUser("rafaelbpalma@gmail.com", "Escola@123").Wait();

            // Act: request the "/Identity/Account/Manage" route
            //var httpResponse = await _client.GetAsync("/Users");
            var httpResponse = await _client.GetAsync("/Identity/Account/Manage");
            var httpResponse2 = await _client.GetAsync("/Assignment/MyAssignments/a816910b-d96b-4590-948e-2393d4f562af");
            // Assert: 
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, httpResponse2.StatusCode);
        }     

    }
}
