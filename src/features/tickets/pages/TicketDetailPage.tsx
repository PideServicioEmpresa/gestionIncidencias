import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import {
  ChevronLeft,
  MessageSquare,
  Paperclip,
  Send,
  Lock,
  FileText,
  Image,
  Video,
  CheckCircle2,
  UserCheck,
  AlertTriangle,
  RefreshCw,
  X,
  Settings2,
} from 'lucide-react'
import { toast } from 'sonner'
import { Button } from '@shared/ui/button'
import { Textarea } from '@shared/ui/textarea'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Badge } from '@shared/ui/badge'
import { Separator } from '@shared/ui/separator'
import { StatusBadge } from '@shared/components/StatusBadge'
import { PriorityBadge } from '@shared/components/PriorityBadge'
import { ConfirmDialog } from '@shared/components/ConfirmDialog'
import { EmptyState } from '@shared/components/EmptyState'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogClose,
  DialogDescription,
} from '@shared/ui/dialog'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { FormField } from '@shared/components/FormField'
import { getTicketById, MOCK_USERS } from '@mocks/data'
import { useAuthStore } from '@store/auth.store'
import { ROUTES } from '@constants/index'
import { cn } from '@lib/utils'
import type { MockComment, MockHistoryEntry, MockEvidencia, MockUser } from '@mocks/data'
import type { TicketStatus, TicketPriority } from '@types-app/index'

// ── Sub-components ────────────────────────────────────────────────────────────

function Avatar({ initials, size = 'sm' }: { initials: string; size?: 'sm' | 'md' }) {
  return (
    <div
      className={cn(
        'flex shrink-0 items-center justify-center rounded-full bg-primary/20 font-bold text-primary',
        size === 'sm' ? 'h-7 w-7 text-[10px]' : 'h-9 w-9 text-xs',
      )}
    >
      {initials}
    </div>
  )
}

function CommentBubble({ comment }: { comment: MockComment }) {
  const date = new Date(comment.createdAt).toLocaleString('es-PE', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
  })
  return (
    <div className={cn('flex gap-3', comment.isInternal && 'opacity-75')}>
      <Avatar initials={comment.author.initials} size="md" />
      <div className="min-w-0 flex-1">
        <div className="flex flex-wrap items-center gap-2">
          <span className="text-sm font-semibold">{comment.author.fullName}</span>
          <span className="text-xs text-muted-foreground">{date}</span>
          {comment.isInternal && (
            <Badge variant="outline" className="gap-1 text-[10px]">
              <Lock className="h-2.5 w-2.5" />
              Interno
            </Badge>
          )}
        </div>
        <p className="mt-1 whitespace-pre-wrap text-sm text-foreground/90">{comment.text}</p>
      </div>
    </div>
  )
}

function HistoryEntry({ entry }: { entry: MockHistoryEntry }) {
  const date = new Date(entry.createdAt).toLocaleString('es-PE', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
  })
  return (
    <div className="flex gap-3">
      <div className="flex flex-col items-center">
        <div className="flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-muted">
          <div className="h-2 w-2 rounded-full bg-primary/60" />
        </div>
        <div className="mt-1 flex-1 border-l border-dashed border-border" />
      </div>
      <div className="min-w-0 pb-3">
        <div className="flex flex-wrap items-center gap-2">
          <span className="text-xs font-semibold">{entry.action}</span>
          <span className="text-xs text-muted-foreground">por {entry.author.fullName}</span>
          <span className="text-xs text-muted-foreground">{date}</span>
        </div>
        {entry.fromStatus && entry.toStatus && (
          <div className="mt-1 flex items-center gap-1.5">
            <StatusBadge status={entry.fromStatus} />
            <span className="text-xs text-muted-foreground">→</span>
            <StatusBadge status={entry.toStatus} />
          </div>
        )}
        {entry.description && (
          <p className="mt-0.5 text-xs text-muted-foreground">{entry.description}</p>
        )}
      </div>
    </div>
  )
}

function EvidenciaItem({ ev }: { ev: MockEvidencia }) {
  const Icon = ev.type === 'imagen' ? Image : ev.type === 'pdf' ? FileText : Video
  return (
    <div className="flex items-center gap-3 rounded-lg border p-3 text-sm hover:bg-muted/50">
      <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-muted">
        <Icon className="h-4 w-4 text-muted-foreground" />
      </div>
      <div className="min-w-0 flex-1">
        <p className="truncate font-medium">{ev.name}</p>
        <p className="text-xs text-muted-foreground">
          {ev.size} · {ev.tipo === 'inicial' ? 'Evidencia inicial' : 'Evidencia de cierre'}
        </p>
      </div>
    </div>
  )
}

// ── Status labels ─────────────────────────────────────────────────────────────

const STATUS_OPTIONS: { value: TicketStatus; label: string }[] = [
  { value: 'sin_asignar', label: 'Sin asignar' },
  { value: 'asignado', label: 'Asignado' },
  { value: 'en_proceso', label: 'En proceso' },
  { value: 'pendiente_validacion', label: 'Pendiente de validación' },
  { value: 'cerrado', label: 'Cerrado' },
  { value: 'reabierto', label: 'Reabierto' },
]

// ── Comment counter (module-level so it survives re-renders within a session) ─
let _commentCounter = 100

// ── Main Page ─────────────────────────────────────────────────────────────────

type ActiveTab = 'comments' | 'history' | 'evidencias'

export function TicketDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const isAdmin = user?.rol === 'admin' || user?.rol === 'superadmin'

  const ticket = getTicketById(id ?? '')

  // ── Local state ──────────────────────────────────────────────────────────────
  const [activeTab, setActiveTab] = useState<ActiveTab>('comments')
  const [comment, setComment] = useState('')
  const [isInternal, setIsInternal] = useState(false)
  const [closeDialog, setCloseDialog] = useState(false)

  const [localStatus, setLocalStatus] = useState<TicketStatus>(ticket?.status ?? 'sin_asignar')
  const [localPriority] = useState<TicketPriority>(ticket?.priority ?? 'baja')
  const [localAssignedTo, setLocalAssignedTo] = useState<
    Pick<MockUser, 'id' | 'fullName' | 'initials'> | undefined
  >(ticket?.assignedTo)
  const [localComments, setLocalComments] = useState<MockComment[]>(ticket?.comments ?? [])
  const [localEvidencias, setLocalEvidencias] = useState<MockEvidencia[]>(ticket?.evidencias ?? [])
  const [localHistory, setLocalHistory] = useState<MockHistoryEntry[]>(ticket?.history ?? [])
  const [assignModal, setAssignModal] = useState(false)
  const [selectedWorker, setSelectedWorker] = useState('')
  const [changeStatusModal, setChangeStatusModal] = useState(false)
  const [pendingStatus, setPendingStatus] = useState<TicketStatus>(localStatus)

  // ── Workers list ─────────────────────────────────────────────────────────────
  const workers = MOCK_USERS.filter((u) => u.rol === 'worker' && u.activo)

  if (!ticket) {
    return (
      <div className="flex min-h-[60vh] flex-col items-center justify-center p-6">
        <EmptyState
          icon={AlertTriangle}
          title="Ticket no encontrado"
          description="El ticket que buscas no existe o no tienes acceso."
          action={<Button onClick={() => navigate(ROUTES.TICKETS)}>Volver a tickets</Button>}
        />
      </div>
    )
  }

  const tabs: { id: ActiveTab; label: string; count: number }[] = [
    { id: 'comments', label: 'Comentarios', count: localComments.length },
    { id: 'history', label: 'Historial', count: localHistory.length },
    { id: 'evidencias', label: 'Evidencias', count: localEvidencias.length },
  ]

  const createdDate = new Date(ticket.createdAt).toLocaleString('es-PE', {
    day: '2-digit',
    month: 'long',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })

  const updatedDate = new Date(ticket.updatedAt).toLocaleString('es-PE', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
  })

  // ── Handlers ─────────────────────────────────────────────────────────────────

  function handleAssignWorker() {
    const worker = workers.find((w) => w.id === selectedWorker)
    if (!worker) return
    setLocalAssignedTo({ id: worker.id, fullName: worker.fullName, initials: worker.initials })
    setLocalStatus('asignado')
    setAssignModal(false)
    setSelectedWorker('')
    const authorFullName = user ? `${user.nombre} ${user.apellido ?? ''}`.trim() : 'Sistema'
    const authorInitials = user ? `${user.nombre.charAt(0)}${user.apellido?.charAt(0) ?? ''}` : 'S'
    setLocalHistory((prev) => [
      ...prev,
      {
        id: 'h-' + String(Date.now()),
        action: 'Responsable asignado',
        description: `Asignado a ${worker.fullName}`,
        author: { id: user?.id ?? 'unknown', fullName: authorFullName, initials: authorInitials },
        createdAt: new Date().toISOString(),
      },
    ])
    toast.success(`Ticket asignado a ${worker.fullName}`)
  }

  function handleConfirmStatusChange() {
    const prevStatus = localStatus
    setLocalStatus(pendingStatus)
    setChangeStatusModal(false)
    const authorFullName = user ? `${user.nombre} ${user.apellido ?? ''}`.trim() : 'Sistema'
    const authorInitials = user ? `${user.nombre.charAt(0)}${user.apellido?.charAt(0) ?? ''}` : 'S'
    setLocalHistory((prev) => [
      ...prev,
      {
        id: 'h-' + String(Date.now()),
        action: 'Estado actualizado',
        fromStatus: prevStatus,
        toStatus: pendingStatus,
        description: '',
        author: { id: user?.id ?? 'unknown', fullName: authorFullName, initials: authorInitials },
        createdAt: new Date().toISOString(),
      },
    ])
    toast.success('Estado del ticket actualizado')
  }

  function handleSendComment() {
    if (!comment.trim()) return
    _commentCounter += 1
    const authorInitials = user ? `${user.nombre.charAt(0)}${user.apellido?.charAt(0) ?? ''}` : 'U'
    const newComment: MockComment = {
      id: 'c-' + String(_commentCounter),
      text: comment.trim(),
      author: {
        id: user?.id ?? 'unknown',
        fullName: user ? `${user.nombre} ${user.apellido ?? ''}`.trim() : 'Usuario',
        initials: authorInitials,
        rol: user?.rol ?? 'user',
      },
      createdAt: new Date().toISOString(),
      isInternal,
    }
    setLocalComments((prev) => [...prev, newComment])
    setLocalHistory((prev) => [
      ...prev,
      {
        id: 'h-' + String(Date.now()),
        action: 'Comentario agregado',
        description: comment.trim().slice(0, 80),
        author: {
          id: user?.id ?? 'unknown',
          fullName: user ? `${user.nombre} ${user.apellido ?? ''}`.trim() : 'Usuario',
          initials: authorInitials,
        },
        createdAt: new Date().toISOString(),
      },
    ])
    setComment('')
    setIsInternal(false)
    toast.success('Comentario agregado')
  }

  function handleAttachEvidence() {
    const simulatedNames = [
      'captura-evidencia.png',
      'foto-adjunta.jpg',
      'documento-soporte.pdf',
      'video-registro.mp4',
      'reporte-adicional.pdf',
    ]
    const simulatedTypes: MockEvidencia['type'][] = ['imagen', 'imagen', 'pdf', 'video', 'pdf']
    const idx = localEvidencias.length % simulatedNames.length
    const newEv: MockEvidencia = {
      id: 'ev-' + String(Date.now()),
      name: simulatedNames[idx],
      type: simulatedTypes[idx],
      size: `${(Math.random() * 3 + 0.5).toFixed(1)} MB`,
      tipo:
        localStatus === 'pendiente_validacion' || localStatus === 'cerrado' ? 'final' : 'inicial',
    }
    setLocalEvidencias((prev) => [...prev, newEv])
    toast.success(`Archivo "${newEv.name}" adjuntado`)
  }

  function handleCloseTicket() {
    setLocalStatus('cerrado')
    setCloseDialog(false)
    toast.success('Ticket cerrado correctamente')
  }

  function handleReopenTicket() {
    setLocalStatus('reabierto')
    toast.success('Ticket reabierto')
  }

  function handleCancelTicket() {
    toast.info('Ticket cancelado')
    navigate(ROUTES.TICKETS)
  }

  // ── Render ────────────────────────────────────────────────────────────────────

  return (
    <>
      {/* ── Modal: Asignar trabajador ── */}
      <Dialog
        open={assignModal}
        onOpenChange={(open) => {
          setAssignModal(open)
          if (!open) setSelectedWorker('')
        }}
      >
        <DialogContent className="ps-glow-modal max-w-md">
          <DialogHeader>
            <DialogTitle className="text-base font-semibold">Asignar trabajador</DialogTitle>
          </DialogHeader>
          <div className="rounded-lg bg-muted p-3 text-xs">
            <p className="font-medium">{ticket.title}</p>
            <p className="text-muted-foreground">{ticket.code}</p>
          </div>
          <FormField label="Trabajador" required>
            <Select value={selectedWorker} onValueChange={setSelectedWorker}>
              <SelectTrigger>
                <SelectValue placeholder="Selecciona un trabajador" />
              </SelectTrigger>
              <SelectContent>
                {workers.map((w) => (
                  <SelectItem key={w.id} value={w.id}>
                    <div className="flex items-center gap-2">
                      <span className="flex h-5 w-5 items-center justify-center rounded-full bg-primary/20 text-[9px] font-semibold text-primary">
                        {w.initials}
                      </span>
                      <span>{w.fullName}</span>
                      <Badge variant="outline" className="ml-auto text-[9px]">
                        {w.sucursal}
                      </Badge>
                    </div>
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </FormField>
          <DialogFooter>
            <DialogClose asChild>
              <Button variant="outline" size="sm" onClick={() => setSelectedWorker('')}>
                Cancelar
              </Button>
            </DialogClose>
            <Button size="sm" disabled={!selectedWorker} onClick={handleAssignWorker}>
              Asignar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* ── Modal: Cambiar estado ── */}
      <Dialog open={changeStatusModal} onOpenChange={setChangeStatusModal}>
        <DialogContent className="max-w-xs">
          <DialogHeader>
            <DialogTitle>Cambiar estado</DialogTitle>
            <DialogDescription>Selecciona el nuevo estado para este ticket.</DialogDescription>
          </DialogHeader>
          <div className="space-y-1 py-1">
            {STATUS_OPTIONS.map((opt) => (
              <button
                key={opt.value}
                type="button"
                onClick={() => setPendingStatus(opt.value)}
                className={cn(
                  'flex w-full items-center gap-2 rounded-lg px-3 py-2 text-left text-sm transition-colors',
                  pendingStatus === opt.value
                    ? 'bg-primary/10 font-semibold text-primary'
                    : 'hover:bg-muted',
                )}
              >
                <StatusBadge status={opt.value} />
              </button>
            ))}
          </div>
          <DialogFooter className="gap-2">
            <DialogClose asChild>
              <Button variant="outline" size="sm">
                Cancelar
              </Button>
            </DialogClose>
            <Button size="sm" onClick={handleConfirmStatusChange}>
              Confirmar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* ── Main content ── */}
      <div className="space-y-4 p-3 lg:px-6 lg:py-5">
        {/* Header */}
        <div className="flex items-start gap-3">
          <Button
            variant="ghost"
            size="icon"
            className="mt-0.5 h-8 w-8 shrink-0"
            onClick={() => navigate(-1)}
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <div className="min-w-0 flex-1">
            <div className="flex flex-wrap items-center gap-2">
              <span className="font-mono text-xs font-semibold text-muted-foreground">
                {ticket.code}
              </span>
              <PriorityBadge priority={localPriority} />
              <StatusBadge status={localStatus} />
            </div>
            <h2 className="mt-1 text-base font-semibold leading-snug">{ticket.title}</h2>
            <p className="mt-0.5 text-xs text-muted-foreground">
              Creado el {createdDate} por{' '}
              <span className="font-medium">{ticket.createdBy.fullName}</span> · Última
              actualización: {updatedDate}
            </p>
          </div>
        </div>

        <div className="grid gap-4 lg:grid-cols-[1fr_320px]">
          {/* Left column */}
          <div className="space-y-4">
            {/* Description */}
            <Card>
              <CardHeader className="px-3 pb-2 pt-3">
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Descripción
                </CardTitle>
              </CardHeader>
              <CardContent className="p-3 pt-0">
                <p className="whitespace-pre-wrap break-words text-sm leading-relaxed text-foreground/90">
                  {ticket.description}
                </p>
              </CardContent>
            </Card>

            {/* Tabs */}
            <Card>
              {/* Tab bar */}
              <div className="flex border-b">
                {tabs.map((tab) => (
                  <button
                    key={tab.id}
                    onClick={() => setActiveTab(tab.id)}
                    className={cn(
                      'flex flex-1 items-center justify-center gap-1 px-2 py-2.5 text-xs font-medium transition-colors',
                      activeTab === tab.id
                        ? 'border-b-2 border-primary text-primary'
                        : 'text-muted-foreground hover:text-foreground',
                    )}
                  >
                    {tab.label}
                    {tab.count > 0 && (
                      <Badge variant="secondary" className="h-4 px-1.5 text-[10px]">
                        {tab.count}
                      </Badge>
                    )}
                  </button>
                ))}
              </div>

              <CardContent className="p-3">
                {/* Comments tab */}
                {activeTab === 'comments' && (
                  <div className="space-y-3">
                    {localComments.length === 0 ? (
                      <div className="py-6 text-center">
                        <MessageSquare className="mx-auto h-8 w-8 text-muted-foreground/40" />
                        <p className="mt-2 text-sm text-muted-foreground">
                          Aún no hay comentarios.
                        </p>
                      </div>
                    ) : (
                      <div className="space-y-3">
                        {localComments.map((c) => (
                          <CommentBubble key={c.id} comment={c} />
                        ))}
                      </div>
                    )}

                    <Separator />

                    {/* New comment */}
                    <div className="space-y-1.5">
                      <div className="flex items-center gap-2">
                        {user && (
                          <Avatar
                            initials={`${user.nombre.charAt(0)}${user.apellido?.charAt(0) ?? ''}`}
                            size="md"
                          />
                        )}
                        <div className="flex-1">
                          <Textarea
                            placeholder="Escribe un comentario..."
                            aria-label="Escribe un comentario"
                            rows={2}
                            className="resize-none text-sm"
                            value={comment}
                            onChange={(e) => setComment(e.target.value)}
                          />
                        </div>
                      </div>
                      <div className="flex items-center justify-between">
                        {isAdmin && (
                          <button
                            type="button"
                            onClick={() => setIsInternal((v) => !v)}
                            className={cn(
                              'flex items-center gap-1.5 text-xs transition-colors',
                              isInternal
                                ? 'text-primary'
                                : 'text-muted-foreground hover:text-foreground',
                            )}
                          >
                            <Lock className="h-3.5 w-3.5" />
                            {isInternal ? 'Comentario interno' : 'Marcar como interno'}
                          </button>
                        )}
                        <div className="ml-auto">
                          <Button size="sm" disabled={!comment.trim()} onClick={handleSendComment}>
                            <Send className="mr-1.5 h-3.5 w-3.5" />
                            Comentar
                          </Button>
                        </div>
                      </div>
                    </div>
                  </div>
                )}

                {/* History tab */}
                {activeTab === 'history' && (
                  <div className="space-y-1">
                    {localHistory.length === 0 ? (
                      <div className="py-6 text-center">
                        <p className="text-sm text-muted-foreground">Sin historial aún.</p>
                      </div>
                    ) : (
                      localHistory.map((entry) => <HistoryEntry key={entry.id} entry={entry} />)
                    )}
                  </div>
                )}

                {/* Evidencias tab */}
                {activeTab === 'evidencias' && (
                  <div className="space-y-2">
                    {localEvidencias.length === 0 ? (
                      <div className="py-6 text-center">
                        <Paperclip className="mx-auto h-8 w-8 text-muted-foreground/40" />
                        <p className="mt-2 text-sm text-muted-foreground">
                          No hay evidencias adjuntas.
                        </p>
                      </div>
                    ) : (
                      localEvidencias.map((ev) => <EvidenciaItem key={ev.id} ev={ev} />)
                    )}
                    <Button
                      variant="outline"
                      size="sm"
                      className="mt-1 w-full"
                      onClick={handleAttachEvidence}
                    >
                      <Paperclip className="mr-1.5 h-3.5 w-3.5" />
                      Adjuntar archivo
                    </Button>
                  </div>
                )}
              </CardContent>
            </Card>
          </div>

          {/* Right column — metadata + actions */}
          <div className="space-y-4">
            {/* Info card */}
            <Card>
              <CardHeader className="px-3 pb-2 pt-3">
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Información
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-3 p-3 pt-0 text-sm">
                {[
                  { label: 'Tipo de Servicio', value: ticket.type },
                  { label: 'Empresa', value: ticket.sucursal },
                  { label: 'Sucursal', value: ticket.area },
                  ...(ticket.location ? [{ label: 'Ubicación', value: ticket.location }] : []),
                  {
                    label: 'Asignado a',
                    value: localAssignedTo?.fullName ?? 'Sin asignar',
                  },
                ].map((row) => (
                  <div key={row.label} className="flex flex-col gap-0.5">
                    <span className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
                      {row.label}
                    </span>
                    <span className="font-medium">{row.value}</span>
                  </div>
                ))}
              </CardContent>
            </Card>

            {/* Actions */}
            <Card>
              <CardHeader className="px-3 pb-2 pt-3">
                <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                  Acciones
                </CardTitle>
              </CardHeader>
              <CardContent className="flex flex-col gap-2 p-3 pt-0">
                {/* Confirmar y cerrar */}
                {localStatus === 'pendiente_validacion' && (
                  <Button className="w-full" onClick={() => setCloseDialog(true)}>
                    <CheckCircle2 className="mr-2 h-4 w-4" />
                    Confirmar y cerrar
                  </Button>
                )}

                {/* Asignar trabajador */}
                {isAdmin && localStatus === 'sin_asignar' && (
                  <Button className="w-full" onClick={() => setAssignModal(true)}>
                    <UserCheck className="mr-2 h-4 w-4" />
                    Asignar trabajador
                  </Button>
                )}

                {/* Cambiar estado (solo admin) */}
                {isAdmin && (
                  <Button
                    variant="outline"
                    className="w-full"
                    onClick={() => {
                      setPendingStatus(localStatus)
                      setChangeStatusModal(true)
                    }}
                  >
                    <Settings2 className="mr-2 h-4 w-4" />
                    Cambiar estado
                  </Button>
                )}

                {/* Reabrir ticket */}
                {localStatus === 'cerrado' && (
                  <Button variant="outline" className="w-full" onClick={handleReopenTicket}>
                    <RefreshCw className="mr-2 h-4 w-4" />
                    Reabrir ticket
                  </Button>
                )}

                {/* Cancelar ticket */}
                {localStatus !== 'cerrado' && (
                  <Button
                    variant="outline"
                    className="w-full text-destructive hover:text-destructive"
                    onClick={handleCancelTicket}
                  >
                    <X className="mr-2 h-4 w-4" />
                    Cancelar ticket
                  </Button>
                )}
              </CardContent>
            </Card>
          </div>
        </div>

        {/* Confirm close dialog */}
        <ConfirmDialog
          open={closeDialog}
          onOpenChange={setCloseDialog}
          title="Confirmar cierre de ticket"
          description={`¿Confirmas que el problema reportado en "${ticket.title}" fue resuelto correctamente? Esta acción cerrará el ticket.`}
          confirmLabel="Sí, cerrar ticket"
          variant="default"
          onConfirm={handleCloseTicket}
        />
      </div>
    </>
  )
}
