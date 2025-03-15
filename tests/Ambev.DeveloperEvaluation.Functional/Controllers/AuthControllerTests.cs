using Ambev.DeveloperEvaluation.Functional.Base;
using Ambev.DeveloperEvaluation.Functional.Utils;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Controllers;

public class AuthControllerTests : TestBase
{
    public AuthControllerTests(TestFixture fixture) : base(fixture, requiresAuth: false) { }

    [Fact]
    public async Task AuthenticateUser_ShouldReturn_Token_On_ValidCredentials()
    {
        var loginRequest = TestDataGenerator.GenerateAuthRequest(valid: true);

        var response = await _client.PostAsJsonAsync("/api/auth", loginRequest);
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro: {response.StatusCode}, Resposta: {errorDetails}");
        }
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(authResult);
        Assert.False(string.IsNullOrWhiteSpace(authResult?.Token));
    }

    [Fact]
    public async Task AuthenticateUser_ShouldReturn_Unauthorized_On_InvalidCredentials()
    {
        var loginRequest = TestDataGenerator.GenerateAuthRequest(valid: false);

        var response = await _client.PostAsJsonAsync("/api/auth", loginRequest);
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro: {response.StatusCode}, Resposta: {errorDetails}");
        }
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateUser_ShouldReturn_BadRequest_On_EmptyCredentials()
    {
        var loginRequest = new
        {
            Email = "",
            Password = ""
        };

        var response = await _client.PostAsJsonAsync("/api/auth", loginRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}






