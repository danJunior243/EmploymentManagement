# ---------- Étape 1 : compiler ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# On copie d'abord seulement les fichiers de projet, puis on restaure les packages.
# Docker garde cette étape en cache tant que les .csproj ne changent pas,
# ce qui accélère les déploiements suivants.
COPY *.sln ./
COPY EmploymentManagementSystem/*.csproj EmploymentManagementSystem/
COPY EmploymentManagement.BLL/*.csproj EmploymentManagement.BLL/
COPY EmploymentManagement.Data/*.csproj EmploymentManagement.Data/
RUN dotnet restore EmploymentManagementSystem/EmploymentManagementSystem.csproj

# Puis on copie tout le code et on publie
COPY . .
RUN dotnet publish EmploymentManagementSystem/EmploymentManagementSystem.csproj \
    -c Release -o /app/publish --no-restore

# ---------- Étape 2 : exécuter ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Railway fournit le port dans la variable PORT. On écoute dessus, avec 8080 par défaut.
CMD ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} dotnet EmploymentManagementSystem.dll"]