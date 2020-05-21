# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Athena/*.csproj ./Athena/
RUN dotnet restore

# copy everything else and build app
COPY Athena/. ./Athena/
WORKDIR /source/Athena
RUN dotnet publish -c release -o /app 

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./
COPY config /app/
ENV ASPNETCORE_ENVIRONMENT Development
ENTRYPOINT ["dotnet", "Athena.dll", "--environment=Development"]
