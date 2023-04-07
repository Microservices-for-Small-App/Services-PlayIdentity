#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5002

ENV ASPNETCORE_URLS=http://+:5002

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Src/Identity.Service/Identity.Service.csproj", "Src/Identity.Service/"]
COPY ["Src/Identity.Contracts/Identity.Contracts.csproj", "Src/Identity.Contracts/"]
RUN dotnet restore "Src/Identity.Service/Identity.Service.csproj"
COPY . .
WORKDIR "/src/Src/Identity.Service"
RUN dotnet build "Identity.Service.csproj" -c Release --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Service.csproj" -c Release --no-build -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Identity.Service.dll"]