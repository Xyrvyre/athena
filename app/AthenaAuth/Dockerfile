# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY AthenaAuth/*.csproj ./AthenaAuth/
RUN dotnet restore

# copy everything else and build app
COPY AthenaAuth/. ./AthenaAuth/
WORKDIR /source/AthenaAuth
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./
#COPY templates ./templates
ENTRYPOINT ["dotnet", "AthenaAuth.dll"]
