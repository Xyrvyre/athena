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
RUN mkdir testfolder
WORKDIR testfolder
RUN echo "HelloWorld" > file.name
WORKDIR /app
COPY Athena/OAuth.json ./OAuth.json
COPY Athena/sa.json ./sa.json
ENTRYPOINT ["dotnet", "Athena.dll"]
EXPOSE 80