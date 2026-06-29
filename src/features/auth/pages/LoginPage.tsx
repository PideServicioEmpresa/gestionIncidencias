import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Eye, EyeOff, Loader2 } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Label } from '@shared/ui/label'
import { Separator } from '@shared/ui/separator'
import { ROUTES } from '@constants/index'
import { useAuthStore } from '@store/auth.store'
import { MOCK_USERS } from '@mocks/data'
import type { UserRole } from '@types-app/index'

const loginSchema = z.object({
  correo: z.string().email('Ingresa un correo válido'),
  contrasena: z.string().min(1, 'La contraseña es requerida'),
})
type LoginForm = z.infer<typeof loginSchema>

const DEMO_USERS = [
  { label: 'Trabajador', correo: 'mlopez@empresa.com', id: 'u3' },
  { label: 'Administrador', correo: 'jperez@empresa.com', id: 'u2' },
  { label: 'SuperAdmin', correo: 'gcbarrionuevo@empresa.com', id: 'u1' },
]

export function LoginPage() {
  const navigate = useNavigate()
  const setUser = useAuthStore((s) => s.setUser)
  const [showPassword, setShowPassword] = useState(false)
  const [loading, setLoading] = useState(false)

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<LoginForm>({ resolver: zodResolver(loginSchema) })

  const loginWithUser = (userId: string) => {
    const mockUser = MOCK_USERS.find((u) => u.id === userId)!
    setUser({
      id: mockUser.id,
      authUserId: mockUser.id,
      nombre: mockUser.name,
      apellido: mockUser.apellido,
      correo: mockUser.correo,
      usuario: mockUser.usuario,
      rolId: mockUser.id,
      rol: mockUser.rol as UserRole,
      sucursalId: mockUser.sucursalId,
      areaId: mockUser.areaId,
      estadoLaboral: 'activo',
      activo: true,
    })
    navigate(ROUTES.DASHBOARD)
  }

  const onSubmit = (_data: LoginForm) => {
    setLoading(true)
    setTimeout(() => {
      const found = MOCK_USERS.find((u) => u.correo === _data.correo)
      const target = found ?? MOCK_USERS[2]
      loginWithUser(target.id)
    }, 800)
  }

  const handleDemoLogin = (userId: string, correo: string) => {
    setValue('correo', correo)
    setValue('contrasena', 'demo1234')
    setLoading(true)
    setTimeout(() => {
      loginWithUser(userId)
    }, 600)
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold tracking-tight">Iniciar sesión</h2>
        <p className="mt-1 text-sm text-muted-foreground">
          Ingresa tus credenciales para continuar
        </p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div className="space-y-2">
          <Label htmlFor="correo">Correo electrónico</Label>
          <Input
            id="correo"
            type="email"
            placeholder="tu@empresa.com"
            autoComplete="email"
            disabled={loading}
            {...register('correo')}
          />
          {errors.correo && <p className="text-xs text-destructive">{errors.correo.message}</p>}
        </div>

        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <Label htmlFor="contrasena">Contraseña</Label>
            <button
              type="button"
              className="text-xs text-primary underline-offset-4 hover:underline"
            >
              ¿Olvidaste tu contraseña?
            </button>
          </div>
          <div className="relative">
            <Input
              id="contrasena"
              type={showPassword ? 'text' : 'password'}
              placeholder="••••••••"
              autoComplete="current-password"
              disabled={loading}
              className="pr-10"
              {...register('contrasena')}
            />
            <button
              type="button"
              className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
              onClick={() => setShowPassword((v) => !v)}
              tabIndex={-1}
            >
              {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
            </button>
          </div>
          {errors.contrasena && (
            <p className="text-xs text-destructive">{errors.contrasena.message}</p>
          )}
        </div>

        <Button type="submit" className="w-full" disabled={loading}>
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

      {/* Demo access */}
      <div className="space-y-3">
        <div className="flex items-center gap-3">
          <Separator className="flex-1" />
          <span className="text-xs text-muted-foreground">Acceso demo</span>
          <Separator className="flex-1" />
        </div>
        <p className="text-center text-xs text-muted-foreground">
          Prueba la aplicación con distintos roles
        </p>
        <div className="grid grid-cols-3 gap-2">
          {DEMO_USERS.map((u) => (
            <Button
              key={u.id}
              variant="outline"
              size="sm"
              disabled={loading}
              onClick={() => handleDemoLogin(u.id, u.correo)}
              className="text-xs"
            >
              {u.label}
            </Button>
          ))}
        </div>
      </div>
    </div>
  )
}
