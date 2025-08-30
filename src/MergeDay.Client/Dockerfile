# ---------------------
# Stage 1: Build React app
# ---------------------
FROM node:20-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm ci || npm install

# Copy the rest of the source code
COPY . .
RUN npm run build

# ---------------------
# Stage 2: Serve with Nginx
# ---------------------
FROM nginx:alpine

# Clean default assets
RUN rm -rf /usr/share/nginx/html/*

# Copy build output
COPY --from=build /app/dist /usr/share/nginx/html

# Nginx config (SPA + proxy + no-cache for config.js)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Runtime env injection (executed automatically by nginx image on start)
COPY entrypoint.sh /docker-entrypoint.d/10-runtime-config.sh
RUN chmod +x /docker-entrypoint.d/10-runtime-config.sh

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
