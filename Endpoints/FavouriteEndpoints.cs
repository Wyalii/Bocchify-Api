using System.Security.Claims;
using Bocchify_Api.Contracts;
using Bocchify_Api.Interfaces;

namespace Bocchify_Api.Endpoints
{
    public static class FavouriteEndpoints
    {
        public static IEndpointRouteBuilder MapFavouriteEndPoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/FavouriteHandler", async (FavouriteRequest favouriteRequest, IFavouriteService favouriteService, HttpContext httpContext) =>
            {
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                    return Results.Unauthorized();
                if (!int.TryParse(userIdClaim.Value, out var userId))
                    return Results.BadRequest(new { message = "Invalid user ID in token." });
                try
                {
                    var result = await favouriteService.FavouriteHandler(favouriteRequest, userId);
                    if (result == null)
                        return Results.Problem("favourite service returned null response");

                    if (!result.Success)
                    {
                        return Results.BadRequest(new { message = result.Message ?? "favourite failed", success = result.Success });
                    }
                    return Results.Ok(new { message = result.Message, success = result.Success });
                }
                catch (Exception ex)
                {

                    return Results.Problem($"An error occurred during favourite: {ex.Message}");
                }
            });


            return app;
        }
    }
}