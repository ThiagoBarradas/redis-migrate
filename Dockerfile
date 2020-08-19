## APP BUILDER
FROM mcr.microsoft.com/dotnet/core/runtime:3.1

# Default Environment 
ENV CURRENT_VERSION="__[Version]__"

# Args
ARG distFolder=bin
ARG appFile=RedisMigrate.dll
 
# Copy files to /app
RUN ls
COPY ${distFolder} /app

# Run application
WORKDIR /app
RUN ls
ENV appFile=$appFile
ENTRYPOINT dotnet $appFile