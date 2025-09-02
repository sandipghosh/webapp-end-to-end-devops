# ============================
# Stage 1: Install & Test
# ============================
FROM node:current-alpine3.22 AS test

# Set working directory
WORKDIR /app

# Copy package.json and package-lock.json first (for caching)
COPY package*.json ./

# Install dependencies
RUN npm clean-install

# Copy the rest of the application source
COPY . .

# Install Chrome & dependencies
RUN apk add --no-cache \
    chromium \
    nss \
    freetype \
    harfbuzz \
    ttf-freefont \
    fontconfig \
    dbus \
    libxcomposite \
    libxdamage \
    libxrandr \
    mesa-gbm \
    alsa-lib \
    pango \
    gtk+3.0 \
    && rm -rf /var/cache/apk/*

# Set Chrome binary path
ENV CHROME_BIN=/usr/bin/chromium

# Create non-root user and set permissions
RUN adduser -D tester
RUN mkdir -p /app/.angular/cache && chown -R tester:tester /app/.angular

USER tester

# Run tests with ChromeHeadless and disable sandbox
# RUN npm run test -- --watch=false --browsers=ChromeHeadless --no-sandbox
RUN npm run test -- --watch=false --browsers=ChromeHeadlessNoSandbox


# ============================
# Stage 2: Build Angular App
# ============================
FROM node:current-alpine3.22 AS build

WORKDIR /app

# Copy only what's needed from test stage (dependencies + source)
COPY --from=test /app /app

# Build Angular app for production
RUN npm run build -- --configuration production --output-hashing=none

# ============================
# Stage 3: Serve with NGINX
# ============================
FROM nginx:alpine

# Copy built Angular files from Stage 2
COPY --from=build /app/dist/ /usr/share/nginx/html

# Copy custom NGINX config (optional for SPA routing)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy env.js to override the existing environment
COPY env.js /usr/share/nginx/html/assets/env.js

# Expose port 80
EXPOSE 80

# Start NGINX
CMD ["nginx", "-g", "daemon off;"]
