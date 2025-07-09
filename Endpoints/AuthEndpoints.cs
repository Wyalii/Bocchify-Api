using System.Security.Claims;
using Bocchify_Api.Contracts;
using Bocchify_Api.DTOS;
using Bocchify_Api.Interfaces;

namespace Bocchify_Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndPoint(this IEndpointRouteBuilder app)
        {

            app.MapPost("/register", async (RegisterUser registerUserRequest, IAuthService authService) =>
            {
                if (registerUserRequest == null)
                {
                    return Results.BadRequest("request is null for some reason.");
                }

                try
                {
                    BaseResponse<UserDTO> result = await authService.RegisterAsync(registerUserRequest);
                    if (result == null)
                    {
                        return Results.Problem("Registration service returned null response");
                    }
                    if (!result.Success)
                    {
                        return Results.BadRequest(new { message = result.Message ?? "Registration failed" });
                    }
                    return Results.Created($"/users/{result.Data.Id}", new { result.Message });
                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during registration: {ex.Message}");
                }
            });

            app.MapPost("/login", async (LoginUser loginUserRequest, IAuthService authService) =>
            {
                try
                {
                    BaseResponse<object> result = await authService.LoginAsync(loginUserRequest);
                    if (result == null)
                    {
                        return Results.Problem("Login service returned null response");
                    }


                    if (!result.Success)
                    {
                        return Results.BadRequest(new { message = result.Message ?? "Login failed" });
                    }
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during login: {ex.Message}");
                }
            });

            app.MapPost("/logout", async (HttpContext httpContext, IAuthService authService) =>
            {

                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                    return Results.Unauthorized();
                if (!int.TryParse(userIdClaim.Value, out var userId))
                    return Results.BadRequest(new { message = "Invalid user ID in token." });

                try
                {
                    var result = await authService.LogoutAsync(userId);

                    if (result == null)
                        return Results.Problem("Logout service returned null response");

                    if (!result.Success)
                    {
                        return Results.BadRequest(new { message = result.Message ?? "Logout failed" });
                    }
                    return Results.Ok(new { message = result.Message });
                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during logout: {ex.Message}");
                }


            });

            return app;
        }

    }
}