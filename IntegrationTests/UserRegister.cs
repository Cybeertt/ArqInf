using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class UserRegister
    {
        protected static string REGISTER_COOKIE = ".AspNetCore.Identity.";

        protected SetCookieHeaderValue _antiforgeryCookie;
        protected string _antiforgeryToken;
        protected HttpClient _client;

        public UserRegister(HttpClient client)
        {
            _client = client;
        }

        protected static Regex AntiforgeryFormFieldRegex = new Regex(@"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");

        protected async Task<string> EnsureAntiforgeryToken()
        {
            if (_antiforgeryToken != null) return _antiforgeryToken;

            var response = await _client.GetAsync("/Identity/Account/Register");
            response.EnsureSuccessStatusCode();
            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                _antiforgeryCookie = SetCookieHeaderValue.ParseList(values.ToList()).SingleOrDefault(c => c.Name.StartsWith(".AspNetCore.AntiForgery.", StringComparison.InvariantCultureIgnoreCase));
            }
            Assert.NotNull(_antiforgeryCookie);
            _client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_antiforgeryCookie.Name, _antiforgeryCookie.Value).ToString());

            var responseHtml = await response.Content.ReadAsStringAsync();
            var match = AntiforgeryFormFieldRegex.Match(responseHtml);
            _antiforgeryToken = match.Success ? match.Groups[1].Captures[0].Value : null;
            Assert.NotNull(_antiforgeryToken);

            return _antiforgeryToken;
        }

        protected SetCookieHeaderValue _registerCookie;

        protected async Task<Dictionary<string, string>> EnsureAntiforgeryTokenForm(Dictionary<string, string> formData = null)
        {
            if (formData == null) formData = new Dictionary<string, string>();

            formData.Add("__RequestVerificationToken", await EnsureAntiforgeryToken());
            return formData;
        }

        private string _email;
        private string _password;
        private string _confirmpassword;
        public async Task RegisterUser(string email, string password, string confirm)
        {
            _email = email;
            _password = password;
            _confirmpassword = confirm;
            EnsureRegisterCookie().Wait();
        }

        public async Task EnsureRegisterCookie()
        {
            if (_registerCookie != null) return;

            var formData = await EnsureAntiforgeryTokenForm(new Dictionary<string, string>
            {
                { "Email", _email },
                { "Password", _password },
                { "Confirm Password", _confirmpassword}
            });
            var response = await _client.PostAsync("/Identity/Account/Register", new FormUrlEncodedContent(formData));
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                _registerCookie = SetCookieHeaderValue.ParseList(values.ToList()).SingleOrDefault(c => c.Name.StartsWith(REGISTER_COOKIE, StringComparison.InvariantCultureIgnoreCase));
            }
            Assert.NotNull(_registerCookie);
            _client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_registerCookie.Name, _registerCookie.Value).ToString());

            // The current pair of antiforgery cookie-token is not valid anymore
            // Since the tokens are generated based on the authenticated user!
            // We need a new token after authentication (The cookie can stay the same)
            _antiforgeryToken = null;
        }
    }
}
