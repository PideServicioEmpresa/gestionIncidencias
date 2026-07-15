import { useState, useRef } from 'react'
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
  Check,
  Eye,
  Download,
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
import { TicketListSkeleton } from '@shared/components/PageSkeletons'
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
import { useAuthStore } from '@store/auth.store'
import { ROUTES } from '@constants/index'
import { cn } from '@lib/utils'
import { useTecnicos, useMotivosRechazo, useMotivosCancelacion } from '../hooks/useCatalogos'
import type { TicketStatus, TicketPriority } from '@types-app/index'
import type { ComentarioDto, EvidenciaDto, TicketHistorialDto } from '../services/ticketService'
import {
  useTicket,
  useComentarios,
  useEvidencias,
  useTicketHistorial,
  useAsignarTicket,
  useIniciarProceso,
  usePausar,
  useReanudar,
  useSubmitValidacion,
  useCerrarTicket,
  useReabrirTicket,
  useCancelarTicket,
  useCrearComentario,
  useSubirEvidencia,
} from '../hooks/useTickets'

// ── Normalizadores ────────────────────────────────────────────────────────────

function normalizeEstado(estado: string): TicketStatus {
  return estado.toLowerCase() as TicketStatus
}

function normalizePrioridad(prioridad: string): TicketPriority {
  return prioridad.toLowerCase() as TicketPriority
}

function formatFileSize(bytes: number): string {
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB'
  return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
}

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

function CommentBubble({ comment }: { comment: ComentarioDto }) {
  const date = new Date(comment.createdAt).toLocaleString('es-PE', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
  })
  const displayName = comment.autorNombre ?? comment.autorId.slice(0, 8) + '...'
  const initials = comment.autorNombre
    ? comment.autorNombre
        .split(' ')
        .map((p) => p[0] ?? '')
        .join('')
        .slice(0, 2)
        .toUpperCase()
    : comment.autorId.slice(0, 2).toUpperCase()
  return (
    <div className={cn('flex gap-3', comment.esInterno && 'opacity-75')}>
      <Avatar initials={initials} size="md" />
      <div className="min-w-0 flex-1">
        <div className="flex flex-wrap items-center gap-2">
          <span className="text-sm font-semibold">{displayName}</span>
          <span className="text-xs text-muted-foreground">{date}</span>
          {comment.esInterno && (
            <Badge variant="outline" className="gap-1 text-[10px]">
              <Lock className="h-2.5 w-2.5" />
              Interno
            </Badge>
          )}
        </div>
        <p className="mt-1 whitespace-pre-wrap text-sm text-foreground/90">{comment.cuerpo}</p>
      </div>
    </div>
  )
}

function HistoryEntry({ entry }: { entry: TicketHistorialDto }) {
  const date = new Date(entry.createdAt).toLocaleString('es-PE', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
  })
  const fromStatus = entry.estadoAnterior ? normalizeEstado(entry.estadoAnterior) : undefined
  const toStatus = entry.estadoNuevo ? normalizeEstado(entry.estadoNuevo) : undefined
  const actorLabel =
    entry.actorNombre ?? (entry.actorId ? entry.actorId.slice(0, 8) + '...' : 'Sistema')
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
          <span className="text-xs font-semibold">{entry.tipoEvento}</span>
          <span className="text-xs text-muted-foreground">por {actorLabel}</span>
          <span className="text-xs text-muted-foreground">{date}</span>
        </div>
        {fromStatus && toStatus && (
          <div className="mt-1 flex items-center gap-1.5">
            <StatusBadge status={fromStatus} />
            <span className="text-xs text-muted-foreground">→</span>
            <StatusBadge status={toStatus} />
          </div>
        )}
        {entry.comentarioTexto && (
          <p className="mt-0.5 text-xs text-muted-foreground">{entry.comentarioTexto}</p>
        )}
      </div>
    </div>
  )
}

// El backend guarda rutas relativas en urlAlmacenamiento (ej: "tickets/{id}/{guid}-file.jpg").
// La URL pública de Supabase Storage se construye aquí en el frontend.
function getEvidenciaUrl(ruta: string): string {
  const base = import.meta.env.VITE_SUPABASE_URL as string
  return `${base}/storage/v1/object/public/tickets-evidence/${ruta}`
}

async function descargarEvidencia(ruta: string, nombreOriginal: string) {
  try {
    const url = getEvidenciaUrl(ruta)
    const resp = await fetch(url)
    if (!resp.ok) throw new Error('Error al descargar')
    const blob = await resp.blob()
    const objectUrl = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = objectUrl
    a.download = nombreOriginal
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
    setTimeout(() => URL.revokeObjectURL(objectUrl), 1000)
  } catch {
    toast.error('No se pudo descargar el archivo')
  }
}

function EvidenciaItem({ ev }: { ev: EvidenciaDto }) {
  const [previewOpen, setPreviewOpen] = useState(false)
  const [thumbError, setThumbError] = useState(false)
  const isImage = ev.tipoMime.startsWith('image/')
  const isVideo = ev.tipoMime.startsWith('video/')
  const Icon = isImage ? Image : isVideo ? Video : FileText
  const tipoLabel = ev.tipo === 'INICIAL' ? 'Evidencia inicial' : 'Evidencia de cierre'
  const canPreview = isImage || isVideo
  const publicUrl = getEvidenciaUrl(ev.urlAlmacenamiento)

  return (
    <>
      <div className="flex items-start gap-3 rounded-lg border p-3 text-sm">
        {/* Thumbnail */}
        <div className="flex h-12 w-12 shrink-0 items-center justify-center overflow-hidden rounded-lg bg-muted">
          {isImage && !thumbError ? (
            <img
              src={publicUrl}
              alt={ev.nombreOriginal}
              className="h-full w-full object-cover"
              onError={() => setThumbError(true)}
            />
          ) : (
            <Icon className="h-5 w-5 text-muted-foreground" />
          )}
        </div>

        {/* Info + acciones apiladas para que no desborden en mobile */}
        <div className="min-w-0 flex-1">
          <p className="truncate text-xs font-medium">{ev.nombreOriginal}</p>
          <p className="text-xs text-muted-foreground">
            {formatFileSize(ev.tamanoBytes)} · {tipoLabel}
          </p>
          <div className="mt-2 flex items-center gap-1.5">
            {canPreview && (
              <Button
                variant="outline"
                size="sm"
                className="h-7 gap-1 px-2 text-xs"
                onClick={() => setPreviewOpen(true)}
              >
                <Eye className="h-3 w-3" />
                Visualizar
              </Button>
            )}
            <Button
              variant="ghost"
              size="sm"
              className="h-7 gap-1 px-2 text-xs"
              title="Descargar"
              onClick={() => descargarEvidencia(ev.urlAlmacenamiento, ev.nombreOriginal)}
            >
              <Download className="h-3 w-3" />
              Descargar
            </Button>
          </div>
        </div>
      </div>

      {/* Modal de previsualización */}
      <Dialog open={previewOpen} onOpenChange={setPreviewOpen}>
        <DialogContent className="flex max-h-[95dvh] w-[95vw] max-w-3xl flex-col p-0">
          <DialogHeader className="shrink-0 px-4 pb-2 pt-4">
            <DialogTitle className="truncate pr-6 text-sm font-medium">
              {ev.nombreOriginal}
            </DialogTitle>
            <DialogDescription className="text-xs">
              {formatFileSize(ev.tamanoBytes)} · {tipoLabel}
            </DialogDescription>
          </DialogHeader>

          <div className="flex min-h-[200px] flex-1 items-center justify-center overflow-auto bg-black/80">
            {isImage ? (
              <img
                src={publicUrl}
                alt={ev.nombreOriginal}
                className="max-h-[75dvh] w-auto max-w-full object-contain"
              />
            ) : isVideo ? (
              <video
                src={publicUrl}
                controls
                playsInline
                className="max-h-[75dvh] w-full"
                autoPlay={false}
              />
            ) : null}
          </div>

          <DialogFooter className="shrink-0 px-4 pb-4 pt-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => descargarEvidencia(ev.urlAlmacenamiento, ev.nombreOriginal)}
            >
              <Download className="mr-1.5 h-3.5 w-3.5" />
              Descargar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  )
}

// ── Transiciones válidas por estado (backend enforces state machine) ──────────

const STATUS_OPTIONS: { value: TicketStatus; label: string }[] = [
  { value: 'asignado', label: 'Asignado' },
  { value: 'en_proceso', label: 'En proceso' },
  { value: 'en_espera', label: 'En espera' },
  { value: 'pendiente_validacion', label: 'Pendiente de validación' },
  { value: 'cerrado', label: 'Cerrado' },
  { value: 'reabierto', label: 'Reabierto' },
]

// El backend aplica la máquina de estados — solo mostrar transiciones válidas
const VALID_NEXT_STATES: Partial<Record<TicketStatus, TicketStatus[]>> = {
  asignado: ['en_proceso'],
  en_proceso: ['en_espera', 'pendiente_validacion'],
  en_espera: ['asignado'],
  pendiente_validacion: ['cerrado'],
  reabierto: ['asignado', 'en_proceso'],
}

// ── Main Page ─────────────────────────────────────────────────────────────────

type ActiveTab = 'comments' | 'history' | 'evidencias'

export function TicketDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const isAdmin = user?.rol === 'admin' || user?.rol === 'superadmin'

  // ── Queries — todas antes de cualquier return condicional ─────────────────
  const ticketQuery = useTicket(id ?? '')
  const comentariosQuery = useComentarios(id ?? '')
  const historialQuery = useTicketHistorial(id ?? '')
  const evidenciasQuery = useEvidencias(id ?? '')

  // ── Mutations ─────────────────────────────────────────────────────────────
  const asignarTicket = useAsignarTicket()
  const iniciarProceso = useIniciarProceso()
  const pausar = usePausar()
  const reanudar = useReanudar()
  const submitValidacion = useSubmitValidacion()
  const cerrarTicket = useCerrarTicket()
  const reabrirTicket = useReabrirTicket()
  const cancelarTicket = useCancelarTicket()
  const crearComentario = useCrearComentario()
  const subirEvidencia = useSubirEvidencia()

  // ── Local state ───────────────────────────────────────────────────────────
  const [activeTab, setActiveTab] = useState<ActiveTab>('comments')
  const [comment, setComment] = useState('')
  const [isInternal, setIsInternal] = useState(false)
  const [closeDialog, setCloseDialog] = useState(false)
  const [assignModal, setAssignModal] = useState(false)
  const [selectedWorker, setSelectedWorker] = useState('')
  const [changeStatusModal, setChangeStatusModal] = useState(false)
  const [pendingStatus, setPendingStatus] = useState<TicketStatus>('sin_asignar')
  const [reabrirModal, setReopenModal] = useState(false)
  const [motivoRechazoId, setMotivoRechazoId] = useState('')
  const [comentarioRechazo, setComentarioRechazo] = useState('')
  const [cancelarModal, setCancelarModal] = useState(false)
  const [motivoCancelarTexto, setMotivoCancelarTexto] = useState('')

  const fileInputRef = useRef<HTMLInputElement>(null)

  const tecnicosQuery = useTecnicos(user?.empresaId ?? undefined)
  const workers = tecnicosQuery.data ?? []
  const motivosRechazoQuery = useMotivosRechazo()
  const motivosRechazo = motivosRechazoQuery.data ?? []
  const motivosCancelacionQuery = useMotivosCancelacion()
  const motivosCancelacion = motivosCancelacionQuery.data ?? []

  // ── Guards (después de todos los hooks) ──────────────────────────────────
  if (ticketQuery.isLoading) return <TicketListSkeleton />

  if (ticketQuery.error) {
    return (
      <div className="flex min-h-[60vh] flex-col items-center justify-center p-6">
        <EmptyState
          icon={AlertTriangle}
          title="Error al cargar el ticket"
          description={(ticketQuery.error as Error).message}
          action={<Button onClick={() => navigate(ROUTES.TICKETS)}>Volver a tickets</Button>}
        />
      </div>
    )
  }

  if (!ticketQuery.data) {
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

  const ticket = ticketQuery.data
  const currentStatus = normalizeEstado(ticket.estado)
  const currentPriority = normalizePrioridad(ticket.prioridadEfectiva)

  const comentarios = comentariosQuery.data ?? []
  const historial = historialQuery.data ?? []
  const evidencias = evidenciasQuery.data ?? []

  const tabs: { id: ActiveTab; label: string; count: number }[] = [
    { id: 'comments', label: 'Comentarios', count: comentarios.length },
    { id: 'history', label: 'Historial', count: historial.length },
    { id: 'evidencias', label: 'Evidencias', count: evidencias.length },
  ]

  const createdDate = new Date(ticket.fechaCreacion).toLocaleString('es-PE', {
    day: '2-digit',
    month: 'long',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })

  const updatedDate = ticket.fechaLimite
    ? new Date(ticket.fechaLimite).toLocaleString('es-PE', {
        day: '2-digit',
        month: 'short',
        hour: '2-digit',
        minute: '2-digit',
      })
    : '—'

  // ── Handlers ─────────────────────────────────────────────────────────────

  function handleAssignWorker() {
    if (!selectedWorker) return
    asignarTicket.mutate(
      { ticketId: ticket.id, tecnicoId: selectedWorker },
      {
        onSuccess: () => {
          setAssignModal(false)
          setSelectedWorker('')
        },
      },
    )
  }

  function handleConfirmStatusChange() {
    const onErr = (err: Error) => toast.error(err.message || 'No se pudo cambiar el estado.')
    const acciones: Partial<Record<TicketStatus, () => void>> = {
      en_proceso: () => iniciarProceso.mutate(ticket.id, { onError: onErr }),
      en_espera: () => pausar.mutate(ticket.id, { onError: onErr }),
      asignado: () => reanudar.mutate(ticket.id, { onError: onErr }),
      pendiente_validacion: () => submitValidacion.mutate(ticket.id, { onError: onErr }),
      cerrado: () => cerrarTicket.mutate({ ticketId: ticket.id }, { onError: onErr }),
    }
    const accion = acciones[pendingStatus]
    if (accion) {
      accion()
      setChangeStatusModal(false)
    } else {
      toast.error('Transición de estado no disponible desde esta vista.')
    }
  }

  function handleSendComment() {
    if (!comment.trim()) return
    crearComentario.mutate(
      { ticketId: ticket.id, cuerpo: comment.trim(), esInterno: isInternal },
      {
        onSuccess: () => {
          setComment('')
          setIsInternal(false)
        },
      },
    )
  }

  function handleFileSelected(e: React.ChangeEvent<HTMLInputElement>) {
    const archivo = e.target.files?.[0]
    if (!archivo) return
    const tipo: 'INICIAL' | 'FINAL' =
      currentStatus === 'pendiente_validacion' || currentStatus === 'cerrado' ? 'FINAL' : 'INICIAL'
    subirEvidencia.mutate({ ticketId: ticket.id, archivo, tipo })
    e.target.value = ''
  }

  function handleCloseTicket() {
    cerrarTicket.mutate(
      { ticketId: ticket.id },
      {
        onSuccess: () => setCloseDialog(false),
        onError: (err: Error) => toast.error(err.message || 'No se pudo cerrar el ticket.'),
      },
    )
  }

  function handleReopenTicket() {
    setMotivoRechazoId('')
    setComentarioRechazo('')
    setReopenModal(true)
  }

  function handleConfirmReabrir() {
    if (!motivoRechazoId) return
    reabrirTicket.mutate(
      { ticketId: ticket.id, motivoRechazoId, comentarioRechazo: comentarioRechazo || undefined },
      {
        onSuccess: () => {
          setReopenModal(false)
          setMotivoRechazoId('')
          setComentarioRechazo('')
        },
      },
    )
  }

  function handleOpenCancelar() {
    setMotivoCancelarTexto('')
    setCancelarModal(true)
  }

  function handleConfirmCancelar() {
    if (!motivoCancelarTexto.trim()) return
    // El backend requiere un ID de catálogo; usamos el motivo "Otro" o el primero disponible
    const motivoBase = motivosCancelacion.find((m) => m.esOtro) ?? motivosCancelacion[0]
    if (!motivoBase) {
      toast.error('No hay motivos de cancelación configurados en el sistema.')
      return
    }
    cancelarTicket.mutate(
      {
        ticketId: ticket.id,
        motivoCancelacionId: motivoBase.id,
        comentario: motivoCancelarTexto.trim(),
      },
      {
        onSuccess: () => {
          setCancelarModal(false)
          setMotivoCancelarTexto('')
        },
        onError: (err: Error) => {
          toast.error(err.message || 'No se pudo cancelar el ticket.')
        },
      },
    )
  }

  // ── Render ────────────────────────────────────────────────────────────────

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
            <p className="font-medium">{ticket.titulo}</p>
            <p className="text-muted-foreground">{ticket.codigo}</p>
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
                        {w.nombreCompleto
                          .split(' ')
                          .map((p) => p[0] ?? '')
                          .join('')
                          .slice(0, 2)
                          .toUpperCase()}
                      </span>
                      <span>{w.nombreCompleto}</span>
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
            <Button
              size="sm"
              disabled={!selectedWorker || asignarTicket.isPending}
              onClick={handleAssignWorker}
            >
              {asignarTicket.isPending ? 'Asignando...' : 'Asignar'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* ── Modal: Cambiar estado ── */}
      <Dialog open={changeStatusModal} onOpenChange={setChangeStatusModal}>
        <DialogContent className="ps-glow-modal max-w-sm">
          <DialogHeader>
            <DialogTitle className="text-base font-semibold">Cambiar estado</DialogTitle>
            <DialogDescription className="flex items-center gap-1.5 text-xs">
              Estado actual:
              <StatusBadge status={currentStatus} />
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-1.5">
            {STATUS_OPTIONS.filter((opt) =>
              (VALID_NEXT_STATES[currentStatus] ?? []).includes(opt.value),
            ).map((opt) => {
              const isSelected = pendingStatus === opt.value
              return (
                <button
                  key={opt.value}
                  type="button"
                  onClick={() => setPendingStatus(opt.value)}
                  className={cn(
                    'flex w-full items-center gap-3 rounded-lg border px-3 py-2.5 text-left transition-all',
                    isSelected
                      ? 'border-primary/30 bg-primary/10'
                      : 'border-transparent hover:bg-muted',
                  )}
                >
                  <StatusBadge status={opt.value} />
                  {isSelected && <Check className="ml-auto h-3.5 w-3.5 shrink-0 text-primary" />}
                </button>
              )
            })}
            {(VALID_NEXT_STATES[currentStatus] ?? []).length === 0 && (
              <p className="py-3 text-center text-xs text-muted-foreground">
                No hay transiciones disponibles desde el estado actual.
              </p>
            )}
          </div>
          <DialogFooter className="gap-2 pt-1">
            <DialogClose asChild>
              <Button variant="outline" size="sm">
                Cancelar
              </Button>
            </DialogClose>
            <Button
              size="sm"
              onClick={handleConfirmStatusChange}
              disabled={
                pendingStatus === currentStatus ||
                !(VALID_NEXT_STATES[currentStatus] ?? []).includes(pendingStatus)
              }
            >
              Confirmar cambio
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* ── Modal: Reabrir ticket ── */}
      <Dialog
        open={reabrirModal}
        onOpenChange={(open) => {
          setReopenModal(open)
          if (!open) {
            setMotivoRechazoId('')
            setComentarioRechazo('')
          }
        }}
      >
        <DialogContent className="ps-glow-modal max-w-md">
          <DialogHeader>
            <DialogTitle className="text-base font-semibold">Reabrir ticket</DialogTitle>
            <DialogDescription className="text-xs">
              Selecciona el motivo por el que se rechaza el cierre y se reabre el ticket.
            </DialogDescription>
          </DialogHeader>
          <FormField label="Motivo de rechazo" required>
            <Select
              value={motivoRechazoId}
              onValueChange={setMotivoRechazoId}
              disabled={motivosRechazoQuery.isLoading}
            >
              <SelectTrigger>
                <SelectValue
                  placeholder={motivosRechazoQuery.isLoading ? 'Cargando...' : 'Seleccionar motivo'}
                />
              </SelectTrigger>
              <SelectContent>
                {motivosRechazo.map((m) => (
                  <SelectItem key={m.id} value={m.id}>
                    {m.nombre}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </FormField>
          <FormField label="Comentario" optional>
            <Textarea
              rows={2}
              placeholder="Describe por qué se reabre el ticket..."
              className="resize-none text-xs"
              value={comentarioRechazo}
              onChange={(e) => setComentarioRechazo(e.target.value)}
            />
          </FormField>
          <DialogFooter>
            <DialogClose asChild>
              <Button variant="outline" size="sm">
                Cancelar
              </Button>
            </DialogClose>
            <Button
              size="sm"
              disabled={!motivoRechazoId || reabrirTicket.isPending}
              onClick={handleConfirmReabrir}
            >
              {reabrirTicket.isPending ? 'Reabriendo...' : 'Reabrir ticket'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* ── Modal: Cancelar ticket ── */}
      <Dialog
        open={cancelarModal}
        onOpenChange={(open) => {
          setCancelarModal(open)
          if (!open) setMotivoCancelarTexto('')
        }}
      >
        <DialogContent className="ps-glow-modal w-[calc(100%-2rem)] max-w-md rounded-2xl">
          <DialogHeader>
            <DialogTitle className="text-base font-semibold">Cancelar ticket</DialogTitle>
            <DialogDescription className="text-xs">
              Describe el motivo por el que se cancela el ticket. Esta acción retira el ticket del
              flujo de trabajo.
            </DialogDescription>
          </DialogHeader>
          <FormField label="Motivo de cancelación" required>
            <Textarea
              rows={3}
              placeholder="Describe el motivo de cancelación..."
              className="resize-none text-sm"
              value={motivoCancelarTexto}
              onChange={(e) => setMotivoCancelarTexto(e.target.value)}
            />
          </FormField>
          <div className="flex flex-col gap-2 sm:flex-row sm:justify-end">
            <DialogClose asChild>
              <Button variant="outline" className="w-full sm:w-auto">
                Volver
              </Button>
            </DialogClose>
            <Button
              variant="destructive"
              className="w-full sm:w-auto"
              disabled={!motivoCancelarTexto.trim() || cancelarTicket.isPending}
              onClick={handleConfirmCancelar}
            >
              {cancelarTicket.isPending ? 'Cancelando...' : 'Confirmar cancelación'}
            </Button>
          </div>
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
            aria-label="Volver a la lista de tickets"
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <div className="min-w-0 flex-1">
            <div className="flex flex-wrap items-center gap-2">
              <span className="font-mono text-xs font-semibold text-muted-foreground">
                {ticket.codigo}
              </span>
              <PriorityBadge priority={currentPriority} />
              <StatusBadge status={currentStatus} />
            </div>
            <h2 className="mt-1 text-base font-semibold leading-snug">{ticket.titulo}</h2>
            <p className="mt-0.5 text-xs text-muted-foreground">
              Creado el {createdDate}
              {updatedDate !== '—' && ` · Fecha límite: ${updatedDate}`}
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
                  {ticket.descripcion}
                </p>
              </CardContent>
            </Card>

            {/* Tabs */}
            <Card>
              {/* Tab bar — ARIA tablist para WCAG 4.1.2 */}
              <div
                role="tablist"
                aria-label="Secciones del ticket"
                className="flex overflow-x-auto border-b [scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
              >
                {tabs.map((tab) => (
                  <button
                    key={tab.id}
                    role="tab"
                    aria-selected={activeTab === tab.id}
                    aria-controls={`tabpanel-${tab.id}`}
                    id={`tab-${tab.id}`}
                    onClick={() => setActiveTab(tab.id)}
                    className={cn(
                      'flex flex-1 shrink-0 items-center justify-center gap-1 whitespace-nowrap px-3 py-2.5 text-xs font-medium transition-colors',
                      activeTab === tab.id
                        ? 'border-b-2 border-primary text-primary'
                        : 'text-muted-foreground hover:text-foreground',
                    )}
                  >
                    {tab.label}
                    {tab.count > 0 && (
                      <Badge
                        variant="secondary"
                        aria-hidden="true"
                        className="h-4 px-1.5 text-[10px]"
                      >
                        {tab.count}
                      </Badge>
                    )}
                  </button>
                ))}
              </div>

              <CardContent className="p-3">
                {/* Comments tab */}
                {activeTab === 'comments' && (
                  <div
                    role="tabpanel"
                    id="tabpanel-comments"
                    aria-labelledby="tab-comments"
                    className="space-y-3"
                  >
                    {comentariosQuery.isLoading ? (
                      <p className="py-6 text-center text-sm text-muted-foreground">
                        Cargando comentarios...
                      </p>
                    ) : comentarios.length === 0 ? (
                      <div className="py-6 text-center">
                        <MessageSquare className="mx-auto h-8 w-8 text-muted-foreground/40" />
                        <p className="mt-2 text-sm text-muted-foreground">
                          Aún no hay comentarios.
                        </p>
                      </div>
                    ) : (
                      <div className="space-y-3">
                        {comentarios.map((c) => (
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
                          <Button
                            size="sm"
                            disabled={!comment.trim() || crearComentario.isPending}
                            onClick={handleSendComment}
                          >
                            <Send className="mr-1.5 h-3.5 w-3.5" />
                            {crearComentario.isPending ? 'Enviando...' : 'Comentar'}
                          </Button>
                        </div>
                      </div>
                    </div>
                  </div>
                )}

                {/* History tab */}
                {activeTab === 'history' && (
                  <div
                    role="tabpanel"
                    id="tabpanel-history"
                    aria-labelledby="tab-history"
                    className="space-y-1"
                  >
                    {historialQuery.isLoading ? (
                      <p className="py-6 text-center text-sm text-muted-foreground">
                        Cargando historial...
                      </p>
                    ) : historial.length === 0 ? (
                      <div className="py-6 text-center">
                        <p className="text-sm text-muted-foreground">Sin historial aún.</p>
                      </div>
                    ) : (
                      historial.map((entry) => <HistoryEntry key={entry.id} entry={entry} />)
                    )}
                  </div>
                )}

                {/* Evidencias tab */}
                {activeTab === 'evidencias' && (
                  <div
                    role="tabpanel"
                    id="tabpanel-evidencias"
                    aria-labelledby="tab-evidencias"
                    className="space-y-2"
                  >
                    {evidenciasQuery.isLoading ? (
                      <p className="py-6 text-center text-sm text-muted-foreground">
                        Cargando evidencias...
                      </p>
                    ) : evidencias.length === 0 ? (
                      <div className="py-6 text-center">
                        <Paperclip className="mx-auto h-8 w-8 text-muted-foreground/40" />
                        <p className="mt-2 text-sm text-muted-foreground">
                          No hay evidencias adjuntas.
                        </p>
                      </div>
                    ) : (
                      evidencias.map((ev) => <EvidenciaItem key={ev.id} ev={ev} />)
                    )}
                    {/* Input de archivo oculto */}
                    <input
                      ref={fileInputRef}
                      type="file"
                      className="sr-only"
                      accept="image/*,video/*,.pdf,.doc,.docx,.xls,.xlsx"
                      onChange={handleFileSelected}
                    />
                    <Button
                      variant="outline"
                      size="sm"
                      className="mt-1 w-full"
                      disabled={subirEvidencia.isPending}
                      onClick={() => fileInputRef.current?.click()}
                    >
                      <Paperclip className="mr-1.5 h-3.5 w-3.5" />
                      {subirEvidencia.isPending ? 'Subiendo...' : 'Adjuntar archivo'}
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
                  {
                    label: 'Tipo de Servicio',
                    value: ticket.tipoServicioNombre ?? ticket.tipoServicioId,
                  },
                  { label: 'Sucursal', value: ticket.sucursalNombre ?? ticket.sucursalId },
                  { label: 'Área', value: ticket.areaNombre ?? ticket.areaId },
                  ...(ticket.ubicacion ? [{ label: 'Ubicación', value: ticket.ubicacion }] : []),
                  {
                    label: 'Asignado a',
                    value:
                      ticket.tecnicoNombre ??
                      (ticket.tecnicoId ? ticket.tecnicoId.slice(0, 8) + '...' : 'Sin asignar'),
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
                {currentStatus === 'pendiente_validacion' && (
                  <Button className="w-full" onClick={() => setCloseDialog(true)}>
                    <CheckCircle2 className="mr-2 h-4 w-4" />
                    Confirmar y cerrar
                  </Button>
                )}

                {/* Asignar trabajador */}
                {isAdmin && currentStatus === 'sin_asignar' && (
                  <Button className="w-full" onClick={() => setAssignModal(true)}>
                    <UserCheck className="mr-2 h-4 w-4" />
                    Asignar trabajador
                  </Button>
                )}

                {/* Cambiar estado (solo admin, solo cuando hay transiciones válidas) */}
                {isAdmin && (VALID_NEXT_STATES[currentStatus] ?? []).length > 0 && (
                  <Button
                    variant="outline"
                    className="w-full"
                    onClick={() => {
                      setPendingStatus((VALID_NEXT_STATES[currentStatus] ?? [])[0] ?? currentStatus)
                      setChangeStatusModal(true)
                    }}
                  >
                    <Settings2 className="mr-2 h-4 w-4" />
                    Cambiar estado
                  </Button>
                )}

                {/* Reabrir ticket */}
                {currentStatus === 'cerrado' && (
                  <Button variant="outline" className="w-full" onClick={handleReopenTicket}>
                    <RefreshCw className="mr-2 h-4 w-4" />
                    Reabrir ticket
                  </Button>
                )}

                {/* Cancelar ticket */}
                {currentStatus !== 'cerrado' && currentStatus !== 'cancelado' && (
                  <Button
                    variant="outline"
                    className="w-full text-destructive hover:text-destructive"
                    onClick={handleOpenCancelar}
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
          description={`¿Confirmas que el problema reportado en "${ticket.titulo}" fue resuelto correctamente? Esta acción cerrará el ticket.`}
          confirmLabel="Sí, cerrar ticket"
          variant="default"
          onConfirm={handleCloseTicket}
        />
      </div>
    </>
  )
}
