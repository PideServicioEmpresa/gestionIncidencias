import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { AlertCircle, Eye, EyeOff, Info, Loader2 } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Label } from '@shared/ui/label'
import { Card, CardContent } from '@shared/ui/card'
import { FormField } from '@shared/components/FormField'
import { Separator } from '@shared/ui/separator'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@shared/ui/dialog'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { ROUTES } from '@constants/index'
import { useAuthStore } from '@store/auth.store'
import { MOCK_USERS, MOCK_SUCURSALES, MOCK_AREAS } from '@mocks/data'
import type { UserRole } from '@types-app/index'

const loginSchema = z.object({
  correo: z.string().email('Ingresa un correo electronico valido.'),
  contrasena: z.string().min(1, 'Ingresa tu contrasena.'),
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
  const [forgotOpen, setForgotOpen] = useState(false)
  const [selectedSucursalId, setSelectedSucursalId] = useState<string>('')
  const [selectedAreaId, setSelectedAreaId] = useState<string>('')
  const [empresaError, setEmpresaError] = useState('')
  const [sucursalError, setSucursalError] = useState('')

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<LoginForm>({ resolver: zodResolver(loginSchema) })

  const filteredAreas = MOCK_AREAS.filter((a) => a.sucursalId === selectedSucursalId && a.activo)

  const activeSucursales = MOCK_SUCURSALES.filter((s) => s.activo)

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
    let hasExtraError = false
    if (!selectedSucursalId) {
      setEmpresaError('Selecciona una empresa.')
      hasExtraError = true
    } else {
      setEmpresaError('')
    }
    if (!selectedAreaId) {
      setSucursalError('Selecciona una sucursal.')
      hasExtraError = true
    } else {
      setSucursalError('')
    }
    if (hasExtraError) return

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

  const handleSucursalChange = (value: string) => {
    setSelectedSucursalId(value)
    setSelectedAreaId('')
    setEmpresaError('')
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

            {/* Contraseña — label row tiene enlace adicional, se construye manualmente */}
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

            {/* Empresa */}
            <FormField label="Empresa" required error={empresaError}>
              <Select
                value={selectedSucursalId}
                onValueChange={handleSucursalChange}
                disabled={loading}
              >
                <SelectTrigger className="h-10 rounded-xl text-sm">
                  <SelectValue placeholder="Selecciona una empresa" />
                </SelectTrigger>
                <SelectContent>
                  {activeSucursales.map((s) => (
                    <SelectItem key={s.id} value={s.id}>
                      {s.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </FormField>

            {/* Sucursal (dependiente de Empresa) */}
            <FormField label="Sucursal" required error={sucursalError}>
              <Select
                value={selectedAreaId}
                onValueChange={(value) => {
                  setSelectedAreaId(value)
                  setSucursalError('')
                }}
                disabled={loading || !selectedSucursalId}
              >
                <SelectTrigger className="h-10 rounded-xl text-sm">
                  <SelectValue
                    placeholder={
                      selectedSucursalId
                        ? 'Selecciona una sucursal'
                        : 'Primero selecciona una empresa'
                    }
                  />
                </SelectTrigger>
                <SelectContent>
                  {filteredAreas.map((a) => (
                    <SelectItem key={a.id} value={a.id}>
                      {a.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </FormField>

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

      {/* Demo access — outside card, visually secondary */}
      <div className="space-y-2.5">
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
              className="h-9 rounded-xl text-xs"
            >
              {u.label}
            </Button>
          ))}
        </div>
      </div>

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
