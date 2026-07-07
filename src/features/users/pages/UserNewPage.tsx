import { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { ArrowLeft, User } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Badge } from '@shared/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { FormField } from '@shared/components/FormField'
import { MOCK_SUCURSALES, MOCK_AREAS } from '@mocks/data'
import type { MockArea } from '@mocks/data'
import type { UserRole } from '@types-app/index'
import { ROUTES } from '@constants/index'

// ── Constantes de presentacion ────────────────────────────────────────────────

const ROL_LABELS: Record<UserRole, string> = {
  superadmin: 'SuperAdministrador',
  admin: 'Administrador',
  worker: 'Trabajador',
  user: 'Usuario',
}

const ROL_COLORS: Record<UserRole, string> = {
  superadmin:
    'bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400 border-transparent',
  admin: 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400 border-transparent',
  worker:
    'bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400 border-transparent',
  user: 'bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-300 border-transparent',
}

// ── Estado del formulario ─────────────────────────────────────────────────────

interface FormState {
  name: string
  apellido: string
  correo: string
  telefono: string
  rol: UserRole | ''
  sucursalId: string
  areaId: string
  estado: 'activo' | 'inactivo'
}

const INITIAL_FORM: FormState = {
  name: '',
  apellido: '',
  correo: '',
  telefono: '',
  rol: '',
  sucursalId: '',
  areaId: '',
  estado: 'activo',
}

// ── UserNewPage ───────────────────────────────────────────────────────────────

export function UserNewPage() {
  const navigate = useNavigate()

  const [form, setForm] = useState<FormState>(INITIAL_FORM)
  const [errors, setErrors] = useState<Partial<Record<keyof FormState, string>>>({})

  // Areas filtradas por empresa seleccionada
  const areasFiltered = useMemo<MockArea[]>(
    () => MOCK_AREAS.filter((a) => a.sucursalId === form.sucursalId && a.activo),
    [form.sucursalId],
  )

  // Iniciales calculadas en tiempo real
  const initials = useMemo(() => {
    const first = form.name.trim()[0] ?? ''
    const last = form.apellido.trim()[0] ?? ''
    return (first + last).toUpperCase() || null
  }, [form.name, form.apellido])

  // ── Handlers ────────────────────────────────────────────────────────────────

  function handleChange(field: keyof FormState, value: string) {
    setForm((prev) => ({
      ...prev,
      [field]: value,
      // Al cambiar empresa se limpia la sucursal
      ...(field === 'sucursalId' ? { areaId: '' } : {}),
    }))
    setErrors((prev) => ({ ...prev, [field]: undefined }))
  }

  function validate(): boolean {
    const next: Partial<Record<keyof FormState, string>> = {}
    if (!form.name.trim()) next.name = 'Ingresa tu nombre completo.'
    if (!form.apellido.trim()) next.apellido = 'Ingresa tu apellido completo.'
    if (!form.correo.trim()) next.correo = 'Ingresa un correo electronico valido.'
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.correo))
      next.correo = 'Ingresa un correo electronico valido.'
    if (!form.rol) next.rol = 'Selecciona un rol.'
    if (!form.sucursalId) next.sucursalId = 'Selecciona una empresa.'
    if (!form.estado) next.estado = 'Selecciona un estado.'
    setErrors(next)
    return Object.keys(next).length === 0
  }

  function handleSubmit() {
    if (!validate()) return

    const id = Math.random().toString(36).slice(2, 8)
    void id // id generado para uso futuro en BD

    toast.success('Usuario creado correctamente')
    navigate(ROUTES.USERS)
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
            </CardContent>
          </Card>

          {/* Card: Acceso y permisos */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-sm font-semibold">Acceso y permisos</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 p-3 pt-0">
              <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                <FormField label="Rol" required error={errors.rol}>
                  <Select value={form.rol} onValueChange={(v) => handleChange('rol', v)}>
                    <SelectTrigger className="h-9 text-sm">
                      <SelectValue placeholder="Seleccionar rol" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="worker">Trabajador</SelectItem>
                      <SelectItem value="admin">Administrador</SelectItem>
                      <SelectItem value="superadmin">SuperAdministrador</SelectItem>
                    </SelectContent>
                  </Select>
                </FormField>

                <FormField label="Estado" required error={errors.estado}>
                  <Select
                    value={form.estado}
                    onValueChange={(v) => handleChange('estado', v as 'activo' | 'inactivo')}
                  >
                    <SelectTrigger className="h-9 text-sm">
                      <SelectValue placeholder="Seleccionar estado" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="activo">Activo</SelectItem>
                      <SelectItem value="inactivo">Inactivo</SelectItem>
                    </SelectContent>
                  </Select>
                </FormField>
              </div>

              <FormField label="Empresa" required error={errors.sucursalId}>
                <Select
                  value={form.sucursalId}
                  onValueChange={(v) => handleChange('sucursalId', v)}
                >
                  <SelectTrigger className="h-9 text-sm">
                    <SelectValue placeholder="Seleccionar empresa" />
                  </SelectTrigger>
                  <SelectContent>
                    {MOCK_SUCURSALES.filter((s) => s.activo).map((s) => (
                      <SelectItem key={s.id} value={s.id}>
                        {s.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </FormField>

              <FormField label="Sucursal" optional>
                <Select
                  value={form.areaId}
                  onValueChange={(v) => handleChange('areaId', v)}
                  disabled={!form.sucursalId}
                >
                  <SelectTrigger className="h-9 text-sm">
                    <SelectValue
                      placeholder={
                        form.sucursalId
                          ? 'Selecciona una sucursal.'
                          : 'Primero selecciona una empresa.'
                      }
                    />
                  </SelectTrigger>
                  <SelectContent>
                    {areasFiltered.length === 0 ? (
                      <SelectItem value="_empty" disabled>
                        Sin sucursales disponibles
                      </SelectItem>
                    ) : (
                      areasFiltered.map((a) => (
                        <SelectItem key={a.id} value={a.id}>
                          {a.name}
                        </SelectItem>
                      ))
                    )}
                  </SelectContent>
                </Select>
              </FormField>
            </CardContent>
          </Card>

          {/* Footer de acciones */}
          <div className="flex items-center justify-end gap-2 pb-2">
            <Button variant="outline" onClick={handleCancel}>
              Cancelar
            </Button>
            <Button onClick={handleSubmit}>Crear usuario</Button>
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
                  {form.estado === 'activo' ? (
                    <Badge className="border-transparent bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400">
                      Activo
                    </Badge>
                  ) : (
                    <Badge variant="secondary">Inactivo</Badge>
                  )}
                </div>

                {form.sucursalId && (
                  <div className="flex items-center justify-between text-xs">
                    <span className="text-muted-foreground">Empresa</span>
                    <span className="max-w-[140px] truncate text-right font-medium">
                      {MOCK_SUCURSALES.find((s) => s.id === form.sucursalId)?.name ?? ''}
                    </span>
                  </div>
                )}

                {form.areaId && (
                  <div className="flex items-center justify-between text-xs">
                    <span className="text-muted-foreground">Sucursal</span>
                    <span className="max-w-[140px] truncate text-right font-medium">
                      {MOCK_AREAS.find((a) => a.id === form.areaId)?.name ?? ''}
                    </span>
                  </div>
                )}
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
