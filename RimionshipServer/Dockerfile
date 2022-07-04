#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 1443
ENV ASPNETCORE_URLS=https://*:1443
ENV USER_SECRETS_ID=3ca70a3b-0c13-4dd1-a7f2-5f32d3e3875a

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RimionshipServer/RimionshipServer.csproj", "RimionshipServer/"]
RUN dotnet restore "RimionshipServer/RimionshipServer.csproj"
COPY . .
WORKDIR "/src/RimionshipServer"
RUN dotnet build "RimionshipServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RimionshipServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RimionshipServer.dll"]
