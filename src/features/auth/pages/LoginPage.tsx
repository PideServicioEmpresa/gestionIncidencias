import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { AlertCircle, Eye, EyeOff, Info, Loader2, Wrench } from 'lucide-react'
import { ApiClientError } from '@services/apiClient'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Label } from '@shared/ui/label'
import { Card, CardContent } from '@shared/ui/card'
import { FormField } from '@shared/components/FormField'
import { Separator } from '@shared/ui/separator'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@shared/ui/dialog'
import { ROUTES } from '@constants/index'
import { authService } from '../services/authService'

const loginSchema = z.object({
  correo: z.string().email('Ingresa un correo electrónico válido.'),
  contrasena: z.string().min(1, 'Ingresa tu contraseña.'),
})
type LoginForm = z.infer<typeof loginSchema>

export function LoginPage() {
  const navigate = useNavigate()
  const [showPassword, setShowPassword] = useState(false)
  const [loading, setLoading] = useState(false)
  const [authError, setAuthError] = useState('')
  const [esMantenimiento, setEsMantenimiento] = useState(false)
  const [forgotOpen, setForgotOpen] = useState(false)

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginForm>({ resolver: zodResolver(loginSchema) })

  const doLogin = async (correo: string, contrasena: string) => {
    setLoading(true)
    setAuthError('')
    setEsMantenimiento(false)
    try {
      await authService.login(correo, contrasena)
      navigate(ROUTES.DASHBOARD, { replace: true })
    } catch (err) {
      if (err instanceof ApiClientError && err.codigo === 'SISTEMA_EN_MANTENIMIENTO') {
        setEsMantenimiento(true)
      } else {
        setAuthError(err instanceof Error ? err.message : 'Error al iniciar sesión.')
      }
    } finally {
      setLoading(false)
    }
  }

  const onSubmit = (data: LoginForm) => {
    doLogin(data.correo, data.contrasena)
  }

  return (
    <div className="space-y-5">
      {/* Login card */}
      <Card className="ps-glow-form rounded-2xl shadow-lg">
        <CardContent className="p-6">
          <div className="mb-5">
            <h2 className="text-xl font-bold tracking-tight">Iniciar sesión</h2>
            <p className="mt-1 text-sm text-muted-foreground">
              Ingresa tus credenciales para continuar
            </p>
          </div>

          <Separator className="mb-5" />

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            {/* Correo */}
            <FormField label="Correo electrónico" required error={errors.correo?.message}>
              <Input
                id="correo"
                type="email"
                placeholder="tu@empresa.com"
                autoComplete="email"
                disabled={loading}
                className="h-10 rounded-xl text-sm"
                {...register('correo')}
              />
            </FormField>

            {/* Contraseña */}
            <div className="space-y-1.5">
              <div className="flex items-center justify-between">
                <Label htmlFor="contrasena" className="text-xs font-medium">
                  Contraseña <span className="text-destructive">*</span>
                </Label>
                <Button
                  type="button"
                  variant="link"
                  size="sm"
                  className="h-auto p-0 text-xs font-normal text-primary"
                  tabIndex={0}
                  onClick={() => setForgotOpen(true)}
                >
                  ¿Olvidaste tu contraseña?
                </Button>
              </div>
              <div className="relative">
                <Input
                  id="contrasena"
                  type={showPassword ? 'text' : 'password'}
                  placeholder="••••••••"
                  autoComplete="current-password"
                  disabled={loading}
                  className="h-10 rounded-xl pr-10 text-sm"
                  {...register('contrasena')}
                />
                <button
                  type="button"
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
                  onClick={() => setShowPassword((v) => !v)}
                  aria-label={showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
                >
                  {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                </button>
              </div>
              {errors.contrasena && (
                <p className="flex items-center gap-1 text-[11px] text-destructive">
                  <AlertCircle className="h-3 w-3 shrink-0" />
                  {errors.contrasena.message}
                </p>
              )}
            </div>

            {/* Mantenimiento programado */}
            {esMantenimiento && (
              <div className="flex items-start gap-2 rounded-lg bg-amber-50 px-3 py-3 text-xs text-amber-800 dark:bg-amber-950/30 dark:text-amber-400">
                <Wrench className="mt-0.5 h-3.5 w-3.5 shrink-0" aria-hidden />
                <div>
                  <p className="font-semibold">Sistema en mantenimiento</p>
                  <p className="mt-0.5 leading-relaxed">
                    El sistema está temporalmente en mantenimiento. Por favor, vuelve a intentarlo
                    en unos minutos. Disculpa las molestias.
                  </p>
                </div>
              </div>
            )}

            {/* Error de autenticación */}
            {authError && (
              <p className="flex items-center gap-1.5 rounded-lg bg-destructive/10 px-3 py-2 text-xs text-destructive">
                <AlertCircle className="h-3.5 w-3.5 shrink-0" />
                {authError}
              </p>
            )}

            <div aria-live="polite" aria-atomic="true" className="sr-only">
              {loading ? 'Iniciando sesión, por favor espera.' : ''}
            </div>

            <Button
              type="submit"
              className="h-10 w-full rounded-xl text-sm font-semibold"
              disabled={loading}
            >
              {loading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Ingresando...
                </>
              ) : (
                'Ingresar'
              )}
            </Button>
          </form>
        </CardContent>
      </Card>

      {/* Modal: Olvidé mi contraseña */}
      <Dialog open={forgotOpen} onOpenChange={setForgotOpen}>
        <DialogContent className="max-w-sm rounded-2xl">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2 text-base font-semibold">
              <Info className="h-4 w-4 shrink-0 text-primary" />
              Restablecer contraseña
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-3 py-2">
            <p className="text-sm leading-relaxed text-muted-foreground">
              Para restablecer tu contraseña, comunícate con un{' '}
              <span className="font-medium text-foreground">Administrador</span> o{' '}
              <span className="font-medium text-foreground">SuperAdministrador</span> del sistema.
            </p>
            <p className="text-xs text-muted-foreground">
              Ellos podrán generar una nueva contraseña o enviarte instrucciones de recuperación a
              tu correo registrado.
            </p>
          </div>
          <Button
            className="w-full rounded-xl text-sm font-semibold"
            onClick={() => setForgotOpen(false)}
          >
            Entendido
          </Button>
        </DialogContent>
      </Dialog>
    </div>
  )
}
