#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["helpdesk_api/HelpDeskSystem/HelpDeskSystem.csproj", "HelpDeskSystem/"]
#COPY ["helpdesk_api/Interfaces/Interfaces.csproj", "Interfaces/"]
#RUN dotnet restore "HelpDeskSystem/HelpDeskSystem.csproj"
#COPY . .
#WORKDIR "/src/HelpDeskSystem"
#RUN dotnet build "src/helpdesk_api/HelpDeskSystem.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "src/helpdesk_api/HelpDeskSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS AS build
WORKDIR /src
COPY ["helpdesk_api/HelpDeskSystem/HelpDeskSystem.csproj", "HelpDeskSystem/"]
RUN dotnet restore "HelpDeskSystem/HelpDeskSystem.csproj"
COPY . .
WORKDIR "/src/HelpDeskSystem"
RUN dotnet build "HelpDeskSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HelpDeskSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HelpDeskSystem.dll"]