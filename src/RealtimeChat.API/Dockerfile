FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/RealtimeChat.API/RealtimeChat.API.csproj", "src/RealtimeChat.API/"]
COPY ["src/RealtimeChat.Application/RealtimeChat.Application.csproj", "src/RealtimeChat.Application/"]
COPY ["src/RealtimeChat.Infrastructure/RealtimeChat.Infrastructure.csproj", "src/RealtimeChat.Infrastructure/"]
COPY ["src/RealtimeChat.Domain/RealtimeChat.Domain.csproj", "src/RealtimeChat.Domain/"]
COPY ["src/RealtimeChat.Shared/RealtimeChat.Shared.csproj", "src/RealtimeChat.Shared/"]

RUN dotnet restore "src/RealtimeChat.API/RealtimeChat.API.csproj"
COPY . .
WORKDIR "/src/src/RealtimeChat.API"
RUN dotnet build "RealtimeChat.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealtimeChat.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealtimeChat.API.dll"]