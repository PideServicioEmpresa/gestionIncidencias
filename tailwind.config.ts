import type { Config } from 'tailwindcss'
import animate from 'tailwindcss-animate'

const config: Config = {
  darkMode: ['class'],
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    screens: {
      sm: '640px',
      md: '768px',
      lg: '1024px',
      xl: '1280px',
      '2xl': '1536px',
    },
    extend: {
      colors: {
        border: 'hsl(var(--border))',
        input: 'hsl(var(--input))',
        ring: 'hsl(var(--ring))',
        background: 'hsl(var(--background))',
        foreground: 'hsl(var(--foreground))',
        primary: {
          DEFAULT: 'hsl(var(--primary))',
          foreground: 'hsl(var(--primary-foreground))',
        },
        secondary: {
          DEFAULT: 'hsl(var(--secondary))',
          foreground: 'hsl(var(--secondary-foreground))',
        },
        destructive: {
          DEFAULT: 'hsl(var(--destructive))',
          foreground: 'hsl(var(--destructive-foreground))',
        },
        muted: {
          DEFAULT: 'hsl(var(--muted))',
          foreground: 'hsl(var(--muted-foreground))',
        },
        accent: {
          DEFAULT: 'hsl(var(--accent))',
          foreground: 'hsl(var(--accent-foreground))',
        },
        popover: {
          DEFAULT: 'hsl(var(--popover))',
          foreground: 'hsl(var(--popover-foreground))',
        },
        card: {
          DEFAULT: 'hsl(var(--card))',
          foreground: 'hsl(var(--card-foreground))',
        },
        // Colores semanticos del sistema
        success: {
          DEFAULT: 'hsl(var(--success))',
          foreground: 'hsl(var(--success-foreground))',
        },
        warning: {
          DEFAULT: 'hsl(var(--warning))',
          foreground: 'hsl(var(--warning-foreground))',
        },
        info: {
          DEFAULT: 'hsl(var(--info))',
          foreground: 'hsl(var(--info-foreground))',
        },
        // Estados de tickets (configurables desde BD)
        ticket: {
          'sin-asignar': {
            DEFAULT: 'hsl(var(--ticket-sin-asignar))',
            foreground: 'hsl(var(--ticket-sin-asignar-foreground))',
          },
          asignado: {
            DEFAULT: 'hsl(var(--ticket-asignado))',
            foreground: 'hsl(var(--ticket-asignado-foreground))',
          },
          'en-proceso': {
            DEFAULT: 'hsl(var(--ticket-en-proceso))',
            foreground: 'hsl(var(--ticket-en-proceso-foreground))',
          },
          pendiente: {
            DEFAULT: 'hsl(var(--ticket-pendiente))',
            foreground: 'hsl(var(--ticket-pendiente-foreground))',
          },
          cerrado: {
            DEFAULT: 'hsl(var(--ticket-cerrado))',
            foreground: 'hsl(var(--ticket-cerrado-foreground))',
          },
          reabierto: {
            DEFAULT: 'hsl(var(--ticket-reabierto))',
            foreground: 'hsl(var(--ticket-reabierto-foreground))',
          },
        },
        // Prioridades de tickets
        priority: {
          baja: {
            DEFAULT: 'hsl(var(--priority-baja))',
            foreground: 'hsl(var(--priority-baja-foreground))',
          },
          media: {
            DEFAULT: 'hsl(var(--priority-media))',
            foreground: 'hsl(var(--priority-media-foreground))',
          },
          alta: {
            DEFAULT: 'hsl(var(--priority-alta))',
            foreground: 'hsl(var(--priority-alta-foreground))',
          },
          critica: {
            DEFAULT: 'hsl(var(--priority-critica))',
            foreground: 'hsl(var(--priority-critica-foreground))',
          },
        },
      },
      borderRadius: {
        lg: 'var(--radius)',
        md: 'calc(var(--radius) - 2px)',
        sm: 'calc(var(--radius) - 4px)',
      },
      fontFamily: {
        sans: ['Geist', '-apple-system', 'BlinkMacSystemFont', '"Segoe UI"', 'sans-serif'],
      },
      fontSize: {
        xs: ['0.75rem', { lineHeight: '1rem' }],
        sm: ['0.875rem', { lineHeight: '1.25rem' }],
        base: ['1rem', { lineHeight: '1.5rem' }],
        lg: ['1.125rem', { lineHeight: '1.75rem' }],
        xl: ['1.25rem', { lineHeight: '1.75rem' }],
        '2xl': ['1.5rem', { lineHeight: '2rem' }],
        '3xl': ['1.875rem', { lineHeight: '2.25rem' }],
        '4xl': ['2.25rem', { lineHeight: '2.5rem' }],
        '5xl': ['3rem', { lineHeight: '1' }],
        display: ['3.75rem', { lineHeight: '1', letterSpacing: '-0.02em' }],
      },
      spacing: {
        'safe-bottom': 'env(safe-area-inset-bottom)',
        'safe-top': 'env(safe-area-inset-top)',
      },
      minHeight: {
        screen: ['100vh', '100dvh'],
      },
      width: {
        sidebar: '260px',
        'sidebar-collapsed': '64px',
      },
      maxWidth: {
        sidebar: '260px',
      },
      height: {
        header: '56px',
      },
      keyframes: {
        'accordion-down': {
          from: {
            height: '0',
          },
          to: {
            height: 'var(--radix-accordion-content-height)',
          },
        },
        'accordion-up': {
          from: {
            height: 'var(--radix-accordion-content-height)',
          },
          to: {
            height: '0',
          },
        },
      },
      animation: {
        'accordion-down': 'accordion-down 0.2s ease-out',
        'accordion-up': 'accordion-up 0.2s ease-out',
      },
    },
  },
  plugins: [animate],
}

export default config
