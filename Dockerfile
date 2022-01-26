FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY GrpcServer/*.csproj .
RUN dotnet restore
COPY GrpcServer/* .
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GrpcServer.dll"]
EXPOSE 80 443