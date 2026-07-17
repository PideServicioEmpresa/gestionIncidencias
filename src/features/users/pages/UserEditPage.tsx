import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, List, Building2, MapPin } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Skeleton } from '@shared/ui/skeleton'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { FormField } from '@shared/components/FormField'
import {
  useUsuario,
  useActualizarPerfil,
  useToggleEstadoUsuario,
  useRoles,
  useCambiarRol,
} from '../hooks/useUsuarios'
import { useAuthStore } from '@store/auth.store'
import { useEmpresa } from '@features/empresas/hooks/useEmpresas'
import { useSucursal } from '@features/sucursales/hooks/useSucursales'
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

  const currentUser = useAuthStore((s) => s.user)
  const isSuperAdmin = currentUser?.rol === 'superadmin'

  const { data: user, isLoading, isError } = useUsuario(id ?? '')
  const actualizarPerfil = useActualizarPerfil()
  const toggleEstado = useToggleEstadoUsuario()
  const cambiarRol = useCambiarRol()

  const { data: rolesData } = useRoles()
  const roles = (rolesData?.items ?? []).filter(
    (r) => r.activo && (isSuperAdmin || r.codigo !== 'SUPERADMIN') && r.codigo !== 'SUPERVISOR',
  )

  const { data: empresa } = useEmpresa(user?.empresaId ?? '')
  const { data: sucursal } = useSucursal(user?.sucursalId ?? '')

  const [form, setForm] = useState<UserFormState>({
    nombre: '',
    apellido: '',
    correo: '',
    telefono: '',
    rol: 'usuario',
    estado: 'activo',
  })
  const [errors, setErrors] = useState<Partial<Record<keyof UserFormState, string>>>({})

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
    setErrors(next)
    return Object.keys(next).length === 0
  }

  function handleSave() {
    if (!validate() || !id || !user) return

    const rolOriginal = user.rol.toLowerCase() as UserRole
    if (form.rol !== rolOriginal) {
      cambiarRol.mutate({ id, nuevoRol: form.rol.toUpperCase() })
    }

    const estadoCambiado = form.estado !== (user.activo ? 'activo' : 'inactivo')
    if (estadoCambiado) {
      toggleEstado.mutate({ id, activar: form.estado === 'activo' })
    }

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
              No se encontró el usuario con id <strong>{id}</strong>.
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

  const isSaving = actualizarPerfil.isPending || toggleEstado.isPending || cambiarRol.isPending

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

      <div className="space-y-4">
        {/* Card: Datos personales */}
        <Card>
          <CardHeader className="px-4 pb-2 pt-4">
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Datos personales
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4 p-4 pt-0">
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

            <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
              <FormField label="Correo electrónico">
                <Input
                  className="h-8 text-xs"
                  type="email"
                  value={form.correo}
                  disabled
                  title="El correo no puede modificarse desde esta pantalla"
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
          </CardContent>
        </Card>

        {/* Card: Acceso */}
        <Card>
          <CardHeader className="px-4 pb-2 pt-4">
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Acceso y permisos
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4 p-4 pt-0">
            <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
              <FormField label="Rol" required error={errors.rol}>
                <Select value={form.rol} onValueChange={(v) => handleChange('rol', v as UserRole)}>
                  <SelectTrigger className="h-8 text-xs">
                    <SelectValue placeholder="Seleccionar rol" />
                  </SelectTrigger>
                  <SelectContent>
                    {roles.map((r) => (
                      <SelectItem key={r.codigo} value={r.codigo.toLowerCase()}>
                        {r.nombre}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </FormField>

              <FormField label="Estado" required>
                <Select
                  value={form.estado}
                  onValueChange={(v) => handleChange('estado', v as 'activo' | 'inactivo')}
                >
                  <SelectTrigger className="h-8 text-xs">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="activo">Activo</SelectItem>
                    <SelectItem value="inactivo">Inactivo</SelectItem>
                  </SelectContent>
                </Select>
              </FormField>
            </div>
          </CardContent>
        </Card>

        {/* Card: Asignación (solo lectura) */}
        <Card>
          <CardHeader className="px-4 pb-2 pt-4">
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Asignación
            </CardTitle>
          </CardHeader>
          <CardContent className="p-4 pt-0">
            <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
              <div className="flex items-center gap-2.5 rounded-lg border bg-muted/20 px-3 py-2.5">
                <Building2 className="h-4 w-4 shrink-0 text-muted-foreground" />
                <div className="min-w-0">
                  <p className="text-[10px] text-muted-foreground">Empresa</p>
                  <p className="truncate text-xs font-medium">
                    {empresa?.nombreComercial ?? user.empresaId}
                  </p>
                </div>
              </div>

              <div className="flex items-center gap-2.5 rounded-lg border bg-muted/20 px-3 py-2.5">
                <MapPin className="h-4 w-4 shrink-0 text-muted-foreground" />
                <div className="min-w-0">
                  <p className="text-[10px] text-muted-foreground">Sucursal</p>
                  <p className="truncate text-xs font-medium">
                    {sucursal?.nombre ?? user.sucursalId}
                  </p>
                </div>
              </div>
            </div>
            <p className="mt-2 text-[10px] text-muted-foreground">
              Para cambiar la empresa o sucursal, contacta al superadministrador.
            </p>
          </CardContent>
        </Card>

        {/* Footer actions */}
        <div className="flex justify-end gap-2 pb-2">
          <Button variant="outline" size="sm" onClick={() => navigate(userDetailPath(id ?? ''))}>
            Cancelar
          </Button>
          <Button size="sm" disabled={isSaving} onClick={handleSave}>
            {isSaving ? 'Guardando...' : 'Guardar cambios'}
          </Button>
        </div>
      </div>
    </div>
  )
}
