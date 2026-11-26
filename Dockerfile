# Accessibility Way - imagem para o Render (ou qualquer host com Docker)

# 1) Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Visual Studio/MeuSiteEmMVC/MeuSiteEmMVC/MeuSiteEmMVC.csproj", "app/"]
RUN dotnet restore "app/MeuSiteEmMVC.csproj"
COPY ["Visual Studio/MeuSiteEmMVC/MeuSiteEmMVC/", "app/"]
RUN dotnet publish "app/MeuSiteEmMVC.csproj" -c Release -o /out --no-restore

# 2) Execução
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true \
    PORT=8080
COPY --from=build /out .
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "MeuSiteEmMVC.dll"]
