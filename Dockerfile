FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG GITHUB_USER
ARG GITHUB_TOKEN
WORKDIR /app

RUN apt-get -qq update && apt-get -qqy --no-install-recommends install wget gnupg git unzip && \
    curl -sL https://deb.nodesource.com/setup_14.x | bash - && \
    apt-get install -y nodejs

COPY SpaceInvadersUI.csproj ./
RUN GITHUB_USER=${GITHUB_USER} GITHUB_TOKEN=${GITHUB_TOKEN} dotnet restore

COPY package.json package-lock.json ./
RUN npm install -y

COPY . .
RUN npm run publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/bin/Release/net5.0/publish/ ./
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "SpaceInvadersUI.dll"]