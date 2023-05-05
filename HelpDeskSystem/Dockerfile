FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5216

ENV ASPNETCORE_URLS=http://+:5216

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HelpDeskSystem/HelpDeskSystem.csproj", "HelpDeskSystem/"]
RUN dotnet restore "HelpDeskSystem/HelpDeskSystem.csproj"
COPY . .
WORKDIR "/src/HelpDeskSystem"
RUN dotnet build "HelpDeskSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HelpDeskSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HelpDeskSystem.dll"]
