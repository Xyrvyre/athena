FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Athena/*.csproj ./Athena/
RUN dotnet restore

# copy everything else and build app
COPY Athena/. ./Athena/
WORKDIR /app/Athena
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/Athena/out ./


ENTRYPOINT ["dotnet", "Athena.dll"]
EXPOSE 80