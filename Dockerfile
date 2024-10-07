# Stage 1: Build Angular app
FROM node:16-alpine3.16 AS frontend-build
WORKDIR /luminate-chicago
COPY ./luminate-chicago/package*.json ./
RUN npm ci
COPY ./luminate-chicago .
RUN npm run build

# Stage 2: Build .NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /luminate-backend
COPY ./luminate-backend/luminate-backend/luminate-backend.csproj ./luminate-backend/
RUN dotnet restore "./luminate-backend/luminate-backend.csproj"
COPY ./luminate-backend .
RUN dotnet build "./luminate-backend/luminate-backend.csproj" -c Release -o /app/build
RUN dotnet publish "./luminate-backend/luminate-backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy Angular build output
COPY --from=luminate-chicago-build /luminate-chicago/dist/luminate-chicago /app/wwwroot

# Copy .NET backend output
COPY --from=luminate-backend-build /app/publish /app

# Expose ports for both frontend (80) and backend (8080)
EXPOSE 80 8080

# Start both Angular frontend using nginx and backend
COPY ./luminate-backend/nginx.conf /etc/nginx/nginx.conf

CMD ["sh", "-c", "nginx && dotnet luminate-backend.dll"]