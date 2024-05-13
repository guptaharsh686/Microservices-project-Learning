FROM mcr.microsoft.com/dotnet/sdk:8.0 As build-env
WORKDIR /app

#copy csproj fil from current directory to /app(workdir)
COPY *.csproj .
#will automatically add project dependencies like EntityFrameworkCore etc.
RUN dotnet restore

#copy all files from current dir(where docker file is present) to /app(workdir)
COPY . .
# publish the project into app/out folder
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

#copy all the built files from build-env image (/app/out) to current runtime image /app folder
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet","CommandsService.dll"]


