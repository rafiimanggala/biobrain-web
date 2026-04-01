# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all project files first for layer-cached restore
COPY Domain/Biobrain.Domain.csproj Domain/
COPY Biobrain.Infrastructure.Payments.PinPayment/Biobrain.Infrastructure.Payments.PinPayments.csproj Biobrain.Infrastructure.Payments.PinPayment/
COPY Application/Biobrain.Application.csproj Application/
COPY DataAccessLayer/DataAccessLayer.csproj DataAccessLayer/
COPY Biobrain.Infrastructure.Notifications/Biobrain.Infrastructure.Notifications.csproj Biobrain.Infrastructure.Notifications/
COPY Biobrain.Infrastructure.Payments/Biobrain.Infrastructure.Payments.csproj Biobrain.Infrastructure.Payments/
COPY BiobrainWebAPI/BiobrainWebAPI.csproj BiobrainWebAPI/

RUN dotnet restore BiobrainWebAPI/BiobrainWebAPI.csproj

# Copy everything else
COPY Domain/ Domain/
COPY Biobrain.Infrastructure.Payments.PinPayment/ Biobrain.Infrastructure.Payments.PinPayment/
COPY Application/ Application/
COPY DataAccessLayer/ DataAccessLayer/
COPY Biobrain.Infrastructure.Notifications/ Biobrain.Infrastructure.Notifications/
COPY Biobrain.Infrastructure.Payments/ Biobrain.Infrastructure.Payments/
COPY BiobrainWebAPI/ BiobrainWebAPI/

RUN dotnet publish BiobrainWebAPI/BiobrainWebAPI.csproj -c Release -o /app/publish --no-restore \
    && find /app/publish -name "libwkhtmltox*" -delete \
    && find /app/publish -name "libSkiaSharp*" -delete \
    && find /app/publish -name "*.so" -exec echo "[BUILD] Native lib: {}" \;

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Minimal native deps (PDF/chart native libs removed at build time)
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgdiplus \
    libfontconfig1 \
    libssl3 \
    libfreetype6 \
    libpng16-16 \
    zlib1g \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Ensure static and cache folders exist
RUN mkdir -p /app/static /app/cache

EXPOSE 10000

ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_gcServer=0
ENV DOTNET_GCHeapHardLimit=0x10000000
ENV DOTNET_EnableDiagnostics=0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "BiobrainWebAPI.dll"]
