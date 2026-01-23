# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IronWill.csproj", "."]
RUN dotnet restore "./IronWill.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "IronWill.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IronWill.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IronWill.dll"]
