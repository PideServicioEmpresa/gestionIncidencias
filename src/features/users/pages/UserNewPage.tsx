import { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { ArrowLeft, User, Eye, EyeOff } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Badge } from '@shared/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { FormField } from '@shared/components/FormField'
import { useCrearUsuario } from '../hooks/useUsuarios'
import { useAuthStore } from '@store/auth.store'
import type { UserRole } from '@types-app/index'
import { ROUTES } from '@constants/index'

// ── Constantes de presentación ────────────────────────────────────────────────

const ROL_LABELS: Record<UserRole, string> = {
  superadmin: 'SuperAdministrador',
  admin: 'Administrador',
  supervisor: 'Supervisor',
  tecnico: 'Técnico',
  trabajador: 'Trabajador',
  usuario: 'Usuario',
}

const ROL_COLORS: Record<UserRole, string> = {
  superadmin:
    'bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400 border-transparent',
  admin: 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400 border-transparent',
  supervisor: 'bg-cyan-100 text-cyan-700 dark:bg-cyan-900/30 dark:text-cyan-400 border-transparent',
  tecnico:
    'bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400 border-transparent',
  trabajador:
    'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400 border-transparent',
  usuario: 'bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-300 border-transparent',
}

// ── Estado del formulario ─────────────────────────────────────────────────────

interface FormState {
  name: string
  apellido: string
  correo: string
  telefono: string
  contrasena: string
  confirmarContrasena: string
  rol: UserRole | ''
}

const INITIAL_FORM: FormState = {
  name: '',
  apellido: '',
  correo: '',
  telefono: '',
  contrasena: '',
  confirmarContrasena: '',
  rol: '',
}

// ── UserNewPage ───────────────────────────────────────────────────────────────

export function UserNewPage() {
  const navigate = useNavigate()
  const crearUsuario = useCrearUsuario()
  const currentUser = useAuthStore((s) => s.user)

  const [form, setForm] = useState<FormState>(INITIAL_FORM)
  const [errors, setErrors] = useState<Partial<Record<keyof FormState, string>>>({})
  const [showPw, setShowPw] = useState(false)
  const [showConfirm, setShowConfirm] = useState(false)

  // Iniciales calculadas en tiempo real
  const initials = useMemo(() => {
    const first = form.name.trim()[0] ?? ''
    const last = form.apellido.trim()[0] ?? ''
    return (first + last).toUpperCase() || null
  }, [form.name, form.apellido])

  // ── Handlers ────────────────────────────────────────────────────────────────

  function handleChange(field: keyof FormState, value: string) {
    setForm((prev) => ({ ...prev, [field]: value }))
    setErrors((prev) => ({ ...prev, [field]: undefined }))
  }

  function validate(): boolean {
    const next: Partial<Record<keyof FormState, string>> = {}
    if (!form.name.trim()) next.name = 'Ingresa el nombre completo.'
    if (!form.apellido.trim()) next.apellido = 'Ingresa el apellido completo.'
    if (!form.correo.trim()) next.correo = 'Ingresa un correo electrónico válido.'
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.correo))
      next.correo = 'Ingresa un correo electrónico válido.'
    if (!form.contrasena) next.contrasena = 'Ingresa una contraseña temporal.'
    else if (form.contrasena.length < 8) next.contrasena = 'Mínimo 8 caracteres.'
    if (form.confirmarContrasena !== form.contrasena)
      next.confirmarContrasena = 'Las contraseñas no coinciden.'
    if (!form.rol) next.rol = 'Selecciona un rol.'
    setErrors(next)
    return Object.keys(next).length === 0
  }

  function handleSubmit() {
    if (!validate()) return
    const sucursalId = currentUser?.sucursalId
    if (!sucursalId) {
      toast.error('No se pudo determinar la sucursal del usuario actual.')
      return
    }
    const nombreUsuario = form.correo.split('@')[0] ?? form.correo
    crearUsuario.mutate(
      {
        sucursalId,
        nombre: form.name.trim(),
        apellido: form.apellido.trim(),
        correo: form.correo.trim(),
        nombreUsuario,
        contrasena: form.contrasena,
        telefono: form.telefono.trim() || undefined,
        rol: form.rol.toUpperCase(),
      },
      {
        onSuccess: () => navigate(ROUTES.USERS),
      },
    )
  }

  function handleCancel() {
    navigate(ROUTES.USERS)
  }

  // ── Render ──────────────────────────────────────────────────────────────────

  return (
    <div className="px-3 py-3 lg:px-5">
      {/* Page header */}
      <div className="mb-4 flex items-center justify-between gap-3">
        <div className="flex items-center gap-2">
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8 shrink-0"
            onClick={handleCancel}
            aria-label="Volver a usuarios"
          >
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <div>
            <h2 className="text-base font-semibold tracking-tight">Nuevo Usuario</h2>
            <p className="text-xs text-muted-foreground">
              Completa los datos para registrar un nuevo usuario
            </p>
          </div>
        </div>
        <Button variant="outline" size="sm" onClick={handleCancel}>
          Cancelar
        </Button>
      </div>

      {/* Layout principal */}
      <div className="flex flex-col gap-4 lg:flex-row lg:items-start">
        {/* ── Columna izquierda: formulario ─────────────────────────────────── */}
        <div className="flex-1 space-y-4">
          {/* Card: Informacion personal */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-sm font-semibold">Información personal</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 p-3 pt-0">
              <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                <FormField label="Nombre completo" required error={errors.name}>
                  <Input
                    className="h-9 text-sm"
                    placeholder="Nombre"
                    value={form.name}
                    onChange={(e) => handleChange('name', e.target.value)}
                    autoComplete="given-name"
                  />
                </FormField>

                <FormField label="Apellido completo" required error={errors.apellido}>
                  <Input
                    className="h-9 text-sm"
                    placeholder="Apellido"
                    value={form.apellido}
                    onChange={(e) => handleChange('apellido', e.target.value)}
                    autoComplete="family-name"
                  />
                </FormField>
              </div>

              <FormField label="Correo electrónico" required error={errors.correo}>
                <Input
                  className="h-9 text-sm"
                  type="email"
                  placeholder="correo@empresa.com"
                  value={form.correo}
                  onChange={(e) => handleChange('correo', e.target.value)}
                  autoComplete="email"
                />
              </FormField>

              <FormField label="Teléfono" optional>
                <Input
                  className="h-9 text-sm"
                  type="tel"
                  placeholder="+51 999 000 000"
                  value={form.telefono}
                  onChange={(e) => handleChange('telefono', e.target.value)}
                  autoComplete="tel"
                />
              </FormField>

              <FormField label="Contraseña temporal" required error={errors.contrasena}>
                <div className="relative">
                  <Input
                    className="h-9 pr-9 text-sm"
                    type={showPw ? 'text' : 'password'}
                    placeholder="Mínimo 8 caracteres"
                    value={form.contrasena}
                    onChange={(e) => handleChange('contrasena', e.target.value)}
                    autoComplete="new-password"
                  />
                  <button
                    type="button"
                    tabIndex={-1}
                    onClick={() => setShowPw((v) => !v)}
                    className="absolute right-2.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground focus-visible:outline-none"
                  >
                    {showPw ? <EyeOff className="h-3.5 w-3.5" /> : <Eye className="h-3.5 w-3.5" />}
                  </button>
                </div>
              </FormField>

              <FormField label="Confirmar contraseña" required error={errors.confirmarContrasena}>
                <div className="relative">
                  <Input
                    className="h-9 pr-9 text-sm"
                    type={showConfirm ? 'text' : 'password'}
                    placeholder="Repite la contraseña"
                    value={form.confirmarContrasena}
                    onChange={(e) => handleChange('confirmarContrasena', e.target.value)}
                    autoComplete="new-password"
                  />
                  <button
                    type="button"
                    tabIndex={-1}
                    onClick={() => setShowConfirm((v) => !v)}
                    className="absolute right-2.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground focus-visible:outline-none"
                  >
                    {showConfirm ? (
                      <EyeOff className="h-3.5 w-3.5" />
                    ) : (
                      <Eye className="h-3.5 w-3.5" />
                    )}
                  </button>
                </div>
              </FormField>
            </CardContent>
          </Card>

          {/* Card: Acceso y permisos */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-sm font-semibold">Acceso y permisos</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 p-3 pt-0">
              <FormField label="Rol" required error={errors.rol}>
                <Select value={form.rol} onValueChange={(v) => handleChange('rol', v)}>
                  <SelectTrigger className="h-9 text-sm">
                    <SelectValue placeholder="Seleccionar rol" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="trabajador">Trabajador</SelectItem>
                    <SelectItem value="tecnico">Técnico</SelectItem>
                    <SelectItem value="supervisor">Supervisor</SelectItem>
                    <SelectItem value="admin">Administrador</SelectItem>
                    <SelectItem value="superadmin">SuperAdministrador</SelectItem>
                  </SelectContent>
                </Select>
              </FormField>

              {currentUser?.sucursalId && (
                <p className="text-xs text-muted-foreground">
                  El usuario será creado en tu sucursal actual.
                </p>
              )}
            </CardContent>
          </Card>

          {/* Footer de acciones */}
          <div className="flex items-center justify-end gap-2 pb-2">
            <Button variant="outline" onClick={handleCancel}>
              Cancelar
            </Button>
            <Button disabled={crearUsuario.isPending} onClick={handleSubmit}>
              {crearUsuario.isPending ? 'Creando...' : 'Crear usuario'}
            </Button>
          </div>
        </div>

        {/* ── Columna derecha: panel de resumen (solo desktop) ──────────────── */}
        <div className="w-full lg:w-80 lg:shrink-0">
          <Card className="lg:sticky lg:top-4">
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-sm font-semibold">Resumen</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4 p-3 pt-0">
              {/* Avatar con iniciales */}
              <div className="flex flex-col items-center gap-3 py-2">
                <div className="flex h-16 w-16 items-center justify-center rounded-full bg-primary/20 text-lg font-semibold text-primary">
                  {initials ? (
                    <span>{initials}</span>
                  ) : (
                    <User className="h-7 w-7 text-primary/60" />
                  )}
                </div>
                <div className="text-center">
                  <p className="text-sm font-medium text-foreground">
                    {form.name.trim() || form.apellido.trim()
                      ? `${form.name.trim()} ${form.apellido.trim()}`.trim()
                      : 'Nombre del usuario'}
                  </p>
                  {form.correo.trim() && (
                    <p className="mt-0.5 text-xs text-muted-foreground">{form.correo.trim()}</p>
                  )}
                </div>
              </div>

              {/* Badges de rol y estado */}
              <div className="space-y-2">
                <div className="flex items-center justify-between text-xs">
                  <span className="text-muted-foreground">Rol</span>
                  {form.rol ? (
                    <Badge className={ROL_COLORS[form.rol as UserRole]}>
                      {ROL_LABELS[form.rol as UserRole]}
                    </Badge>
                  ) : (
                    <span className="text-muted-foreground/60">Sin asignar</span>
                  )}
                </div>

                <div className="flex items-center justify-between text-xs">
                  <span className="text-muted-foreground">Estado</span>
                  <Badge className="border-transparent bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400">
                    Activo
                  </Badge>
                </div>
              </div>

              {/* Hint */}
              <p className="rounded-lg border border-dashed p-3 text-[11px] text-muted-foreground">
                El usuario recibirá un correo con sus credenciales de acceso una vez creado.
              </p>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
