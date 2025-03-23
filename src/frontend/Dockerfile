# Etapa 1 - Build da aplicação Angular
FROM node:18 AS build
ARG BUILD_CONFIGURATION=production
WORKDIR /app

# Instala as dependências
COPY package*.json ./
RUN npm install

# Copia todo o código-fonte
COPY . .

# Gera o build para produção
RUN npm run build -- --configuration=$BUILD_CONFIGURATION

# Etapa 2 - Servir com NGINX
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# ✅ Copia o conteúdo do dist gerado corretamente
COPY --from=build /app/dist/frontend /usr/share/nginx/html

# Copia a configuração personalizada do nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
