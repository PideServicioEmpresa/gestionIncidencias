import { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Search,
  Plus,
  MoreHorizontal,
  Check,
  X,
  Pencil,
  Power,
  Building2,
  MapPin,
} from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Badge } from '@shared/ui/badge'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@shared/ui/dropdown-menu'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { ConfirmDialog } from '@shared/components/ConfirmDialog'
import { UserListSkeleton } from '@shared/components/PageSkeletons'
import {
  useUsuarios,
  useToggleEstadoUsuario,
  useEliminarUsuario,
  useRestablecerContrasena,
} from '../hooks/useUsuarios'
import type { UsuarioResumenDto } from '../services/usuarioService'
import type { UserRole } from '@types-app/index'
import {
  ROUTES,
  userDetailPath,
  userEditPath,
  empresaEditPath,
  sucursalEditPath,
} from '@constants/index'
import { useEmpresas, useToggleEmpresa } from '@features/empresas/hooks/useEmpresas'
import type { EmpresaResumenDto } from '@features/empresas/services/empresaService'
import { useSucursales, useToggleSucursal } from '@features/sucursales/hooks/useSucursales'
import type { SucursalResumenDto } from '@features/sucursales/services/sucursalService'

// ── Tipos de presentación ─────────────────────────────────────────────────────

interface DisplayUser {
  id: string
  fullName: string
  initials: string
  correo: string
  rol: UserRole
  estadoLaboral: string
  activo: boolean
  sucursal: string
  area: string | undefined
  telefono: string | undefined
}

// ── Mapper DTO → DisplayUser ──────────────────────────────────────────────────

function mapToDisplayUser(dto: UsuarioResumenDto): DisplayUser {
  const partes = dto.nombreCompleto.trim().split(' ')
  const initials = partes
    .map((p) => p[0] ?? '')
    .join('')
    .slice(0, 2)
    .toUpperCase()
  return {
    id: dto.id,
    fullName: dto.nombreCompleto,
    initials,
    correo: dto.correo,
    rol: dto.rol.toLowerCase() as UserRole,
    estadoLaboral: dto.estadoLaboral,
    activo: dto.activo,
    sucursal: '',
    area: undefined,
    telefono: undefined,
  }
}

// ── Constantes de presentación ────────────────────────────────────────────────

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

const ROL_LABELS: Record<UserRole, string> = {
  superadmin: 'SuperAdmin',
  admin: 'Administrador',
  supervisor: 'Supervisor',
  tecnico: 'Técnico',
  trabajador: 'Trabajador',
  usuario: 'Usuario',
}

// ── UserRow ───────────────────────────────────────────────────────────────────

interface UserRowProps {
  user: DisplayUser
  onView: (user: DisplayUser) => void
  onEdit: (user: DisplayUser) => void
  onDelete: (user: DisplayUser) => void
  onToggleStatus: (user: DisplayUser) => void
  onResetPw: (user: DisplayUser) => void
}

function UserRow({ user, onView, onEdit, onDelete, onToggleStatus, onResetPw }: UserRowProps) {
  return (
    <Card className="transition-all hover:border-primary/30 hover:shadow-sm">
      <CardContent className="p-3">
        <div className="flex items-start gap-3">
          {/* Avatar */}
          <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-primary/20 text-xs font-medium text-primary">
            {user.initials}
          </div>

          {/* Info */}
          <div className="min-w-0 flex-1">
            <div className="flex flex-wrap items-center gap-2">
              <span className="font-semibold">{user.fullName}</span>
              <Badge className={ROL_COLORS[user.rol]}>{ROL_LABELS[user.rol]}</Badge>
              {!user.activo && (
                <Badge variant="outline" className="text-muted-foreground">
                  Inactivo
                </Badge>
              )}
            </div>
            <p className="mt-0.5 text-xs text-muted-foreground">{user.correo}</p>
            <div className="mt-1 flex flex-wrap gap-x-3 gap-y-0.5 text-[11px] text-muted-foreground">
              <span>{user.sucursal}</span>
              {user.area && <span>· {user.area}</span>}
              {user.telefono && <span>· {user.telefono}</span>}
            </div>
          </div>

          {/* Status + actions */}
          <div className="flex shrink-0 items-center gap-2">
            <div className="hidden items-center gap-1.5 sm:flex">
              {user.activo ? (
                <span className="flex items-center gap-1 text-xs text-green-600 dark:text-green-400">
                  <Check className="h-3 w-3" /> Activo
                </span>
              ) : (
                <span className="flex items-center gap-1 text-xs text-muted-foreground">
                  <X className="h-3 w-3" /> Inactivo
                </span>
              )}
            </div>

            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7"
                  aria-label={`Acciones para ${user.fullName}`}
                >
                  <MoreHorizontal className="h-4 w-4" aria-hidden="true" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem onClick={() => onView(user)}>Ver perfil</DropdownMenuItem>
                <DropdownMenuItem onClick={() => onEdit(user)}>Editar datos</DropdownMenuItem>
                <DropdownMenuItem onClick={() => onResetPw(user)}>
                  Restablecer contraseña
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                {user.activo ? (
                  <DropdownMenuItem
                    className="text-destructive"
                    onClick={() => onToggleStatus(user)}
                  >
                    Desactivar usuario
                  </DropdownMenuItem>
                ) : (
                  <DropdownMenuItem onClick={() => onToggleStatus(user)}>
                    Activar usuario
                  </DropdownMenuItem>
                )}
                <DropdownMenuSeparator />
                <DropdownMenuItem className="text-destructive" onClick={() => onDelete(user)}>
                  Eliminar usuario
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}

// ── UsersPage ─────────────────────────────────────────────────────────────────

export function UsersPage() {
  const navigate = useNavigate()

  // Filtros locales para UI
  const [search, setSearch] = useState('')
  const [rolFilter, setRolFilter] = useState<UserRole | 'all'>('all')
  const [sucursalFilter, setSucursalFilter] = useState<string>('all')

  // Datos del servidor
  const { data, isLoading } = useUsuarios({
    pagina: 1,
    tamanoPagina: 50,
    sucursalId: sucursalFilter !== 'all' ? sucursalFilter : undefined,
  })
  const allUsers = useMemo(() => (data?.items ?? []).map(mapToDisplayUser), [data])

  const toggleEstado = useToggleEstadoUsuario()
  const eliminarUsuario = useEliminarUsuario()
  const restablecerContrasena = useRestablecerContrasena()

  // Estados de modales de acciones
  const [deleteTarget, setDeleteTarget] = useState<DisplayUser | null>(null)
  const [statusTarget, setStatusTarget] = useState<DisplayUser | null>(null)
  const [resetPwTarget, setResetPwTarget] = useState<DisplayUser | null>(null)

  // Panel de Empresas y Sucursales
  const { data: empresasData } = useEmpresas({ tamanoPagina: 100 })
  const empresas = empresasData?.items ?? []
  const toggleEmpresa = useToggleEmpresa()

  const [selectedEmpresaPanel, setSelectedEmpresaPanel] = useState<string>('')
  const empresaSeleccionadaId = selectedEmpresaPanel || empresas[0]?.id || ''

  const { data: sucursalesData } = useSucursales({
    empresaId: empresaSeleccionadaId || undefined,
    tamanoPagina: 100,
  })
  const sucursales = sucursalesData?.items ?? []
  const toggleSucursal = useToggleSucursal()

  const [toggleEmpresaTarget, setToggleEmpresaTarget] = useState<EmpresaResumenDto | null>(null)
  const [toggleSucursalTarget, setToggleSucursalTarget] = useState<SucursalResumenDto | null>(null)

  // Lista filtrada client-side (búsqueda y rol)
  const filtered = useMemo(() => {
    return allUsers.filter((u) => {
      const matchSearch =
        !search ||
        u.fullName.toLowerCase().includes(search.toLowerCase()) ||
        u.correo.toLowerCase().includes(search.toLowerCase())
      const matchRol = rolFilter === 'all' || u.rol === rolFilter
      return matchSearch && matchRol
    })
  }, [allUsers, search, rolFilter])

  // Conteos (total desde el servidor, resto desde items cargados)
  const counts = useMemo(
    () => ({
      total: data?.totalRegistros ?? 0,
      activos: allUsers.filter((u) => u.activo).length,
      admins: allUsers.filter((u) => u.rol === 'admin' || u.rol === 'superadmin').length,
      workers: allUsers.filter((u) => u.rol === 'trabajador' || u.rol === 'tecnico').length,
    }),
    [data, allUsers],
  )

  // ── Handlers ────────────────────────────────────────────────────────────────

  function handleDelete() {
    if (!deleteTarget) return
    eliminarUsuario.mutate(deleteTarget.id, {
      onSuccess: () => setDeleteTarget(null),
    })
  }

  function handleToggleStatus() {
    if (!statusTarget) return
    toggleEstado.mutate(
      { id: statusTarget.id, activar: !statusTarget.activo },
      { onSuccess: () => setStatusTarget(null) },
    )
  }

  function handleResetPw() {
    if (!resetPwTarget) return
    restablecerContrasena.mutate(resetPwTarget.correo, {
      onSuccess: () => setResetPwTarget(null),
    })
  }

  function handleToggleEmpresa() {
    if (!toggleEmpresaTarget) return
    toggleEmpresa.mutate(toggleEmpresaTarget.id, { onSuccess: () => setToggleEmpresaTarget(null) })
  }

  function handleToggleSucursal() {
    if (!toggleSucursalTarget) return
    toggleSucursal.mutate(toggleSucursalTarget.id, {
      onSuccess: () => setToggleSucursalTarget(null),
    })
  }

  // ── Render ──────────────────────────────────────────────────────────────────

  return (
    <div className="space-y-4 p-3 lg:p-4">
      {/* Page header */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-base font-semibold tracking-tight">Gestión de usuarios</h2>
          <p role="status" aria-live="polite" className="text-xs text-muted-foreground">
            {filtered.length} de {counts.total} usuarios
          </p>
        </div>
        <Button onClick={() => navigate(ROUTES.USERS_NEW)}>
          <Plus className="mr-2 h-4 w-4" />
          Nuevo usuario
        </Button>
      </div>

      {/* Stats row */}
      <div className="grid grid-cols-2 gap-2 sm:grid-cols-4">
        {[
          { label: 'Total', value: counts.total, color: '' },
          { label: 'Activos', value: counts.activos, color: 'text-green-600 dark:text-green-400' },
          {
            label: 'Administradores',
            value: counts.admins,
            color: 'text-blue-600 dark:text-blue-400',
          },
          {
            label: 'Trabajadores',
            value: counts.workers,
            color: 'text-orange-600 dark:text-orange-400',
          },
        ].map((stat) => (
          <Card key={stat.label}>
            <CardContent className="p-3">
              <p className="text-xs font-medium text-muted-foreground">{stat.label}</p>
              <p className={`text-2xl font-bold ${stat.color}`}>{stat.value}</p>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Filters */}
      <div className="flex flex-col gap-1.5 sm:flex-row">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Buscar por nombre, correo o sucursal..."
            className="h-8 pl-9 text-xs"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        <Select value={rolFilter} onValueChange={(v) => setRolFilter(v as UserRole | 'all')}>
          <SelectTrigger className="h-8 w-full text-xs sm:w-44">
            <SelectValue placeholder="Rol" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">Todos los roles</SelectItem>
            <SelectItem value="superadmin">SuperAdmin</SelectItem>
            <SelectItem value="admin">Administrador</SelectItem>
            <SelectItem value="supervisor">Supervisor</SelectItem>
            <SelectItem value="tecnico">Técnico</SelectItem>
            <SelectItem value="trabajador">Trabajador</SelectItem>
            <SelectItem value="usuario">Usuario</SelectItem>
          </SelectContent>
        </Select>
        <Select value={sucursalFilter} onValueChange={setSucursalFilter}>
          <SelectTrigger className="h-8 w-full text-xs sm:w-48">
            <SelectValue placeholder="Empresa" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">Todas las empresas</SelectItem>
            {/* TODO: Cargar empresas desde /empresas cuando el endpoint esté disponible */}
          </SelectContent>
        </Select>
      </div>

      {/* Users list */}
      {isLoading ? (
        <UserListSkeleton />
      ) : filtered.length === 0 ? (
        <Card>
          <CardContent className="flex flex-col items-center gap-2 p-3 py-10 pt-0 text-center">
            <Search className="h-8 w-8 text-muted-foreground/40" />
            <p className="text-sm text-muted-foreground">
              No se encontraron usuarios con esos filtros.
            </p>
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-2">
          {filtered.map((user) => (
            <UserRow
              key={user.id}
              user={user}
              onView={(u) => navigate(userDetailPath(u.id))}
              onEdit={(u) => navigate(userEditPath(u.id))}
              onDelete={setDeleteTarget}
              onToggleStatus={setStatusTarget}
              onResetPw={setResetPwTarget}
            />
          ))}
        </div>
      )}

      {/* Paneles de Empresas y Sucursales */}
      <div className="grid gap-3 pt-2 lg:grid-cols-2">
        {/* Panel Empresas */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Empresas
                </CardTitle>
                <CardDescription>Sedes registradas en el sistema</CardDescription>
              </div>
              <Button
                size="sm"
                variant="outline"
                className="h-7 gap-1.5 text-xs"
                onClick={() => navigate(ROUTES.EMPRESAS_NEW)}
              >
                <Plus className="h-3 w-3" />
                Nueva empresa
              </Button>
            </div>
          </CardHeader>
          <CardContent className="space-y-2 p-3 pt-0">
            {empresas.length === 0 ? (
              <p className="py-4 text-center text-xs text-muted-foreground">
                No hay empresas registradas.
              </p>
            ) : (
              empresas.map((empresa) => (
                <div
                  key={empresa.id}
                  className="flex items-center justify-between rounded-lg border p-3 text-sm"
                >
                  <div className="flex items-start gap-3">
                    <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-primary/10">
                      <Building2 className="h-4 w-4 text-primary" />
                    </div>
                    <div>
                      <p className="font-medium">{empresa.nombre}</p>
                      <p className="text-xs text-muted-foreground">
                        RUC: {empresa.ruc} · {empresa.totalSucursales} sucursal
                        {empresa.totalSucursales !== 1 ? 'es' : ''}
                      </p>
                    </div>
                  </div>
                  <div className="flex shrink-0 items-center gap-2">
                    <Badge
                      className={
                        empresa.activo
                          ? 'border-transparent bg-green-100 text-[10px] text-green-700 dark:bg-green-900/30 dark:text-green-400'
                          : 'border-transparent bg-gray-100 text-[10px] text-gray-500 dark:bg-gray-800 dark:text-gray-400'
                      }
                    >
                      {empresa.activo ? 'Activa' : 'Inactiva'}
                    </Badge>
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-7 w-7"
                          aria-label={`Acciones para ${empresa.nombre}`}
                        >
                          <MoreHorizontal className="h-3.5 w-3.5" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem onClick={() => navigate(empresaEditPath(empresa.id))}>
                          <Pencil className="mr-2 h-3.5 w-3.5" />
                          Editar
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem
                          className={
                            empresa.activo ? 'text-destructive focus:text-destructive' : ''
                          }
                          onClick={() => setToggleEmpresaTarget(empresa)}
                        >
                          <Power className="mr-2 h-3.5 w-3.5" />
                          {empresa.activo ? 'Desactivar' : 'Activar'}
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </div>
                </div>
              ))
            )}
          </CardContent>
        </Card>

        {/* Panel Sucursales */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Sucursales
                </CardTitle>
                <CardDescription>Sucursales por empresa</CardDescription>
              </div>
              <Button
                size="sm"
                variant="outline"
                className="h-7 gap-1.5 text-xs"
                onClick={() => navigate(ROUTES.SUCURSALES_NEW)}
              >
                <Plus className="h-3 w-3" />
                Nueva sucursal
              </Button>
            </div>
            {empresas.length > 0 && (
              <Select value={empresaSeleccionadaId} onValueChange={setSelectedEmpresaPanel}>
                <SelectTrigger className="mt-2 h-7 text-xs">
                  <SelectValue placeholder="Selecciona empresa" />
                </SelectTrigger>
                <SelectContent>
                  {empresas.map((e) => (
                    <SelectItem key={e.id} value={e.id}>
                      {e.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          </CardHeader>
          <CardContent className="space-y-2 p-3 pt-0">
            {sucursales.length === 0 ? (
              <p className="py-4 text-center text-xs text-muted-foreground">
                {empresaSeleccionadaId
                  ? 'No hay sucursales para esta empresa.'
                  : 'Selecciona una empresa para ver sus sucursales.'}
              </p>
            ) : (
              sucursales.map((sucursal) => (
                <div
                  key={sucursal.id}
                  className="flex items-center justify-between rounded-lg border p-3 text-sm"
                >
                  <div className="flex items-center gap-3">
                    <div className="flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-blue-500/10">
                      <MapPin className="h-3.5 w-3.5 text-blue-600 dark:text-blue-400" />
                    </div>
                    <div>
                      <div className="flex items-center gap-1.5">
                        <p className="font-medium">{sucursal.nombre}</p>
                        <span className="rounded bg-muted px-1 py-0.5 font-mono text-[10px] text-muted-foreground">
                          {sucursal.codigo}
                        </span>
                      </div>
                      <p className="text-xs text-muted-foreground">
                        {sucursal.ciudad} · {sucursal.totalUsuarios} usuario
                        {sucursal.totalUsuarios !== 1 ? 's' : ''}
                      </p>
                    </div>
                  </div>
                  <div className="flex shrink-0 items-center gap-2">
                    <Badge
                      className={
                        sucursal.activo
                          ? 'border-transparent bg-green-100 text-[10px] text-green-700 dark:bg-green-900/30 dark:text-green-400'
                          : 'border-transparent bg-gray-100 text-[10px] text-gray-500 dark:bg-gray-800 dark:text-gray-400'
                      }
                    >
                      {sucursal.activo ? 'Activa' : 'Inactiva'}
                    </Badge>
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-7 w-7"
                          aria-label={`Acciones para ${sucursal.nombre}`}
                        >
                          <MoreHorizontal className="h-3.5 w-3.5" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem onClick={() => navigate(sucursalEditPath(sucursal.id))}>
                          <Pencil className="mr-2 h-3.5 w-3.5" />
                          Editar
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem
                          className={
                            sucursal.activo ? 'text-destructive focus:text-destructive' : ''
                          }
                          onClick={() => setToggleSucursalTarget(sucursal)}
                        >
                          <Power className="mr-2 h-3.5 w-3.5" />
                          {sucursal.activo ? 'Desactivar' : 'Activar'}
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </div>
                </div>
              ))
            )}
          </CardContent>
        </Card>
      </div>

      {/* ── Modales de acciones destructivas ──────────────────────────────────── */}

      <ConfirmDialog
        open={!!deleteTarget}
        onOpenChange={(open) => {
          if (!open) setDeleteTarget(null)
        }}
        title="Eliminar usuario"
        description={
          deleteTarget
            ? `¿Confirmas que deseas eliminar a "${deleteTarget.fullName}"? Esta acción no se puede deshacer.`
            : undefined
        }
        confirmLabel="Eliminar"
        variant="destructive"
        loading={eliminarUsuario.isPending}
        onConfirm={handleDelete}
      />

      <ConfirmDialog
        open={!!statusTarget}
        onOpenChange={(open) => {
          if (!open) setStatusTarget(null)
        }}
        title={statusTarget?.activo ? 'Desactivar usuario' : 'Activar usuario'}
        description={
          statusTarget
            ? statusTarget.activo
              ? `¿Deseas desactivar a "${statusTarget.fullName}"? No podrá iniciar sesión mientras esté inactivo.`
              : `¿Deseas activar a "${statusTarget.fullName}"? Recuperará acceso al sistema.`
            : undefined
        }
        confirmLabel={statusTarget?.activo ? 'Desactivar' : 'Activar'}
        variant={statusTarget?.activo ? 'destructive' : 'default'}
        loading={toggleEstado.isPending}
        onConfirm={handleToggleStatus}
      />

      <ConfirmDialog
        open={!!resetPwTarget}
        onOpenChange={(open) => {
          if (!open) setResetPwTarget(null)
        }}
        title="Restablecer contraseña"
        description={
          resetPwTarget
            ? `Se enviará un enlace de restablecimiento al correo "${resetPwTarget.correo}".`
            : undefined
        }
        confirmLabel="Enviar enlace"
        loading={restablecerContrasena.isPending}
        onConfirm={handleResetPw}
      />

      <ConfirmDialog
        open={!!toggleEmpresaTarget}
        onOpenChange={(open) => {
          if (!open) setToggleEmpresaTarget(null)
        }}
        title={toggleEmpresaTarget?.activo ? 'Desactivar empresa' : 'Activar empresa'}
        description={
          toggleEmpresaTarget
            ? toggleEmpresaTarget.activo
              ? `¿Deseas desactivar la empresa "${toggleEmpresaTarget.nombre}"? Los usuarios de esta empresa no podrán acceder.`
              : `¿Deseas activar la empresa "${toggleEmpresaTarget.nombre}"?`
            : undefined
        }
        confirmLabel={toggleEmpresaTarget?.activo ? 'Desactivar' : 'Activar'}
        variant={toggleEmpresaTarget?.activo ? 'destructive' : 'default'}
        loading={toggleEmpresa.isPending}
        onConfirm={handleToggleEmpresa}
      />

      <ConfirmDialog
        open={!!toggleSucursalTarget}
        onOpenChange={(open) => {
          if (!open) setToggleSucursalTarget(null)
        }}
        title={toggleSucursalTarget?.activo ? 'Desactivar sucursal' : 'Activar sucursal'}
        description={
          toggleSucursalTarget
            ? toggleSucursalTarget.activo
              ? `¿Deseas desactivar la sucursal "${toggleSucursalTarget.nombre}"?`
              : `¿Deseas activar la sucursal "${toggleSucursalTarget.nombre}"?`
            : undefined
        }
        confirmLabel={toggleSucursalTarget?.activo ? 'Desactivar' : 'Activar'}
        variant={toggleSucursalTarget?.activo ? 'destructive' : 'default'}
        loading={toggleSucursal.isPending}
        onConfirm={handleToggleSucursal}
      />
    </div>
  )
}
