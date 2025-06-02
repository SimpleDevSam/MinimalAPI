using MinimalAPI_KeyCloack.Application.Requests;
using MinimalAPI_KeyCloack.Data.Models;
using System.Net.Http.Json;
using System.Net;
using Xunit;

namespace Tests.IntegrationTests;
public class UserTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    private readonly CreateUserRequest validUser = new()
    {
        UserName = "TestUser",
        UserEmail = "test@example.com"
    };

    private readonly CreateUserRequest updatedUser = new()
    {
        UserName = "UpdatedUser",
        UserEmail = "updated@example.com"
    };

    private readonly CreateUserRequest[] invalidUsers =
    {
        new() { UserName = "", UserEmail = "test@example.com" },
        new() { UserName = "User", UserEmail = "" },
        new() { UserName = "", UserEmail = "" },
        new() { UserName = "U", UserEmail = "invalidemail.com" }
    };

    public UserTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnOk()
    {
        var response = await _client.PostAsJsonAsync("/User", validUser);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateUser_Invalid_ShouldReturnBadRequest()
    {
        foreach (var invalid in invalidUsers)
        {
            var response = await _client.PostAsJsonAsync("/User", invalid);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnList()
    {
        await _client.PostAsJsonAsync("/User", validUser);
        var response = await _client.GetAsync("/Users");

        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<User>>();

        Assert.IsType<List<User>>(users);
        Assert.True(users.Count > 0, "User list should not be empty after creation.");
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser()
    {
        await _client.PostAsJsonAsync("/User", validUser);
        var users = await _client.GetFromJsonAsync<List<User>>("/Users");

        var id = users[0].Id;
        var get = await _client.GetAsync($"/User/{id}");

        get.EnsureSuccessStatusCode();
        var user = await get.Content.ReadFromJsonAsync<User>();

        Assert.Equal("TestUser", user.UserName);
    }

    [Fact]
    public async Task GetUserById_NotFound_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        var get = await _client.GetAsync($"/User/{id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldModifyUser()
    {
        await _client.PostAsJsonAsync("/User", validUser);
        var users = await _client.GetFromJsonAsync<List<User>>("/Users");
        var id = users[0].Id;

        var response = await _client.PutAsJsonAsync($"/User/{id}", updatedUser);
        response.EnsureSuccessStatusCode();

        var user = await _client.GetFromJsonAsync<User>($"/User/{id}");
        Assert.Equal("UpdatedUser", user.UserName);
    }

    [Fact]
    public async Task UpdateUser_Invalid_ShouldReturnBadRequest()
    {
        await _client.PostAsJsonAsync("/User", validUser);
        var users = await _client.GetFromJsonAsync<List<User>>("/Users");
        var id = users[0].Id;

        foreach (var invalid in invalidUsers)
        {
            var response = await _client.PutAsJsonAsync($"/User/{id}", invalid);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Fact]
    public async Task UpdateUser_NotFound_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        var response = await _client.PutAsJsonAsync($"/User/{id}", updatedUser);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ShouldRemoveUser()
    {
        await _client.PostAsJsonAsync("/User", validUser);
        var users = await _client.GetFromJsonAsync<List<User>>("/Users");
        var id = users[0].Id;

        var delete = await _client.DeleteAsync($"/User/{id}");
        delete.EnsureSuccessStatusCode();

        var get = await _client.GetAsync($"/User/{id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_NotFound_ShouldReturn404()
    {
        var id = Guid.NewGuid();
        var delete = await _client.DeleteAsync($"/User/{id}");
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
    }
}
