FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
# EXPOSE 5002
# EXPOSE 5003
EXPOSE 80
EXPOSE 443

# ENV ASPNETCORE_URLS=http://+:5002
# ENV ASPNETCORE_URLS=http://+:5002;https://+:5003;http://+:80;https://+:443
# ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://+:443;http://+:80

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
# RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
# USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY ["Src/Identity.Contracts/Identity.Contracts.csproj", "Src/Identity.Contracts/"]
COPY ["Src/Identity.Service/Identity.Service.csproj", "Src/Identity.Service/"]

# RUN dotnet dev-certs https -ep /https/aspnetapp.pfx -p Sample@123$

RUN --mount=type=secret,id=GH_OWNER,dst=/GH_OWNER --mount=type=secret,id=GH_PAT,dst=/GH_PAT \
    dotnet nuget add source --username USERNAME --password `cat /GH_PAT` --store-password-in-clear-text --name github "https://nuget.pkg.github.com/`cat /GH_OWNER`/index.json"

RUN dotnet restore "Src/Identity.Service/Identity.Service.csproj"
COPY ./Src ./Src

# Generate and install SSL certificate
RUN mkdir /https \
    && openssl req -x509 -newkey rsa:4096 -nodes -out /https/aspnetapp.crt -keyout /https/aspnetapp.key -subj "/CN=localhost" \
    && openssl pkcs12 -export -out /https/aspnetapp.pfx -inkey /https/aspnetapp.key -in /https/aspnetapp.crt -passout pass: \
    && apt-get update \
    && apt-get install -y --no-install-recommends openssl \
    && rm -rf /var/lib/apt/lists/* \
    && dotnet dev-certs https -ep /https/aspnetapp.pfx -p "Sample@123$"

WORKDIR "/Src/Identity.Service"
RUN dotnet build "Identity.Service.csproj" -c Release --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Service.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /https
COPY --from=build /https .

WORKDIR /app
COPY --from=publish /app/publish .

# Set the HTTPS environment variable
# ENV ASPNETCORE_URLS=https://+:443;http://+:80;https://+:5003;http://+:5002

ENTRYPOINT ["dotnet", "Identity.Service.dll"]