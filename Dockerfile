# ----- Build Stage -----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /build

COPY ["src/AlatrafClinic.Api/AlatrafClinic.Api.csproj", "src/AlatrafClinic.Api/"]
COPY ["src/AlatrafClinic.Application/AlatrafClinic.Application.csproj", "src/AlatrafClinic.Application/"]
COPY ["src/AlatrafClinic.Domain/AlatrafClinic.Domain.csproj", "src/AlatrafClinic.Domain/"]
COPY ["src/AlatrafClinic.Infrastructure/AlatrafClinic.Infrastructure.csproj", "src/AlatrafClinic.Infrastructure/"]

RUN dotnet restore "src/AlatrafClinic.Api/AlatrafClinic.Api.csproj"

COPY . .

RUN dotnet publish "src/AlatrafClinic.Api/AlatrafClinic.Api.csproj" -c Release -o /app

# ----- Final Stage -----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Install timezone data for TimeZoneInfo support + set TZ to Yemen (Sana'a)
RUN apt-get update \
    && apt-get install -y --no-install-recommends tzdata \
    && ln -snf /usr/share/zoneinfo/Asia/Aden /etc/localtime \
    && echo "Asia/Aden" > /etc/timezone \
    && rm -rf /var/lib/apt/lists/*

ENV TZ=Asia/Aden

WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "AlatrafClinic.Api.dll"]
