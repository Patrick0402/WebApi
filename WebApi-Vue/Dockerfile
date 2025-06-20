# Build stage
FROM node:22-alpine AS build-stage

# Argumento de build
ARG VUE_APP_API_BASE_URL

# Passa a variável pro ambiente de build (importante pro Vue acessar)
ENV VUE_APP_API_BASE_URL=$VUE_APP_API_BASE_URL

WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .

# Garante que o Vue CLI use as variáveis de ambiente no momento do build
RUN npm run build

# Production stage
FROM nginx:stable-alpine AS production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html

# Exemplo de nginx.conf custom (se quiser)
# COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
