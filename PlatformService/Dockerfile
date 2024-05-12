#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#pull the SDK image to do build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
#Create a working Directory
WORKDIR /app

#Copy Purject file which contains dependency information
COPY *.csproj ./
# Copy install all the dependencies of the project pull down packages
RUN dotnet restore

# Copy all files after restore
COPY . ./

RUN dotnet publish -c Release -o out

#Build the runtime version without excess sdk images
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80

#when we run an image this will be kicked off
ENTRYPOINT ["dotnet", "PlatformService.dll"]