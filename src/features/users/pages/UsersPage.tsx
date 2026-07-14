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
import { toast } from 'sonner'
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
import { useUsuarios, useToggleEstadoUsuario } from '../hooks/useUsuarios'
import type { UsuarioResumenDto } from '../services/usuarioService'
import type { UserRole } from '@types-app/index'
import { ROUTES, userDetailPath, userEditPath } from '@constants/index'

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

// Sucursales/areas sin API aún — estructura mínima para el panel de admin
interface LocalSucursal {
  id: string
  name: string
  address: string
  ciudad: string
  activo: boolean
}

interface LocalArea {
  id: string
  name: string
  sucursalId: string
  activo: boolean
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

  // Mutación de activar/desactivar
  const toggleEstado = useToggleEstadoUsuario()

  // Estados de modales de acciones
  const [deleteTarget, setDeleteTarget] = useState<DisplayUser | null>(null)
  const [statusTarget, setStatusTarget] = useState<DisplayUser | null>(null)
  const [resetPwTarget, setResetPwTarget] = useState<DisplayUser | null>(null)

  // Panel de Empresas/Sucursales (sin API por ahora)
  // TODO: Conectar con /empresas y /sucursales cuando los endpoints estén disponibles
  const [localSucursales] = useState<LocalSucursal[]>([])
  const [localAreas] = useState<LocalArea[]>([])
  const [selectedEmpresaFilter, setSelectedEmpresaFilter] = useState<string>('s1')
  const [toggleEmpresaTarget, setToggleEmpresaTarget] = useState<LocalSucursal | null>(null)
  const [toggleAreaTarget, setToggleAreaTarget] = useState<LocalArea | null>(null)

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
    // TODO: Implementar cuando exista el endpoint DELETE /usuarios/{id} (baja lógica)
    toast.info('Funcionalidad en implementación')
    setDeleteTarget(null)
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
    // TODO: Implementar cuando exista el endpoint de restablecimiento de contraseña
    toast.info('Funcionalidad en implementación')
    setResetPwTarget(null)
  }

  function handleToggleEmpresa() {
    if (!toggleEmpresaTarget) return
    const nuevoEstado = !toggleEmpresaTarget.activo
    toast.success(
      `Empresa "${toggleEmpresaTarget.name}" ${nuevoEstado ? 'activada' : 'desactivada'} correctamente`,
    )
    setToggleEmpresaTarget(null)
  }

  function handleToggleArea() {
    if (!toggleAreaTarget) return
    const nuevoEstado = !toggleAreaTarget.activo
    toast.success(
      `Sucursal "${toggleAreaTarget.name}" ${nuevoEstado ? 'activada' : 'desactivada'} correctamente`,
    )
    setToggleAreaTarget(null)
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

      {/* Sucursales y areas — admin view */}
      <div className="grid gap-3 pt-2 lg:grid-cols-2">
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
                onClick={() => toast.info('Módulo de creación de empresas en desarrollo')}
              >
                <Plus className="h-3 w-3" />
                Nueva empresa
              </Button>
            </div>
          </CardHeader>
          <CardContent className="space-y-2 p-3 pt-0">
            {localSucursales.length === 0 ? (
              <p className="py-4 text-center text-xs text-muted-foreground">
                No hay empresas registradas.
              </p>
            ) : (
              localSucursales.map((s) => (
                <div
                  key={s.id}
                  className="flex items-center justify-between rounded-lg border p-3 text-sm"
                >
                  <div className="flex items-start gap-3">
                    <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-primary/10">
                      <Building2 className="h-4 w-4 text-primary" />
                    </div>
                    <div>
                      <p className="font-medium">{s.name}</p>
                      <p className="text-xs text-muted-foreground">
                        {s.address} · {s.ciudad}
                      </p>
                    </div>
                  </div>
                  <div className="flex shrink-0 items-center gap-2">
                    <Badge variant={s.activo ? 'default' : 'secondary'} className="text-[10px]">
                      {s.activo ? 'Activa' : 'Inactiva'}
                    </Badge>
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-7 w-7"
                          aria-label={`Acciones para empresa ${s.name}`}
                        >
                          <MoreHorizontal className="h-3.5 w-3.5" aria-hidden="true" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem
                          onClick={() => toast.info(`Editar empresa "${s.name}" — en desarrollo`)}
                        >
                          <Pencil className="mr-2 h-3.5 w-3.5" />
                          Editar
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem
                          className={s.activo ? 'text-destructive' : ''}
                          onClick={() => setToggleEmpresaTarget(s)}
                        >
                          <Power className="mr-2 h-3.5 w-3.5" />
                          {s.activo ? 'Desactivar' : 'Activar'}
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </div>
                </div>
              ))
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Sucursales
                </CardTitle>
                <CardDescription>Departamentos por empresa</CardDescription>
              </div>
              <Button
                size="sm"
                variant="outline"
                className="h-7 gap-1.5 text-xs"
                onClick={() => toast.info('Módulo de creación de sucursales en desarrollo')}
              >
                <Plus className="h-3 w-3" />
                Nueva sucursal
              </Button>
            </div>
            {/* Selector de empresa */}
            <Select value={selectedEmpresaFilter} onValueChange={setSelectedEmpresaFilter}>
              <SelectTrigger className="mt-2 h-7 text-xs">
                <SelectValue placeholder="Selecciona empresa" />
              </SelectTrigger>
              <SelectContent>
                {localSucursales.map((s) => (
                  <SelectItem key={s.id} value={s.id}>
                    {s.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </CardHeader>
          <CardContent className="space-y-2 p-3 pt-0">
            {localAreas
              .filter((a) => a.sucursalId === selectedEmpresaFilter)
              .map((area) => {
                return (
                  <div
                    key={area.id}
                    className="flex items-center justify-between rounded-lg border p-3 text-sm"
                  >
                    <div className="flex items-center gap-3">
                      <div className="flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-muted">
                        <MapPin className="h-3.5 w-3.5 text-muted-foreground" />
                      </div>
                      <div>
                        <p className="font-medium">{area.name}</p>
                        <p className="text-xs text-muted-foreground">0 personas</p>
                      </div>
                    </div>
                    <div className="flex shrink-0 items-center gap-2">
                      <Badge
                        variant={area.activo ? 'default' : 'secondary'}
                        className="text-[10px]"
                      >
                        {area.activo ? 'Activa' : 'Inactiva'}
                      </Badge>
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-7 w-7"
                            aria-label={`Acciones para sucursal ${area.name}`}
                          >
                            <MoreHorizontal className="h-3.5 w-3.5" aria-hidden="true" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          <DropdownMenuItem
                            onClick={() =>
                              toast.info(`Editar sucursal "${area.name}" — en desarrollo`)
                            }
                          >
                            <Pencil className="mr-2 h-3.5 w-3.5" />
                            Editar
                          </DropdownMenuItem>
                          <DropdownMenuSeparator />
                          <DropdownMenuItem
                            className={area.activo ? 'text-destructive' : ''}
                            onClick={() => setToggleAreaTarget(area)}
                          >
                            <Power className="mr-2 h-3.5 w-3.5" />
                            {area.activo ? 'Desactivar' : 'Activar'}
                          </DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </div>
                  </div>
                )
              })}
            {localAreas.filter((a) => a.sucursalId === selectedEmpresaFilter).length === 0 && (
              <p className="py-4 text-center text-xs text-muted-foreground">
                No hay sucursales registradas para esta empresa.
              </p>
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
              ? `¿Deseas desactivar la empresa "${toggleEmpresaTarget.name}"?`
              : `¿Deseas activar la empresa "${toggleEmpresaTarget.name}"?`
            : undefined
        }
        confirmLabel={toggleEmpresaTarget?.activo ? 'Desactivar' : 'Activar'}
        variant={toggleEmpresaTarget?.activo ? 'destructive' : 'default'}
        onConfirm={handleToggleEmpresa}
      />

      <ConfirmDialog
        open={!!toggleAreaTarget}
        onOpenChange={(open) => {
          if (!open) setToggleAreaTarget(null)
        }}
        title={toggleAreaTarget?.activo ? 'Desactivar sucursal' : 'Activar sucursal'}
        description={
          toggleAreaTarget
            ? toggleAreaTarget.activo
              ? `¿Deseas desactivar la sucursal "${toggleAreaTarget.name}"?`
              : `¿Deseas activar la sucursal "${toggleAreaTarget.name}"?`
            : undefined
        }
        confirmLabel={toggleAreaTarget?.activo ? 'Desactivar' : 'Activar'}
        variant={toggleAreaTarget?.activo ? 'destructive' : 'default'}
        onConfirm={handleToggleArea}
      />
    </div>
  )
}
