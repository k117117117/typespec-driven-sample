import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    host: "0.0.0.0",
    port: 3000,
    // Docker bind mount (特にWindows) ではinotifyが効かないためポーリングで監視
    watch: {
      usePolling: true,
      interval: 1000,
    },
    // HMR: コンテナ外のブラウザからWebSocketに接続するための設定
    hmr: {
      port: 3000,
    },
  },
  publicDir: "public",
});
