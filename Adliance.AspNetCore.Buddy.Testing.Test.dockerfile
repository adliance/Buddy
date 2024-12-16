FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID

WORKDIR /app
COPY ./src/Adliance.AspNetCore.Buddy.Testing ./src/Adliance.AspNetCore.Buddy.Testing
COPY ./test/Adliance.AspNetCore.Buddy.Testing.Test ./test/Adliance.AspNetCore.Buddy.Testing.Test
RUN dotnet publish ./test/Adliance.AspNetCore.Buddy.Testing.Test/Adliance.AspNetCore.Buddy.Testing.Test.csproj --configuration Release --output out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
WORKDIR /app
EXPOSE 80
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Adliance.AspNetCore.Buddy.Testing.Test.dll"]