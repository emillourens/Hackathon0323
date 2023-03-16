FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_lts.x | bash - && \
    apt-get install -y nodejs && \
    apt-get install npm

RUN npm install -g azurite