using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPI_KeyCloack.Application.Requests;
using MinimalAPI_KeyCloack.Application.Validators;
using MinimalAPI_KeyCloack.Data.Models;
using MinimalAPI_KeyCloack.WebAPI.Abstractions;

namespace MinimalAPI_KeyCloack.WebAPI.EndPoints;

public class UserEndpoints : IEndPointDefinition
{
    public void RegisterEndpoints(WebApplication app)
    {
        app.MapPost("/User", async ([FromServices] AppDbContext dbContext, [FromServices] CreateUserValidator userValidator, CreateUserRequest userReq) =>
        {
            var validationResult = await userValidator.ValidateAsync(userReq);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var user = new User()
            {
                Id = Guid.NewGuid(),
                UserName = userReq.UserName,
                UserEmail = userReq.UserEmail
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Results.Ok("User created");
        })
        .WithName("CreateUser")
        .WithOpenApi();

        app.MapGet("/Users", async ([FromServices] AppDbContext dbContext) =>
        {
            var users = await dbContext.Users.ToListAsync();
            return Results.Ok(users);
        })
        .WithName("GetAllUsers")
        .WithOpenApi();

        app.MapGet("/User/{id}", async ([FromServices] AppDbContext dbContext, Guid id) =>
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return Results.NotFound($"User with ID {id} not found");
            }

            return Results.Ok(user);
        })
        .WithName("GetUserById")
        .WithOpenApi();

        app.MapPut("/User/{id}", async ([FromServices] AppDbContext dbContext, [FromServices] CreateUserValidator userValidator, Guid id, CreateUserRequest userReq) =>
        {
            var validationResult = await userValidator.ValidateAsync(userReq);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return Results.NotFound($"User with ID {id} not found");
            }

            user.UserName = userReq.UserName;
            user.UserEmail = userReq.UserEmail;

            await dbContext.SaveChangesAsync();

            return Results.Ok("User updated");
        })
        .WithName("UpdateUser")
        .WithOpenApi();

        app.MapDelete("/User/{id}", async ([FromServices] AppDbContext dbContext, Guid id) =>
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return Results.NotFound($"User with ID {id} not found");
            }

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();

            return Results.Ok("User deleted");
        })
        .WithName("DeleteUser")
        .WithOpenApi();
    }
}
