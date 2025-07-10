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

            app.MapPost("/Register", async (RegisterUser registerUserRequest, IAuthService authService) =>
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
                        return Results.BadRequest(new { message = result.Message ?? "Registration failed", success = result.Success });
                    }
                    return Results.Created($"/users/{result.Data.Id}", new { result.Message, success = result.Success });
                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during registration: {ex.Message}");
                }
            });

            app.MapPost("/Login", async (LoginUser loginUserRequest, IAuthService authService) =>
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
                        return Results.BadRequest(new { message = result.Message ?? "Login failed", success = result.Success });
                    }
                    return Results.Ok(new { message = result.Message, success = result.Success, data = result.Data });
                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during login: {ex.Message}");
                }
            });

            app.MapPost("/Logout", async (HttpContext httpContext, IAuthService authService) =>
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
                        return Results.BadRequest(new { message = result.Message ?? "Logout failed", success = result.Success });
                    }
                    return Results.Ok(new { message = result.Message, success = result.Success });
                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during logout: {ex.Message}");
                }


            }).RequireAuthorization();

            app.MapPost("/Verify", async (HttpRequest request, IAuthService authService) =>
            {
                try
                {
                    if (!request.Headers.TryGetValue("verifyToken", out var tokenHeader) || string.IsNullOrWhiteSpace(tokenHeader))
                    {
                        return Results.BadRequest(new
                        {
                            success = false,
                            message = "Verification token is missing in the header."
                        });
                    }

                    VerifyUser verifyUser = new VerifyUser
                    {
                        VerifyToken = tokenHeader
                    };
                    var result = await authService.VerifyUserAsync(verifyUser);
                    if (!result.Success || result == null)
                    {
                        return Results.BadRequest(new { message = result.Message ?? "verify failed", success = result.Success });
                    }

                    return Results.Ok(new { message = result.Message, success = result.Success });
                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during logout: {ex.Message}");
                }
            });

            app.MapPost("/ResendVerification", async (GenericEmail ResendVerifyTokenRequest, IAuthService authService) =>
            {
                try
                {
                    var result = await authService.ResendVerifyToken(ResendVerifyTokenRequest);
                    if (!result.Success || result == null)
                    {
                        return Results.BadRequest(new { message = result.Message ?? "resend verification failed", success = result.Success });
                    }

                    return Results.Ok(new { message = result.Message, success = result.Success });
                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during resend verification: {ex.Message}");
                }
            });

            app.MapPost("/SendForgotPassword", async (GenericEmail ForgotPasswordRequest, IAuthService authService) =>
            {
                try
                {
                    var result = await authService.ForgotPassword(ForgotPasswordRequest);
                    if (!result.Success || result == null)
                    {
                        return Results.BadRequest(new { message = result.Message ?? "send forgot password verification failed", success = result.Success });
                    }
                    return Results.Ok(new { message = result.Message, success = result.Success });

                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during send forgot password verification: {ex.Message}");
                }
            });

            app.MapPost("/ChangePassword", async (ChangePassword ChangePasswordRequest, IAuthService authService) =>
            {
                try
                {
                    var result = await authService.ChangePassword(ChangePasswordRequest);
                    if (!result.Success || result == null)
                    {
                        return Results.BadRequest(new { message = result.Message ?? "change password failed.", success = result.Success });
                    }
                    return Results.Ok(new { message = result.Message, success = result.Success });

                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during change password: {ex.Message}");
                }
            });
            return app;
        }



    }
}