# ---------------------
# Stage 1: Build React app
# ---------------------
FROM node:20-alpine AS build

WORKDIR /app

# Copy package.json and lock file if present
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the source code
COPY . .

# Build for production
RUN npm run build

# ---------------------
# Stage 2: Serve with Nginx
# ---------------------
FROM nginx:alpine

# Remove default nginx static files
RUN rm -rf /usr/share/nginx/html/*

# Copy build output
COPY --from=build /app/dist /usr/share/nginx/html

# Copy a default nginx config (SPA support)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
