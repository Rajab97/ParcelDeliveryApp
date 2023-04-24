#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OrderManagementService.OutBox/OrderManagementService.OutBox.csproj", "OrderManagementService.OutBox/"]
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]
RUN dotnet restore "OrderManagementService.OutBox/OrderManagementService.OutBox.csproj"
COPY . .
WORKDIR "/src/OrderManagementService.OutBox"
RUN dotnet build "OrderManagementService.OutBox.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderManagementService.OutBox.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderManagementService.OutBox.dll"]