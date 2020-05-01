FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app

COPY ./BetterRead.Shared.sln ./
COPY ./src/BetterRead.Shared/BetterRead.Shared.csproj ./src/BetterRead.Shared/
COPY ./tests/BetterRead.Shared.Tests/BetterRead.Shared.Tests.csproj ./tests/BetterRead.Shared.Tests/

RUN dotnet restore
COPY . .

ARG CI_BUILDID
ARG CI_PRERELEASE

ENV CI_BUILDID ${CI_BUILDID}
ENV CI_PRERELEASE ${CI_PRERELEASE}

RUN dotnet build -c Release
RUN dotnet test -c Release --no-build --no-restore
RUN dotnet pack -c Release --no-restore --no-build -o /app/out