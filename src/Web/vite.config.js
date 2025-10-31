import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    port: 5173,
    // proxy API calls to .NET in dev
    proxy: { '/api': { target: 'http://localhost:5180', changeOrigin: true } }
  },
  build: { outDir: 'dist' }, // matches AddSpaStaticFiles RootPath
  base: '/',                 // important for correct asset paths
})
