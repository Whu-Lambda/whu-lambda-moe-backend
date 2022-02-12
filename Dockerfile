FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /Whu.Lambda.Moe.Backend
COPY Whu.Lambda.Moe.Backend/*.csproj ./
RUN dotnet restore
COPY Whu.Lambda.Moe.Backend/ ./
RUN dotnet publish -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/ ./
ENTRYPOINT ["dotnet", "Whu.Lambda.Moe.Backend.dll"]
EXPOSE 5000 5001