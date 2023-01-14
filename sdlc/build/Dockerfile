FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Leads.Shell/Leads.Shell.fsproj", "Leads.Shell/"]
RUN dotnet restore "Leads.Shell/Leads.Shell.fsproj"
COPY . .
WORKDIR "/src/Leads.Shell"
RUN dotnet build "Leads.Shell.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Leads.Shell.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Leads.Shell.dll"]
