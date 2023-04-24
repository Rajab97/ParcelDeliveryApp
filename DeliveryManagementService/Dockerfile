#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

RUN ln -snf /usr/share/zoneinfo/Asia/Baku /etc/localtime && echo "Asia/Baku" > /etc/timezone

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DeliveryManagementService/DeliveryManagementService.csproj", "DeliveryManagementService/"]
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]
RUN dotnet restore "DeliveryManagementService/DeliveryManagementService.csproj"
COPY . .
WORKDIR "/src/DeliveryManagementService"
RUN dotnet build "DeliveryManagementService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeliveryManagementService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeliveryManagementService.dll"]