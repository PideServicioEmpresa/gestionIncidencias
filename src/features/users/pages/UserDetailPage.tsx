import { useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { toast } from 'sonner'
import { ArrowLeft, Edit, History, Check, X } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Badge } from '@shared/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { EmptyState } from '@shared/components/EmptyState'
import { StatusBadge } from '@shared/components/StatusBadge'
import { PriorityBadge } from '@shared/components/PriorityBadge'
import { MOCK_USERS, MOCK_TICKETS } from '@mocks/data'
import type { MockUser } from '@mocks/data'
import { userEditPath } from '@constants/index'
import type { UserRole } from '@types-app/index'

// ── Constantes de presentacion ─────────────────────────────────────────────────

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

// ── UserDetailPage ─────────────────────────────────────────────────────────────

export function UserDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [showHistory, setShowHistory] = useState(false)
  const [userActivo, setUserActivo] = useState<boolean | null>(null)

  const baseUser = MOCK_USERS.find((u) => u.id === id)

  if (!baseUser) {
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

  // Estado de activo puede ser local (toggle) o el original
  const user: MockUser = {
    ...baseUser,
    activo: userActivo !== null ? userActivo : baseUser.activo,
  }

  // Ultimos 3 tickets asignados a este usuario
  const assignedTickets = MOCK_TICKETS.filter((t) => t.assignedTo?.id === user.id).slice(0, 3)

  function handleToggleActivo() {
    const nuevoEstado = !user.activo
    setUserActivo(nuevoEstado)
    toast.success(
      `Usuario "${user.fullName}" ${nuevoEstado ? 'activado' : 'desactivado'} correctamente`,
    )
  }

  function handleResetPassword() {
    toast.info('Se enviara un correo de restablecimiento')
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
                  {user.initials}
                </div>

                {/* Datos */}
                <div className="min-w-0 flex-1 space-y-2">
                  <div>
                    <p className="text-base font-semibold leading-tight">{user.fullName}</p>
                    <p className="text-xs text-muted-foreground">@{user.usuario}</p>
                  </div>

                  <div className="flex flex-wrap gap-1.5">
                    <Badge className={ROL_COLORS[user.rol]}>{ROL_LABELS[user.rol]}</Badge>
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
                      <span className="font-medium">{user.usuario}</span>
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
              </CardTitle>
            </CardHeader>
            <CardContent className="p-3 pt-0">
              {assignedTickets.length === 0 ? (
                <p className="text-xs text-muted-foreground">
                  Este usuario no tiene tickets asignados actualmente.
                </p>
              ) : (
                <div className="space-y-2">
                  {assignedTickets.map((ticket) => (
                    <div
                      key={ticket.id}
                      className="flex flex-col gap-1 rounded-lg border p-3 text-xs sm:flex-row sm:items-center sm:justify-between"
                    >
                      <div className="min-w-0">
                        <p className="font-medium text-muted-foreground">{ticket.code}</p>
                        <p className="truncate font-semibold">{ticket.title}</p>
                      </div>
                      <div className="flex shrink-0 flex-wrap gap-1.5">
                        <StatusBadge status={ticket.status} />
                        <PriorityBadge priority={ticket.priority} />
                      </div>
                    </div>
                  ))}
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
                      date: '01 Ene 2026',
                      desc: 'Usuario registrado en el sistema.',
                    },
                    {
                      label: 'Ultimo acceso',
                      date: baseUser.lastAccess
                        ? new Date(baseUser.lastAccess).toLocaleDateString('es-PE')
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
                <span className="font-medium">{user.sucursal}</span>
              </div>
              {user.area && (
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Sucursal</span>
                  <span className="font-medium">{user.area}</span>
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

              <Button className="w-full" variant="outline" size="sm" onClick={handleToggleActivo}>
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
