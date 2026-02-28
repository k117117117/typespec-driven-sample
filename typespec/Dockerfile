FROM node:22-alpine

WORKDIR /app

COPY package.json package-lock.json* ./
RUN npm install

COPY . .

# admin と game-server の両方をコンパイル（出力先は tspconfig.yaml で指定）
ENTRYPOINT ["sh", "-c", "npx tsp compile admin/ && npx tsp compile game-server/"]
