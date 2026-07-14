import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const apiTarget = env.VITE_API_URL || 'http://localhost:5000'

  return {
    plugins: [react()],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, './src'),
        '@app': path.resolve(__dirname, './src/app'),
        '@features': path.resolve(__dirname, './src/features'),
        '@shared': path.resolve(__dirname, './src/shared'),
        '@services': path.resolve(__dirname, './src/services'),
        '@hooks': path.resolve(__dirname, './src/hooks'),
        '@store': path.resolve(__dirname, './src/store'),
        '@types-app': path.resolve(__dirname, './src/types'),
        '@utils': path.resolve(__dirname, './src/utils'),
        '@constants': path.resolve(__dirname, './src/constants'),
        '@lib': path.resolve(__dirname, './src/lib'),
        '@styles': path.resolve(__dirname, './src/styles'),
        '@assets': path.resolve(__dirname, './src/assets'),
        '@mocks': path.resolve(__dirname, './src/mocks'),
      },
    },
    server: {
      port: parseInt(process.env.PORT ?? '3000'),
      open: false,
      proxy: {
        '/api': {
          target: apiTarget,
          changeOrigin: true,
          secure: false,
        },
      },
    },
    build: {
      target: 'esnext',
      sourcemap: false,
      minify: 'esbuild',
      // Aumentamos el umbral de advertencia a 1 MB; los chunks grandes se separan manualmente.
      chunkSizeWarningLimit: 1000,
      rollupOptions: {
        output: {
          manualChunks: {
            // React base + router (siempre necesario)
            vendor: ['react', 'react-dom', 'react-router-dom'],
            // TanStack Query (runtime de datos)
            query: ['@tanstack/react-query'],
            // Formularios y validación
            forms: ['react-hook-form', 'zod', '@hookform/resolvers'],
            // Recharts (~500 KB sin comprimir — separado para no bloquear carga inicial)
            charts: ['recharts'],
            // Supabase client
            supabase: ['@supabase/supabase-js'],
            // Framer Motion (~130 KB gzipped — solo se usa en animaciones secundarias)
            motion: ['framer-motion'],
            // Drag and drop (solo se usa en dashboard admin)
            dnd: ['@dnd-kit/core', '@dnd-kit/sortable', '@dnd-kit/utilities'],
            // date-fns (solo las funciones de formato de fecha)
            dates: ['date-fns'],
          },
        },
      },
    },
  }
})
