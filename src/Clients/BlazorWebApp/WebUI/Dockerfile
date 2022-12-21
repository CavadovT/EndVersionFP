#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["src/Clients/BlazorWebApp/WebUI/WebUI.csproj", "src/Clients/BlazorWebApp/WebUI/"]
#RUN dotnet restore "src/Clients/BlazorWebApp/WebUI/WebUI.csproj"
#COPY . .
#WORKDIR "/src/src/Clients/BlazorWebApp/WebUI"
#RUN dotnet build "WebUI.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "WebUI.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "WebUI.dll"]

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet publish "src/Clients/BlazorWebApp/WebUI/WebUI.csproj" -c Release -o /app/build

FROM nginx:alpine
#WORKDIR /var/www/web
WORKDIR /usr/share/nginx/html
COPY --from=build-env /app/build/wwwroot .
COPY src/Clients/BlazorWebApp/WebUI/nginx.conf /etc/nginx/nginx.conf
COPY src/Clients/BlazorWebApp/WebUI/default.conf /etc/nginx/conf.d/default.conf
EXPOSE 2000