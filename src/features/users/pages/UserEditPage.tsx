import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, List } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Skeleton } from '@shared/ui/skeleton'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { FormField } from '@shared/components/FormField'
import { useUsuario, useActualizarPerfil } from '../hooks/useUsuarios'
import type { UserRole } from '@types-app/index'
import { userDetailPath, ROUTES } from '@constants/index'

// ── Tipos de formulario ───────────────────────────────────────────────────────

interface UserFormState {
  nombre: string
  apellido: string
  correo: string
  telefono: string
  rol: UserRole
  estado: 'activo' | 'inactivo'
}

// ── UserEditPage ──────────────────────────────────────────────────────────────

export function UserEditPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()

  const { data: user, isLoading, isError } = useUsuario(id ?? '')
  const actualizarPerfil = useActualizarPerfil()

  const [form, setForm] = useState<UserFormState>({
    nombre: '',
    apellido: '',
    correo: '',
    telefono: '',
    rol: 'usuario',
    estado: 'activo',
  })
  const [errors, setErrors] = useState<Partial<Record<keyof UserFormState, string>>>({})

  // Inicializar formulario cuando los datos del usuario cargan
  useEffect(() => {
    if (user) {
      setForm({
        nombre: user.nombre,
        apellido: user.apellido,
        correo: user.correo,
        telefono: user.telefono ?? '',
        rol: user.rol.toLowerCase() as UserRole,
        estado: user.activo ? 'activo' : 'inactivo',
      })
    }
  }, [user])

  function handleChange(field: keyof UserFormState, value: string) {
    setForm((prev) => ({ ...prev, [field]: value }))
    setErrors((prev) => ({ ...prev, [field]: undefined }))
  }

  function validate(): boolean {
    const next: Partial<Record<keyof UserFormState, string>> = {}
    if (!form.nombre.trim()) next.nombre = 'Ingresa el nombre completo.'
    if (!form.apellido.trim()) next.apellido = 'Ingresa el apellido completo.'
    if (!form.correo.trim()) next.correo = 'Ingresa un correo electrónico válido.'
    if (!form.rol) next.rol = 'Selecciona un rol.'
    setErrors(next)
    return Object.keys(next).length === 0
  }

  function handleSave() {
    if (!validate() || !id) return
    actualizarPerfil.mutate(
      {
        id,
        data: {
          nombre: form.nombre.trim(),
          apellido: form.apellido.trim(),
          telefono: form.telefono.trim() || undefined,
        },
      },
      {
        onSuccess: () => navigate(userDetailPath(id)),
      },
    )
  }

  // ── Estado de carga ────────────────────────────────────────────────────────

  if (isLoading) {
    return (
      <div className="px-3 py-3 lg:px-5">
        <div className="mb-4 flex items-center gap-2">
          <Skeleton className="h-5 w-32" />
          <Skeleton className="h-3 w-48" />
        </div>
        <Card>
          <CardContent className="space-y-4 p-4">
            <div className="grid grid-cols-2 gap-3">
              <Skeleton className="h-8 w-full" />
              <Skeleton className="h-8 w-full" />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <Skeleton className="h-8 w-full" />
              <Skeleton className="h-8 w-full" />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <Skeleton className="h-8 w-full" />
              <Skeleton className="h-8 w-full" />
            </div>
          </CardContent>
        </Card>
      </div>
    )
  }

  // ── Usuario no encontrado ──────────────────────────────────────────────────

  if (isError || !user) {
    return (
      <div className="px-3 py-3 lg:px-5">
        <Card>
          <CardContent className="flex flex-col items-center gap-3 py-12 text-center">
            <p className="text-sm text-muted-foreground">
              No se encontro el usuario con id <strong>{id}</strong>.
            </p>
            <Button variant="outline" size="sm" onClick={() => navigate(ROUTES.USERS)}>
              <List className="mr-2 h-4 w-4" />
              Volver al listado
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  // ── Render ─────────────────────────────────────────────────────────────────

  return (
    <div className="px-3 py-3 lg:px-5">
      {/* Page header */}
      <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-base font-semibold tracking-tight">Editar Usuario</h2>
          <p className="text-xs text-muted-foreground">{user.nombreCompleto}</p>
        </div>
        <div className="flex flex-wrap gap-2">
          <Button variant="outline" size="sm" onClick={() => navigate(userDetailPath(id ?? ''))}>
            Cancelar
          </Button>
          <Button variant="ghost" size="sm" onClick={() => navigate(ROUTES.USERS)}>
            <ArrowLeft className="mr-1.5 h-4 w-4" />
            Volver al listado
          </Button>
        </div>
      </div>

      {/* Form card */}
      <Card>
        <CardHeader className="px-4 pb-2 pt-4">
          <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            Datos del usuario
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4 p-4 pt-0">
          {/* Nombre y apellido */}
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <FormField label="Nombre completo" required error={errors.nombre}>
              <Input
                className="h-8 text-xs"
                placeholder="Nombre"
                value={form.nombre}
                onChange={(e) => handleChange('nombre', e.target.value)}
                error={!!errors.nombre}
              />
            </FormField>
            <FormField label="Apellido completo" required error={errors.apellido}>
              <Input
                className="h-8 text-xs"
                placeholder="Apellido"
                value={form.apellido}
                onChange={(e) => handleChange('apellido', e.target.value)}
                error={!!errors.apellido}
              />
            </FormField>
          </div>

          {/* Correo y teléfono */}
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <FormField label="Correo electrónico" required error={errors.correo}>
              <Input
                className="h-8 text-xs"
                placeholder="correo@empresa.com"
                type="email"
                value={form.correo}
                disabled
                onChange={(e) => handleChange('correo', e.target.value)}
                error={!!errors.correo}
              />
            </FormField>
            <FormField label="Teléfono" optional>
              <Input
                className="h-8 text-xs"
                placeholder="+51 999 000 000"
                value={form.telefono}
                onChange={(e) => handleChange('telefono', e.target.value)}
              />
            </FormField>
          </div>

          {/* Rol y estado */}
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <FormField label="Rol" required error={errors.rol}>
              {/* TODO: Cambio de rol via PUT /usuarios/{id}/rol — implementar flujo separado */}
              <Select value={form.rol} onValueChange={(v) => handleChange('rol', v)} disabled>
                <SelectTrigger className="h-8 text-xs">
                  <SelectValue placeholder="Seleccionar rol" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="superadmin">SuperAdmin</SelectItem>
                  <SelectItem value="admin">Administrador</SelectItem>
                  <SelectItem value="supervisor">Supervisor</SelectItem>
                  <SelectItem value="tecnico">Técnico</SelectItem>
                  <SelectItem value="trabajador">Trabajador</SelectItem>
                  <SelectItem value="usuario">Usuario</SelectItem>
                </SelectContent>
              </Select>
            </FormField>

            <FormField label="Estado" required>
              {/* TODO: Cambio de estado via PATCH /usuarios/{id}/activar|desactivar — implementar flujo separado */}
              <Select
                value={form.estado}
                onValueChange={(v) => handleChange('estado', v as 'activo' | 'inactivo')}
                disabled
              >
                <SelectTrigger className="h-8 text-xs">
                  <SelectValue placeholder="Seleccionar estado" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="activo">Activo</SelectItem>
                  <SelectItem value="inactivo">Inactivo</SelectItem>
                </SelectContent>
              </Select>
            </FormField>
          </div>

          {/* Empresa y sucursal — sin API aún */}
          {/* TODO: Conectar selects de empresa/sucursal cuando los endpoints estén disponibles */}
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <FormField label="Empresa" required>
              <Select disabled>
                <SelectTrigger className="h-8 text-xs">
                  <SelectValue placeholder="Sin datos de empresa" />
                </SelectTrigger>
                <SelectContent />
              </Select>
            </FormField>

            <FormField label="Sucursal" optional>
              <Select disabled>
                <SelectTrigger className="h-8 text-xs">
                  <SelectValue placeholder="Sin datos de sucursal" />
                </SelectTrigger>
                <SelectContent />
              </Select>
            </FormField>
          </div>

          {/* Footer actions */}
          <div className="flex justify-end gap-2 border-t pt-4">
            <Button variant="outline" size="sm" onClick={() => navigate(userDetailPath(id ?? ''))}>
              Cancelar
            </Button>
            <Button size="sm" disabled={actualizarPerfil.isPending} onClick={handleSave}>
              {actualizarPerfil.isPending ? 'Guardando...' : 'Guardar cambios'}
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
