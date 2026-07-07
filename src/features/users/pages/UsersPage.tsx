import { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { Search, Plus, MoreHorizontal, Check, X } from 'lucide-react'
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
import { MOCK_USERS, MOCK_SUCURSALES, MOCK_AREAS } from '@mocks/data'
import type { MockUser } from '@mocks/data'
import type { UserRole } from '@types-app/index'
import { ROUTES, userDetailPath, userEditPath } from '@constants/index'

// ── Constantes de presentación ────────────────────────────────────────────────

const ROL_COLORS: Record<UserRole, string> = {
  superadmin:
    'bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400 border-transparent',
  admin: 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400 border-transparent',
  worker:
    'bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400 border-transparent',
  user: 'bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-300 border-transparent',
}

const ROL_LABELS: Record<UserRole, string> = {
  superadmin: 'SuperAdmin',
  admin: 'Administrador',
  worker: 'Trabajador',
  user: 'Usuario',
}

// ── UserRow ───────────────────────────────────────────────────────────────────

interface UserRowProps {
  user: MockUser
  onView: (user: MockUser) => void
  onEdit: (user: MockUser) => void
  onDelete: (user: MockUser) => void
  onToggleStatus: (user: MockUser) => void
  onResetPw: (user: MockUser) => void
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
                <Button variant="ghost" size="icon" className="h-7 w-7">
                  <MoreHorizontal className="h-4 w-4" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem onClick={() => onView(user)}>Ver perfil</DropdownMenuItem>
                <DropdownMenuItem onClick={() => onEdit(user)}>Editar datos</DropdownMenuItem>
                <DropdownMenuItem onClick={() => onResetPw(user)}>
                  Restablecer contrasena
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

  // Estado mutable de usuarios
  const [users, setUsers] = useState<MockUser[]>(MOCK_USERS)

  // Filtros
  const [search, setSearch] = useState('')
  const [rolFilter, setRolFilter] = useState<UserRole | 'all'>('all')
  const [sucursalFilter, setSucursalFilter] = useState<string>('all')

  // Estados de modales de acciones destructivas
  const [deleteTarget, setDeleteTarget] = useState<MockUser | null>(null)
  const [statusTarget, setStatusTarget] = useState<MockUser | null>(null)
  const [resetPwTarget, setResetPwTarget] = useState<MockUser | null>(null)

  // Lista filtrada (lee del estado local)
  const filtered = useMemo(() => {
    return users.filter((u) => {
      const matchSearch =
        !search ||
        u.fullName.toLowerCase().includes(search.toLowerCase()) ||
        u.correo.toLowerCase().includes(search.toLowerCase()) ||
        (u.area ?? '').toLowerCase().includes(search.toLowerCase())
      const matchRol = rolFilter === 'all' || u.rol === rolFilter
      const matchSucursal = sucursalFilter === 'all' || u.sucursalId === sucursalFilter
      return matchSearch && matchRol && matchSucursal
    })
  }, [users, search, rolFilter, sucursalFilter])

  // Conteos (leen del estado local)
  const counts = useMemo(
    () => ({
      total: users.length,
      activos: users.filter((u) => u.activo).length,
      admins: users.filter((u) => u.rol === 'admin' || u.rol === 'superadmin').length,
      workers: users.filter((u) => u.rol === 'worker').length,
    }),
    [users],
  )

  // ── Handlers de acciones ────────────────────────────────────────────────────

  function handleDelete() {
    if (!deleteTarget) return
    setUsers((prev) => prev.filter((u) => u.id !== deleteTarget.id))
    toast.success(`Usuario "${deleteTarget.fullName}" eliminado`)
    setDeleteTarget(null)
  }

  function handleToggleStatus() {
    if (!statusTarget) return
    const nuevoEstado = !statusTarget.activo
    setUsers((prev) =>
      prev.map((u) => (u.id === statusTarget.id ? { ...u, activo: nuevoEstado } : u)),
    )
    toast.success(
      `Usuario "${statusTarget.fullName}" ${nuevoEstado ? 'activado' : 'desactivado'} correctamente`,
    )
    setStatusTarget(null)
  }

  function handleResetPw() {
    if (!resetPwTarget) return
    toast.success(`Se envio el enlace de restablecimiento al correo "${resetPwTarget.correo}"`)
    setResetPwTarget(null)
  }

  // ── Render ──────────────────────────────────────────────────────────────────

  return (
    <div className="space-y-4 p-3 lg:p-4">
      {/* Page header */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-base font-semibold tracking-tight">Gestion de usuarios</h2>
          <p className="text-xs text-muted-foreground">
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
            <SelectItem value="worker">Trabajador</SelectItem>
            <SelectItem value="user">Usuario</SelectItem>
          </SelectContent>
        </Select>
        <Select value={sucursalFilter} onValueChange={setSucursalFilter}>
          <SelectTrigger className="h-8 w-full text-xs sm:w-48">
            <SelectValue placeholder="Empresa" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">Todas las empresas</SelectItem>
            {MOCK_SUCURSALES.map((s) => (
              <SelectItem key={s.id} value={s.id}>
                {s.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      {/* Users list */}
      {filtered.length === 0 ? (
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
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Empresas
            </CardTitle>
            <CardDescription>Gestionar sedes de la empresa</CardDescription>
          </CardHeader>
          <CardContent className="space-y-2 p-3 pt-0">
            {MOCK_SUCURSALES.map((s) => (
              <div
                key={s.id}
                className="flex items-center justify-between rounded-lg border p-3 text-sm"
              >
                <div>
                  <p className="font-medium">{s.name}</p>
                  <p className="text-xs text-muted-foreground">
                    {s.address} · {s.ciudad}
                  </p>
                </div>
                <Badge variant={s.activo ? 'default' : 'secondary'} className="text-[10px]">
                  {s.activo ? 'Activa' : 'Inactiva'}
                </Badge>
              </div>
            ))}
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Sucursales — Empresa Principal
            </CardTitle>
            <CardDescription>Gestionar departamentos</CardDescription>
          </CardHeader>
          <CardContent className="space-y-2 p-3 pt-0">
            {MOCK_AREAS.filter((a) => a.sucursalId === 's1' && a.activo).map((area) => {
              const count = users.filter((u) => u.areaId === area.id).length
              return (
                <div
                  key={area.id}
                  className="flex items-center justify-between rounded-lg border p-3 text-sm"
                >
                  <p className="font-medium">{area.name}</p>
                  <span className="text-xs text-muted-foreground">{count} personas</span>
                </div>
              )
            })}
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
            ? `¿Confirmas que deseas eliminar a "${deleteTarget.fullName}"? Esta accion no se puede deshacer.`
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
              ? `¿Deseas desactivar a "${statusTarget.fullName}"? No podra iniciar sesion mientras este inactivo.`
              : `¿Deseas activar a "${statusTarget.fullName}"? Recuperara acceso al sistema.`
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
        title="Restablecer contrasena"
        description={
          resetPwTarget
            ? `Se enviara un enlace de restablecimiento al correo "${resetPwTarget.correo}".`
            : undefined
        }
        confirmLabel="Enviar enlace"
        onConfirm={handleResetPw}
      />
    </div>
  )
}
