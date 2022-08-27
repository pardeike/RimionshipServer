import { resolve } from 'path';
import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';

export default defineConfig(({ command, mode, ssrBuild }) => ({
  plugins: [solidPlugin()],
  server: {
    port: 3000,
    proxy: {
      '^/(api|Login|ExternalLogin|signin-twich)': {
        target: mode === 'public' ? 'https://frontend.rimionship.com' : 'http://localhost:5062',
        changeOrigin: true,
        autoRewrite: true,
        ws: true
      }
    },
    headers: {
      'x-mode': mode
    }
  },
  build: {
    target: 'esnext',
    outDir: command === 'build' ? '../RimionshipServer/wwwroot' : undefined,
    emptyOutDir: command === 'build' ? false : undefined,
    rollupOptions: {
      input: {
        dashboard: resolve(__dirname, 'dashboard.html')
      }
    }
  }
}));
