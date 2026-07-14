import { useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import {
  Upload,
  X,
  CheckCircle2,
  Loader2,
  MapPin,
  ChevronLeft,
  FileText,
  Paperclip,
} from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Textarea } from '@shared/ui/textarea'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { FormField } from '@shared/components/FormField'
import { useAuthStore } from '@store/auth.store'
import { ROUTES } from '@constants/index'
import { useCrearTicket } from '../hooks/useTickets'
import { useTiposServicio, useSucursales, useAreas, useCategorias } from '../hooks/useCatalogos'

// ── Schema ────────────────────────────────────────────────────────────────────

const createTicketSchema = z.object({
  title: z
    .string()
    .min(1, 'Ingresa un titulo para la solicitud.')
    .min(10, 'El titulo debe tener al menos 10 caracteres.')
    .max(150, 'Maximo 150 caracteres.'),
  type: z.string().min(1, 'Selecciona un tipo de servicio.'),
  categoriaId: z.string().min(1, 'Selecciona una categoría.'),
  priority: z.enum(['baja', 'media', 'alta', 'critica'], {
    errorMap: () => ({ message: 'Selecciona una prioridad.' }),
  }),
  sucursalId: z.string().min(1, 'Selecciona una empresa.'),
  areaId: z.string().min(1, 'Selecciona una sucursal.'),
  location: z.string().max(200, 'Máximo 200 caracteres').optional(),
  description: z
    .string()
    .min(1, 'Describe el problema que deseas reportar.')
    .min(20, 'Describe el problema con al menos 20 caracteres.')
    .max(5000, 'Maximo 5000 caracteres.'),
})

type CreateTicketForm = z.infer<typeof createTicketSchema>

// ── Constantes ────────────────────────────────────────────────────────────────

const PRIORITY_OPTIONS = [
  { value: 'baja', label: 'Baja', description: 'Sin impacto operativo urgente' },
  { value: 'media', label: 'Media', description: 'Afecta parcialmente las operaciones' },
  { value: 'alta', label: 'Alta', description: 'Impacto significativo en operaciones' },
  { value: 'critica', label: 'Crítica', description: 'Detiene completamente las operaciones' },
]

// ── Tipos de evidencias adjuntas ──────────────────────────────────────────────

interface AttachedFile {
  id: string
  file: File
  preview: string | null // objectURL para imágenes, null para otros
  name: string
  size: string // "1.2 MB" formateado
  type: string // mime type
}

function formatFileSize(bytes: number): string {
  if (bytes < 1024 * 1024) {
    return (bytes / 1024).toFixed(1) + ' KB'
  }
  return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
}

// ── Página ────────────────────────────────────────────────────────────────────

export function CreateTicketPage() {
  const navigate = useNavigate()
  const user = useAuthStore((s) => s.user)
  const [submitted, setSubmitted] = useState(false)
  const [createdCode, setCreatedCode] = useState<string>('')
  const [attachments, setAttachments] = useState<AttachedFile[]>([])
  const fileInputRef = useRef<HTMLInputElement>(null)

  const crearTicket = useCrearTicket()
  const tiposServicioQuery = useTiposServicio(user?.empresaId)
  const tiposServicio = (tiposServicioQuery.data ?? []).filter((t) => t.activo)
  const categoriasQuery = useCategorias(user?.empresaId)
  const categorias = (categoriasQuery.data ?? []).filter((c) => c.activa)
  const sucursalesQuery = useSucursales(user?.empresaId)
  const sucursales = (sucursalesQuery.data ?? []).filter((s) => s.activa)
  const areasQuery = useAreas(user?.empresaId)
  const areas = (areasQuery.data ?? []).filter((a) => a.activa)

  const defaultSucursal = user?.sucursalId ?? ''

  const {
    register,
    handleSubmit,
    control,
    watch,
    setValue,
    formState: { errors },
  } = useForm<CreateTicketForm>({
    resolver: zodResolver(createTicketSchema),
    defaultValues: {
      priority: 'media',
      sucursalId: defaultSucursal,
    },
  })

  const watchedSucursal = watch('sucursalId')

  const onSubmit = (data: CreateTicketForm) => {
    crearTicket.mutate(
      {
        titulo: data.title,
        descripcion: data.description,
        sucursalId: data.sucursalId,
        areaId: data.areaId,
        tipoServicioId: data.type,
        categoriaId: data.categoriaId,
        prioridad: data.priority.toUpperCase(),
        ubicacion: data.location,
      },
      {
        onSuccess: (ticketCode) => {
          setCreatedCode(ticketCode ?? '')
          setSubmitted(true)
        },
      },
    )
  }

  function handleFiles(fileList: FileList) {
    const newFiles: AttachedFile[] = Array.from(fileList).map((file) => ({
      id: file.name + file.size,
      file,
      preview: file.type.startsWith('image/') ? URL.createObjectURL(file) : null,
      name: file.name,
      size: formatFileSize(file.size),
      type: file.type,
    }))
    setAttachments((prev) => [...prev, ...newFiles])
  }

  function removeAttachment(id: string) {
    setAttachments((prev) => {
      const removed = prev.find((a) => a.id === id)
      if (removed?.preview) URL.revokeObjectURL(removed.preview)
      return prev.filter((a) => a.id !== id)
    })
  }

  if (submitted) {
    return (
      <div className="flex min-h-[60vh] flex-col items-center justify-center gap-4 p-6 text-center">
        <div className="flex h-16 w-16 items-center justify-center rounded-full bg-green-100 dark:bg-green-900/30">
          <CheckCircle2 className="h-8 w-8 text-green-600 dark:text-green-400" />
        </div>
        <div>
          <h3 className="text-base font-semibold">Ticket creado exitosamente</h3>
          <p className="mt-1 text-sm text-muted-foreground">
            Tu solicitud fue registrada y será asignada en breve.
          </p>
          {createdCode && (
            <p className="mt-1 font-mono text-sm font-semibold text-primary">{createdCode}</p>
          )}
        </div>
        <div className="flex gap-3">
          <Button variant="outline" onClick={() => navigate(ROUTES.TICKETS)}>
            Ver mis tickets
          </Button>
          <Button onClick={() => navigate(ROUTES.DASHBOARD)}>Ir al dashboard</Button>
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-4 p-3 lg:p-5">
      {/* Header */}
      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="icon"
          className="h-8 w-8"
          onClick={() => navigate(-1)}
          aria-label="Volver"
        >
          <ChevronLeft className="h-4 w-4" aria-hidden="true" />
        </Button>
        <div>
          <h2 className="text-base font-semibold tracking-tight">Nuevo ticket</h2>
          <p className="text-xs text-muted-foreground">
            Describe tu solicitud o incidencia con el mayor detalle posible.
          </p>
        </div>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-3">
        {/* ── Clasificación ────────────────────────────────────────────── */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Clasificación
            </CardTitle>
          </CardHeader>
          <CardContent className="p-3 pt-0">
            <div className="grid gap-3 lg:grid-cols-2">
              {/* Tipo de Servicio */}
              <FormField label="Tipo de servicio" required error={errors.type?.message}>
                <Controller
                  control={control}
                  name="type"
                  render={({ field }) => (
                    <Select
                      onValueChange={field.onChange}
                      value={field.value}
                      disabled={tiposServicioQuery.isLoading}
                    >
                      <SelectTrigger
                        id="type"
                        className="[data-error=true]:ring-1 [data-error=true]:ring-destructive/50 h-8 text-xs"
                        data-error={!!errors.type}
                      >
                        <SelectValue
                          placeholder={
                            tiposServicioQuery.isLoading ? 'Cargando...' : 'Seleccionar tipo...'
                          }
                        />
                      </SelectTrigger>
                      <SelectContent>
                        {tiposServicio.map((t) => (
                          <SelectItem key={t.id} value={t.id}>
                            {t.nombre}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
              </FormField>

              {/* Categoría */}
              <FormField label="Categoría" required error={errors.categoriaId?.message}>
                <Controller
                  control={control}
                  name="categoriaId"
                  render={({ field }) => (
                    <Select
                      onValueChange={field.onChange}
                      value={field.value}
                      disabled={categoriasQuery.isLoading}
                    >
                      <SelectTrigger
                        id="categoriaId"
                        className="[data-error=true]:ring-1 [data-error=true]:ring-destructive/50 h-8 text-xs"
                        data-error={!!errors.categoriaId}
                      >
                        <SelectValue
                          placeholder={
                            categoriasQuery.isLoading ? 'Cargando...' : 'Seleccionar categoría...'
                          }
                        />
                      </SelectTrigger>
                      <SelectContent>
                        {categorias.map((c) => (
                          <SelectItem key={c.id} value={c.id}>
                            {c.nombre}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
              </FormField>

              {/* Empresa (sucursalId) */}
              <FormField label="Empresa" required error={errors.sucursalId?.message}>
                <Controller
                  control={control}
                  name="sucursalId"
                  render={({ field }) => (
                    <Select
                      onValueChange={(v) => {
                        field.onChange(v)
                        setValue('areaId', '')
                      }}
                      value={field.value}
                      disabled={sucursalesQuery.isLoading}
                    >
                      <SelectTrigger
                        id="sucursalId"
                        className="[data-error=true]:ring-1 [data-error=true]:ring-destructive/50 h-8 text-xs"
                        data-error={!!errors.sucursalId}
                      >
                        <SelectValue
                          placeholder={
                            sucursalesQuery.isLoading ? 'Cargando...' : 'Seleccionar empresa...'
                          }
                        />
                      </SelectTrigger>
                      <SelectContent>
                        {sucursales.map((s) => (
                          <SelectItem key={s.id} value={s.id}>
                            {s.nombre}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
              </FormField>

              {/* Sucursal (areaId) */}
              <FormField label="Sucursal" required error={errors.areaId?.message}>
                <Controller
                  control={control}
                  name="areaId"
                  render={({ field }) => (
                    <Select
                      onValueChange={field.onChange}
                      value={field.value}
                      disabled={!watchedSucursal || areasQuery.isLoading}
                    >
                      <SelectTrigger
                        id="areaId"
                        className="[data-error=true]:ring-1 [data-error=true]:ring-destructive/50 h-8 text-xs"
                        data-error={!!errors.areaId}
                      >
                        <SelectValue
                          placeholder={
                            !watchedSucursal
                              ? 'Primero selecciona una empresa.'
                              : areasQuery.isLoading
                                ? 'Cargando...'
                                : 'Selecciona una sucursal.'
                          }
                        />
                      </SelectTrigger>
                      <SelectContent>
                        {areas.map((a) => (
                          <SelectItem key={a.id} value={a.id}>
                            {a.nombre}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
              </FormField>

              {/* Prioridad */}
              <FormField label="Prioridad" required>
                <Controller
                  control={control}
                  name="priority"
                  render={({ field }) => (
                    <Select onValueChange={field.onChange} value={field.value}>
                      <SelectTrigger id="priority" className="h-8 text-xs">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        {PRIORITY_OPTIONS.map((p) => (
                          <SelectItem key={p.value} value={p.value}>
                            <div>
                              <p className="font-medium">{p.label}</p>
                              <p className="text-xs text-muted-foreground">{p.description}</p>
                            </div>
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
              </FormField>
            </div>
          </CardContent>
        </Card>

        {/* ── Ubicación específica ──────────────────────────────────────── */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Ubicación
            </CardTitle>
          </CardHeader>
          <CardContent className="p-3 pt-0">
            <FormField label="Ubicación específica" optional>
              <div className="relative">
                <MapPin className="pointer-events-none absolute left-2.5 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-muted-foreground" />
                <Input
                  id="location"
                  className="h-8 pl-7 text-xs"
                  placeholder="Ej: Piso 3 — Módulo B, Impresora del fondo..."
                  {...register('location')}
                />
              </div>
            </FormField>
          </CardContent>
        </Card>

        {/* ── Detalle de la solicitud ───────────────────────────────────── */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Detalle de la solicitud
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 p-3 pt-0">
            <FormField label="Título" required error={errors.title?.message}>
              <Input
                id="title"
                className="h-8 text-xs"
                placeholder="Describe brevemente el problema o solicitud..."
                {...register('title')}
              />
            </FormField>
            <FormField label="Descripción completa" required error={errors.description?.message}>
              <Textarea
                id="description"
                rows={5}
                placeholder="Describe con detalle: ¿Qué ocurrió? ¿Cuándo empezó? ¿Qué intentaste hacer para resolverlo? ¿A cuántos usuarios afecta?"
                className="resize-none text-xs"
                {...register('description')}
              />
            </FormField>
          </CardContent>
        </Card>

        {/* ── Evidencias ───────────────────────────────────────────────── */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Evidencias
              <span className="ml-1 text-[10px] font-normal normal-case tracking-normal text-muted-foreground">
                (Opcional)
              </span>
            </CardTitle>
            <CardDescription className="text-[11px] text-muted-foreground">
              Adjunta fotos, capturas de pantalla, videos o documentos.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-3 p-3 pt-0">
            {/* Galería de archivos adjuntos */}
            {attachments.length > 0 && (
              <div className="grid grid-cols-3 gap-2 sm:grid-cols-4">
                {attachments.map((att) => (
                  <div
                    key={att.id}
                    className="group relative overflow-hidden rounded-lg border bg-muted"
                  >
                    {/* Preview */}
                    {att.preview ? (
                      <img src={att.preview} alt={att.name} className="h-20 w-full object-cover" />
                    ) : (
                      <div className="flex h-20 items-center justify-center">
                        {att.type.includes('pdf') && <FileText className="h-8 w-8 text-red-500" />}
                        {(att.type.includes('word') || att.type.includes('doc')) && (
                          <FileText className="h-8 w-8 text-blue-500" />
                        )}
                        {!att.type.includes('pdf') &&
                          !att.type.includes('word') &&
                          !att.type.includes('doc') && (
                            <Paperclip className="h-8 w-8 text-muted-foreground" />
                          )}
                      </div>
                    )}

                    {/* Información del archivo */}
                    <div className="p-1.5">
                      <p className="truncate text-[10px] font-medium">{att.name}</p>
                      <p className="text-[9px] text-muted-foreground">{att.size}</p>
                    </div>

                    {/* Botón eliminar — siempre visible para garantizar acceso táctil */}
                    <button
                      type="button"
                      onClick={() => removeAttachment(att.id)}
                      className="absolute right-1 top-1 flex h-6 w-6 items-center justify-center rounded-full bg-background/90 shadow-sm transition-colors hover:bg-background"
                      aria-label={`Eliminar ${att.name}`}
                    >
                      <X className="h-3.5 w-3.5" />
                    </button>
                  </div>
                ))}
              </div>
            )}

            {/* Input real de archivo (oculto) */}
            <input
              ref={fileInputRef}
              type="file"
              multiple
              accept="image/*,video/*,.pdf,.doc,.docx,.xls,.xlsx"
              className="sr-only"
              onChange={(e) => e.target.files && handleFiles(e.target.files)}
            />

            {/* Zona de clic para abrir el selector */}
            <button
              type="button"
              onClick={() => fileInputRef.current?.click()}
              className="flex w-full flex-col items-center justify-center gap-1.5 rounded-xl border-2 border-dashed border-border px-4 py-4 transition-colors hover:border-primary/50 hover:bg-muted/50"
            >
              <Upload className="h-5 w-5 text-muted-foreground" />
              <div className="text-center">
                <p className="text-xs font-medium">Haz clic para adjuntar archivo</p>
                <p className="text-[11px] text-muted-foreground">
                  PNG, JPG, GIF, WEBP, MP4, MOV, PDF, DOC, XLS · máx. 10 MB por archivo
                </p>
              </div>
            </button>
          </CardContent>
        </Card>

        {/* ── Acciones ─────────────────────────────────────────────────── */}
        <div className="flex flex-col-reverse gap-3 sm:flex-row sm:justify-end">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate(-1)}
            disabled={crearTicket.isPending}
          >
            Cancelar
          </Button>
          <Button type="submit" disabled={crearTicket.isPending} className="sm:min-w-32">
            {crearTicket.isPending ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Enviando...
              </>
            ) : (
              'Enviar ticket'
            )}
          </Button>
        </div>
      </form>
    </div>
  )
}
