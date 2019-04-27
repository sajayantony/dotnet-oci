FROM mcr.microsoft.com/dotnet/core/runtime:3.0.0-preview4-bionic
COPY __boostrap /__bootstrap 
ENTRYPOINT ["/__bootstrap"]
