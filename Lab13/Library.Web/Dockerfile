FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS builder
COPY ./ /build
WORKDIR /build
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
COPY --from=builder /build/out /app
WORKDIR /app
EXPOSE 91

USER $APP_UID

CMD ["dotnet", "Library.Web.dll"]
