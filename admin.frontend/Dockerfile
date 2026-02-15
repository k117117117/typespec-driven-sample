FROM node:22-alpine

WORKDIR /app

# package.json のみ先にコピーして依存関係をインストール
# (ソースコードはボリュームマウントで提供するため COPY . . は不要)
COPY package.json package-lock.json* ./
RUN npm install

EXPOSE 3000

CMD ["npm", "run", "dev"]
