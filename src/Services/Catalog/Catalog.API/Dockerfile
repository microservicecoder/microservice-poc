#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#docker file consist of 2 parts : Build and Publish[Run]

#FROM : specifies from where the files are taken 
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
#WORKDIR: the files are created in docker container at /app directory
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
#COPY: specify to copy the files from goven location to docker container location
COPY ["Services/Catalog/Catalog.API/Catalog.API.csproj", "Services/Catalog/Catalog.API/"]
RUN dotnet restore "Services/Catalog/Catalog.API/Catalog.API.csproj"
COPY . .
WORKDIR "/src/Services/Catalog/Catalog.API"
RUN dotnet build "Catalog.API.csproj" -c Release -o /app/build

#Once build successful, it will pucbish the project.
FROM build AS publish
RUN dotnet publish "Catalog.API.csproj" -c Release -o /app/publish

# the published code will run using docker file using dotnet command. 
# dotnet command, will execute the Catalog.API.dll, which is a project entrypoint.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
