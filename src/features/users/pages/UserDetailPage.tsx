import { useNavigate, useParams } from 'react-router-dom'
import { toast } from 'sonner'
import { ArrowLeft, Edit, History, Check, X } from 'lucide-react'
import { useState } from 'react'
import { Button } from '@shared/ui/button'
import { Badge } from '@shared/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Skeleton } from '@shared/ui/skeleton'
import { EmptyState } from '@shared/components/EmptyState'
import { StatusBadge } from '@shared/components/StatusBadge'
import { useUsuario, useToggleEstadoUsuario } from '../hooks/useUsuarios'
import { useTickets } from '@features/tickets/hooks/useTickets'
import { userEditPath, ROUTES } from '@constants/index'
import type { UserRole, TicketStatus } from '@types-app/index'

// ── Constantes de presentación ─────────────────────────────────────────────────

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

// ── UserDetailPage ─────────────────────────────────────────────────────────────

export function UserDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [showHistory, setShowHistory] = useState(false)

  const { data: user, isLoading, isError } = useUsuario(id ?? '')
  const toggleEstado = useToggleEstadoUsuario()
  const ticketsQuery = useTickets(id ? { tecnicoId: id, tamanoPagina: 10 } : undefined)

  // ── Estados de carga y error ───────────────────────────────────────────────

  if (isLoading) {
    return (
      <div className="px-3 py-3 lg:px-5">
        <div className="mb-4 flex items-center gap-2">
          <Skeleton className="h-8 w-8 rounded" />
          <Skeleton className="h-5 w-40" />
        </div>
        <div className="grid gap-4 lg:grid-cols-[1fr_300px]">
          <div className="space-y-4">
            <Card>
              <CardContent className="p-3">
                <div className="flex items-start gap-4">
                  <Skeleton className="h-16 w-16 rounded-full" />
                  <div className="flex-1 space-y-2">
                    <Skeleton className="h-5 w-48" />
                    <Skeleton className="h-3 w-32" />
                    <Skeleton className="h-3 w-64" />
                  </div>
                </div>
              </CardContent>
            </Card>
            <Card>
              <CardContent className="p-3">
                <Skeleton className="h-3 w-32" />
                <div className="mt-3 space-y-2">
                  <Skeleton className="h-12 w-full rounded-lg" />
                  <Skeleton className="h-12 w-full rounded-lg" />
                </div>
              </CardContent>
            </Card>
          </div>
          <div className="space-y-4">
            <Card>
              <CardContent className="space-y-2 p-3">
                <Skeleton className="h-3 w-24" />
                <Skeleton className="h-3 w-full" />
                <Skeleton className="h-3 w-full" />
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    )
  }

  if (isError || !user) {
    return (
      <div className="px-3 py-3 lg:px-5">
        <EmptyState
          title="Usuario no encontrado"
          description="El usuario que buscas no existe o fue eliminado."
          action={
            <Button variant="outline" size="sm" onClick={() => navigate('/usuarios')}>
              <ArrowLeft className="mr-2 h-4 w-4" />
              Volver a usuarios
            </Button>
          }
        />
      </div>
    )
  }

  // Datos derivados del DTO
  const partes = user.nombreCompleto.trim().split(' ')
  const initials = partes
    .map((p) => p[0] ?? '')
    .join('')
    .slice(0, 2)
    .toUpperCase()
  const rol = user.rol.toLowerCase() as UserRole

  function handleToggleActivo() {
    if (!user) return
    toggleEstado.mutate({ id: user.id, activar: !user.activo })
  }

  function handleResetPassword() {
    // TODO: Implementar cuando exista el endpoint de restablecimiento de contraseña
    toast.info('Funcionalidad en implementación')
  }

  return (
    <div className="px-3 py-3 lg:px-5">
      {/* ── Header ─────────────────────────────────────────────────────────── */}
      <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-2">
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8"
            onClick={() => navigate('/usuarios')}
            aria-label="Volver"
          >
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <h2 className="text-base font-semibold tracking-tight">Detalle de Usuario</h2>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="outline" size="sm" onClick={() => setShowHistory((prev) => !prev)}>
            <History className="mr-2 h-4 w-4" />
            {showHistory ? 'Ocultar historial' : 'Historial'}
          </Button>
          <Button size="sm" onClick={() => navigate(userEditPath(user.id))}>
            <Edit className="mr-2 h-4 w-4" />
            Editar
          </Button>
        </div>
      </div>

      {/* ── Layout 2 columnas en desktop ───────────────────────────────────── */}
      <div className="grid gap-4 lg:grid-cols-[1fr_300px]">
        {/* ── Columna izquierda ─────────────────────────────────────────────── */}
        <div className="space-y-4">
          {/* Card Informacion personal */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                Informacion personal
              </CardTitle>
            </CardHeader>
            <CardContent className="p-3 pt-0">
              <div className="flex items-start gap-4">
                {/* Avatar grande */}
                <div className="flex h-16 w-16 shrink-0 items-center justify-center rounded-full bg-primary/20 text-lg font-semibold text-primary">
                  {initials}
                </div>

                {/* Datos */}
                <div className="min-w-0 flex-1 space-y-2">
                  <div>
                    <p className="text-base font-semibold leading-tight">{user.nombreCompleto}</p>
                    <p className="text-xs text-muted-foreground">@{user.nombreUsuario}</p>
                  </div>

                  <div className="flex flex-wrap gap-1.5">
                    <Badge className={ROL_COLORS[rol]}>{ROL_LABELS[rol]}</Badge>
                    {user.activo ? (
                      <Badge className="border-transparent bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400">
                        Activo
                      </Badge>
                    ) : (
                      <Badge variant="outline" className="text-muted-foreground">
                        Inactivo
                      </Badge>
                    )}
                  </div>

                  <div className="grid gap-1 text-xs">
                    <div className="flex items-center gap-2">
                      <span className="w-20 shrink-0 text-muted-foreground">Correo</span>
                      <span className="font-medium">{user.correo}</span>
                    </div>
                    {user.telefono && (
                      <div className="flex items-center gap-2">
                        <span className="w-20 shrink-0 text-muted-foreground">Telefono</span>
                        <span className="font-medium">{user.telefono}</span>
                      </div>
                    )}
                    <div className="flex items-center gap-2">
                      <span className="w-20 shrink-0 text-muted-foreground">Usuario</span>
                      <span className="font-medium">{user.nombreUsuario}</span>
                    </div>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Card Tickets asignados */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                Tickets asignados
                {ticketsQuery.data && ticketsQuery.data.totalRegistros > 0 && (
                  <span className="ml-2 font-mono text-[10px] normal-case text-muted-foreground/70">
                    ({ticketsQuery.data.totalRegistros})
                  </span>
                )}
              </CardTitle>
            </CardHeader>
            <CardContent className="p-3 pt-0">
              {ticketsQuery.isLoading ? (
                <div className="space-y-2">
                  <Skeleton className="h-10 w-full rounded-lg" />
                  <Skeleton className="h-10 w-full rounded-lg" />
                </div>
              ) : !ticketsQuery.data || ticketsQuery.data.totalRegistros === 0 ? (
                <p className="text-xs text-muted-foreground">
                  Este usuario no tiene tickets asignados actualmente.
                </p>
              ) : (
                <div className="space-y-2">
                  {ticketsQuery.data.items.map((t) => (
                    <button
                      key={t.id}
                      type="button"
                      onClick={() => navigate(`${ROUTES.TICKETS}/${t.id}`)}
                      className="flex w-full items-start gap-2 rounded-lg border px-3 py-2 text-left text-xs transition-colors hover:bg-muted/50"
                    >
                      <div className="min-w-0 flex-1">
                        <p className="truncate font-medium">{t.titulo}</p>
                        <p className="font-mono text-[10px] text-muted-foreground">{t.codigo}</p>
                      </div>
                      <StatusBadge status={t.estado.toLowerCase() as TicketStatus} />
                    </button>
                  ))}
                  {ticketsQuery.data.totalRegistros > 10 && (
                    <p className="text-center text-[10px] text-muted-foreground">
                      +{ticketsQuery.data.totalRegistros - 10} tickets más
                    </p>
                  )}
                </div>
              )}
            </CardContent>
          </Card>

          {/* Seccion historial (visible al pulsar el boton) */}
          {showHistory && (
            <Card>
              <CardHeader className="px-3 pb-2 pt-3">
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Historial de actividad
                </CardTitle>
              </CardHeader>
              <CardContent className="p-3 pt-0">
                <div className="space-y-3">
                  {[
                    {
                      label: 'Cuenta creada',
                      date: user.createdAt
                        ? new Date(user.createdAt).toLocaleDateString('es-PE')
                        : '—',
                      desc: 'Usuario registrado en el sistema.',
                    },
                    {
                      label: 'Ultimo acceso',
                      date: user.ultimoAcceso
                        ? new Date(user.ultimoAcceso).toLocaleDateString('es-PE')
                        : '—',
                      desc: 'Inicio de sesion exitoso.',
                    },
                    {
                      label: 'Estado actual',
                      date: '—',
                      desc: user.activo ? 'Cuenta activa.' : 'Cuenta inactiva.',
                    },
                  ].map((entry) => (
                    <div key={entry.label} className="flex gap-3 text-xs">
                      <div className="mt-0.5 h-2 w-2 shrink-0 rounded-full bg-primary/40" />
                      <div>
                        <p className="font-semibold">{entry.label}</p>
                        <p className="text-muted-foreground">
                          {entry.date} — {entry.desc}
                        </p>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          )}
        </div>

        {/* ── Columna derecha ───────────────────────────────────────────────── */}
        <div className="space-y-4">
          {/* Card Detalles */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                Detalles
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-2 p-3 pt-0 text-xs">
              <div className="flex justify-between">
                <span className="text-muted-foreground">Empresa</span>
                {/* TODO: Mostrar nombre de empresa cuando el endpoint /empresas/{id} esté disponible */}
                <span className="font-mono text-[10px] font-medium text-muted-foreground">
                  {user.empresaId}
                </span>
              </div>
              {user.areaId && (
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Sucursal</span>
                  {/* TODO: Mostrar nombre de sucursal cuando el endpoint esté disponible */}
                  <span className="font-mono text-[10px] font-medium text-muted-foreground">
                    {user.areaId}
                  </span>
                </div>
              )}
              <div className="flex justify-between">
                <span className="text-muted-foreground">ID</span>
                <span className="font-mono font-medium text-muted-foreground">{user.id}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">Estado</span>
                {user.activo ? (
                  <span className="flex items-center gap-1 font-medium text-green-600 dark:text-green-400">
                    <Check className="h-3 w-3" />
                    Activo
                  </span>
                ) : (
                  <span className="flex items-center gap-1 font-medium text-muted-foreground">
                    <X className="h-3 w-3" />
                    Inactivo
                  </span>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Card Acciones rapidas */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                Acciones rapidas
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-2 p-3 pt-0">
              <Button className="w-full" size="sm" onClick={() => navigate(userEditPath(user.id))}>
                <Edit className="mr-2 h-4 w-4" />
                Editar usuario
              </Button>

              <Button className="w-full" variant="outline" size="sm" onClick={handleResetPassword}>
                Restablecer contrasena
              </Button>

              <Button
                className="w-full"
                variant="outline"
                size="sm"
                disabled={toggleEstado.isPending}
                onClick={handleToggleActivo}
              >
                {user.activo ? (
                  <span className="text-destructive">Desactivar usuario</span>
                ) : (
                  'Activar usuario'
                )}
              </Button>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
