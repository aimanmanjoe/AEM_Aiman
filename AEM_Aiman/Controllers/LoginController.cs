using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using AEM_Aiman.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AEM_Aiman.Controllers
{
    [Route("api/Account/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost, Route("")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            var loginUri = "http://test-demo.aemenersol.com/api/Account/Login";
            var username = request.Username;
            var password = request.Password;

            var loginData = new { Username = username, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(loginData));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsync(loginUri, content);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                return Ok(token);               
            }

            return StatusCode((int)response.StatusCode);
        }
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
