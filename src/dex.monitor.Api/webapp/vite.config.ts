import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  publicDir: "src/static",
  optimizeDeps: {
    esbuildOptions: {
      target: "esnext",
    },
  },
  build: {
    target: "esnext", //browsers can handle the latest ES features
  },
  server: {
    host: true,
    port: 4031,
  },
  resolve: {
    alias: {
      "@models": "/src/models",
      "@components": "/src/components",
      "@styles": "/src/styles",
      "@pages": "/src/pages",
      "@services": "/src/services",
      "@stores": "/src/stores",
      "@helpers": "/src/helpers",
      "@layouts": "/src/layouts",
    },
  },
});
