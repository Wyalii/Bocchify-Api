using Bocchify_Api.Endpoints;
using Bocchify_Api.Extensions;
var builder = WebApplication.CreateBuilder(args);
builder.ConfigureApplicationBuilder();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bocchify API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.MapGroup("/api/v1").WithTags("Auth Endpoints").MapAuthEndPoint();
app.Run();

