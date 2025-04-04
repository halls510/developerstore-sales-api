﻿using Ambev.DeveloperEvaluation.Functional.Infrastructure;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Controllers;

/// <summary>
/// Testes para autenticação de usuários no AuthController.
/// </summary>
public class AuthControllerTests : FunctionalTestBase
{    

    [Fact]
    public async Task AuthenticateUser_ShouldReturn_Token_On_ValidCredentials()
    {
        var loginRequest = new
        {
            Email = "admin@example.com",
            Password = "A#g7jfdsd#$%#"
        };

        var response = await _client.PostAsJsonAsync("/api/auth", loginRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao autenticar: {await response.Content.ReadAsStringAsync()}");

        var authResult = await response.Content.ReadFromJsonAsync<ApiResponseWithData<AuthResponseDto>>();
        authResult.Should().NotBeNull();
        authResult!.Data.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task AuthenticateUser_ShouldReturn_Unauthorized_On_InvalidCredentials()
    {
        var loginRequest = new
        {
            Email = "wronguser@example.com",
            Password = "WrongPassword"
        };

        var response = await _client.PostAsJsonAsync("/api/auth", loginRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private class AuthResponseDto
    {
        public string Token { get; set; }
    }
}
