# Use the .NET 9.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy everything and restore/build
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# Use the ASP.NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Start the app
ENTRYPOINT ["dotnet", "Bocchify_Server.dll"]
