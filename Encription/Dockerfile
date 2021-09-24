FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["Encription.csproj", "./"]
RUN dotnet restore "Encription.csproj"
COPY . .
# WORKDIR "/src/."
# RUN dotnet build "Encription.csproj" -c Release -o /app/build
# FROM build AS publish
RUN dotnet publish "Encription.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Encription.dll"]
