FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app
COPY ./src/Adliance.AspNetCore.Buddy.Testing ./src/Adliance.AspNetCore.Buddy.Testing
COPY ./test/Adliance.AspNetCore.Buddy.Testing.Test ./test/Adliance.AspNetCore.Buddy.Testing.Test
RUN dotnet publish ./test/Adliance.AspNetCore.Buddy.Testing.Test/Adliance.AspNetCore.Buddy.Testing.Test.csproj --configuration Release --output out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Adliance.AspNetCore.Buddy.Testing.Test.dll"]