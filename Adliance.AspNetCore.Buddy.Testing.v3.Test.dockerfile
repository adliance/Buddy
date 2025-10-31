FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app
COPY ./src/Adliance.AspNetCore.Buddy.Testing.v3 ./src/Adliance.AspNetCore.Buddy.Testing.v3
COPY ./src/Adliance.AspNetCore.Buddy.Testing.Shared ./src/Adliance.AspNetCore.Buddy.Testing.Shared
COPY ./test/Adliance.AspNetCore.Buddy.Testing.DemoProject ./test/Adliance.AspNetCore.Buddy.Testing.DemoProject
RUN dotnet publish ./test/Adliance.AspNetCore.Buddy.Testing.DemoProject/Adliance.AspNetCore.Buddy.Testing.DemoProject.csproj --configuration Release --output out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Adliance.AspNetCore.Buddy.Testing.DemoProject.dll"]