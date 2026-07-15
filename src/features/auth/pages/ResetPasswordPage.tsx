import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { Lock, Eye, EyeOff, CheckCircle2, AlertTriangle } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { supabase } from '@services/supabase'
import { ROUTES } from '@constants/index'
import { toast } from 'sonner'

type PageState = 'verifying' | 'form' | 'done' | 'expired'

export function ResetPasswordPage() {
  const navigate = useNavigate()
  const [pageState, setPageState] = useState<PageState>('verifying')
  const [password, setPassword] = useState('')
  const [confirm, setConfirm] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [showConfirm, setShowConfirm] = useState(false)
  const [loading, setLoading] = useState(false)
  const [formError, setFormError] = useState('')

  useEffect(() => {
    // Supabase con detectSessionInUrl:true procesa el hash automáticamente.
    // PASSWORD_RECOVERY se dispara cuando el token del email es válido.
    const {
      data: { subscription },
    } = supabase.auth.onAuthStateChange((event) => {
      if (event === 'PASSWORD_RECOVERY') {
        setPageState('form')
      }
    })

    let expiredTimer: ReturnType<typeof setTimeout> | null = null

    // Fallback: si el hash ya fue procesado antes de montar el listener
    supabase.auth.getSession().then(({ data: { session } }) => {
      if (session) {
        setPageState((prev) => (prev === 'verifying' ? 'form' : prev))
      } else {
        // Esperar 3 s antes de marcar como expirado (el procesamiento del hash puede tardar)
        expiredTimer = setTimeout(() => {
          setPageState((prev) => (prev === 'verifying' ? 'expired' : prev))
        }, 3000)
      }
    })

    return () => {
      subscription.unsubscribe()
      if (expiredTimer !== null) clearTimeout(expiredTimer)
    }
  }, [])

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setFormError('')

    if (password.length < 8) {
      setFormError('La contraseña debe tener al menos 8 caracteres.')
      return
    }
    if (password !== confirm) {
      setFormError('Las contraseñas no coinciden.')
      return
    }

    setLoading(true)
    const { error } = await supabase.auth.updateUser({ password })
    setLoading(false)

    if (error) {
      setFormError(error.message)
      return
    }

    setPageState('done')
    toast.success('Contraseña actualizada correctamente.')

    setTimeout(async () => {
      await supabase.auth.signOut()
      navigate(ROUTES.LOGIN, { replace: true })
    }, 2500)
  }

  // ── Estados de la página ──────────────────────────────────────────────────

  if (pageState === 'verifying') {
    return (
      <Card className="w-full">
        <CardContent className="flex flex-col items-center gap-3 px-6 py-10 text-center">
          <div className="h-8 w-8 animate-spin rounded-full border-2 border-primary border-t-transparent" />
          <p className="text-sm font-medium">Verificando enlace...</p>
          <p className="text-xs text-muted-foreground">
            Si esto tarda mucho, el enlace puede haber expirado.
          </p>
        </CardContent>
      </Card>
    )
  }

  if (pageState === 'expired') {
    return (
      <Card className="w-full">
        <CardContent className="flex flex-col items-center gap-4 px-6 py-10 text-center">
          <div className="flex h-12 w-12 items-center justify-center rounded-full bg-destructive/10">
            <AlertTriangle className="h-6 w-6 text-destructive" />
          </div>
          <div>
            <p className="font-semibold">Enlace inválido o expirado</p>
            <p className="mt-1 text-sm text-muted-foreground">
              El enlace de recuperación ya no es válido. Solicita uno nuevo al administrador del
              sistema.
            </p>
          </div>
          <Button variant="outline" onClick={() => navigate(ROUTES.LOGIN, { replace: true })}>
            Volver al inicio de sesión
          </Button>
        </CardContent>
      </Card>
    )
  }

  if (pageState === 'done') {
    return (
      <Card className="w-full">
        <CardContent className="flex flex-col items-center gap-4 px-6 py-10 text-center">
          <div className="flex h-12 w-12 items-center justify-center rounded-full bg-green-500/10">
            <CheckCircle2 className="h-6 w-6 text-green-600 dark:text-green-400" />
          </div>
          <div>
            <p className="font-semibold">¡Contraseña actualizada!</p>
            <p className="mt-1 text-sm text-muted-foreground">
              Redirigiendo al inicio de sesión...
            </p>
          </div>
        </CardContent>
      </Card>
    )
  }

  // pageState === 'form'
  return (
    <Card className="w-full">
      <CardHeader className="pb-4">
        <div className="mb-3 flex h-11 w-11 items-center justify-center rounded-xl bg-primary/10">
          <Lock className="h-5 w-5 text-primary" />
        </div>
        <CardTitle className="text-xl">Nueva contraseña</CardTitle>
        <CardDescription className="text-sm">
          Ingresa y confirma tu nueva contraseña. Debe tener al menos 8 caracteres.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-1.5">
            <label className="text-sm font-medium">Nueva contraseña</label>
            <div className="relative">
              <Input
                type={showPassword ? 'text' : 'password'}
                placeholder="••••••••"
                value={password}
                onChange={(e) => {
                  setPassword(e.target.value)
                  setFormError('')
                }}
                className="pr-10"
                required
                minLength={8}
                autoComplete="new-password"
              />
              <button
                type="button"
                tabIndex={-1}
                onClick={() => setShowPassword((v) => !v)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
                aria-label={showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
              >
                {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
              </button>
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium">Confirmar contraseña</label>
            <div className="relative">
              <Input
                type={showConfirm ? 'text' : 'password'}
                placeholder="••••••••"
                value={confirm}
                onChange={(e) => {
                  setConfirm(e.target.value)
                  setFormError('')
                }}
                className="pr-10"
                required
                autoComplete="new-password"
              />
              <button
                type="button"
                tabIndex={-1}
                onClick={() => setShowConfirm((v) => !v)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
                aria-label={showConfirm ? 'Ocultar confirmación' : 'Mostrar confirmación'}
              >
                {showConfirm ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
              </button>
            </div>
          </div>

          {formError && (
            <p className="rounded-md bg-destructive/10 px-3 py-2 text-sm text-destructive">
              {formError}
            </p>
          )}

          <Button type="submit" className="w-full" disabled={loading}>
            {loading ? 'Actualizando...' : 'Actualizar contraseña'}
          </Button>

          <Button
            type="button"
            variant="ghost"
            className="w-full text-xs"
            onClick={() => navigate(ROUTES.LOGIN, { replace: true })}
          >
            Volver al inicio de sesión
          </Button>
        </form>
      </CardContent>
    </Card>
  )
}
