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
                BaseResponse<UserDTO> result = await authService.RegisterAsync(registerUserRequest);
                if (result == null)
                {
                    return Results.BadRequest(new { message = result?.Message ?? "Registration failed" });
                }
                return Results.Created($"/users/{result.Data.Id}", new { result.Message });
            });

            return app;
        }

    }
}