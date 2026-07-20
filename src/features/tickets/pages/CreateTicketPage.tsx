import { useRef, useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useQuery } from '@tanstack/react-query'
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
import { toast } from 'sonner'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Textarea } from '@shared/ui/textarea'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { FormField } from '@shared/components/FormField'
import { SearchableSelect } from '@shared/components/SearchableSelect'
import { useAuthStore } from '@store/auth.store'
import { ROUTES } from '@constants/index'
import { empresaService } from '@features/empresas/services/empresaService'
import { useCrearTicket } from '../hooks/useTickets'
import { ticketService } from '../services/ticketService'
import { useTiposServicio, useSucursales, useCategorias } from '../hooks/useCatalogos'

// ── Schema ────────────────────────────────────────────────────────────────────

const createTicketSchema = z.object({
  title: z.string().max(150, 'Máximo 150 caracteres.').optional(),
  type: z.string().min(1, 'Selecciona un tipo de servicio.'),
  categoriaId: z.string().min(1, 'La categoría del sistema no está disponible.'),
  priority: z.enum(['baja', 'media', 'alta', 'critica'], {
    errorMap: () => ({ message: 'Selecciona una prioridad.' }),
  }),
  sucursalId: z.string().min(1, 'Selecciona una sucursal.'),
  areaNombre: z.string().min(1, 'Ingresa el área.').max(150, 'Máximo 150 caracteres.'),
  location: z.string().max(200, 'Máximo 200 caracteres').optional(),
  description: z.string().max(5000, 'Máximo 5000 caracteres.').optional(),
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

  const isSuperAdmin = user?.rol === 'superadmin'
  const isAdmin = user?.rol === 'admin'

  // SUPERADMIN empieza sin empresa seleccionada; los demás roles usan la suya
  const [selectedEmpresaId, setSelectedEmpresaId] = useState<string>(
    isSuperAdmin ? '' : (user?.empresaId ?? ''),
  )

  // Solo Admin/SuperAdmin pueden listar empresas (backend devuelve 403 a otros roles)
  const empresasQuery = useQuery({
    queryKey: ['empresas', 'ticket-form'],
    queryFn: () => empresaService.listar({ soloActivas: true, tamanoPagina: 200 }),
    enabled: isSuperAdmin || isAdmin,
    staleTime: 1000 * 60 * 5,
    select: (data) => data.items ?? [],
  })

  // Auto-seleccionar empresa para SuperAdmin si solo existe una
  useEffect(() => {
    if (!isSuperAdmin || selectedEmpresaId) return
    const items = empresasQuery.data ?? []
    if (items.length === 1) setSelectedEmpresaId(items[0].id)
  }, [empresasQuery.data, isSuperAdmin, selectedEmpresaId])

  const crearTicket = useCrearTicket()
  const tiposServicioQuery = useTiposServicio(user?.empresaId)
  const tiposServicio = (tiposServicioQuery.data ?? []).filter((t) => t.activo)
  const categoriasQuery = useCategorias(user?.empresaId)

  // Sucursales filtradas por la empresa seleccionada (useSucursales solo carga si hay empresaId)
  const sucursalesQuery = useSucursales(selectedEmpresaId || undefined)
  const sucursales = (sucursalesQuery.data ?? []).filter((s) => s.activa)

  const {
    register,
    handleSubmit,
    control,
    setValue,
    formState: { errors },
  } = useForm<CreateTicketForm>({
    resolver: zodResolver(createTicketSchema),
    defaultValues: {
      priority: 'media',
      sucursalId: isSuperAdmin ? '' : (user?.sucursalId ?? ''),
    },
  })

  const [categoriaError, setCategoriaError] = useState<string | null>(null)
  const [isUploadingEvidencias, setIsUploadingEvidencias] = useState(false)

  useEffect(() => {
    if (categoriasQuery.isLoading) return
    const data = categoriasQuery.data ?? []
    const sg = data.find((c) => c.activa && c.nombre === 'Servicios Generales')
    if (sg) {
      setValue('categoriaId', sg.id)
      setCategoriaError(null)
    } else {
      setCategoriaError('No se encontró la categoría del sistema. Contacte al administrador.')
    }
  }, [categoriasQuery.data, categoriasQuery.isLoading, setValue])

  function handleEmpresaChange(empresaId: string) {
    setSelectedEmpresaId(empresaId)
    setValue('sucursalId', '') // limpiar sucursal al cambiar empresa
  }

  const onSubmit = async (data: CreateTicketForm) => {
    let ticketId: string
    try {
      ticketId = await crearTicket.mutateAsync({
        titulo: data.title?.trim() || undefined,
        descripcion: data.description?.trim() || undefined,
        sucursalId: data.sucursalId,
        areaNombre: data.areaNombre,
        tipoServicioId: data.type,
        categoriaId: data.categoriaId,
        prioridad: data.priority.toUpperCase(),
        ubicacion: data.location,
      })
    } catch (err) {
      toast.error(
        (err as Error).message ??
          'No se pudo crear el ticket. Revisa los campos e intenta de nuevo.',
      )
      return
    }

    if (attachments.length > 0) {
      setIsUploadingEvidencias(true)
      const resultados = await Promise.allSettled(
        attachments.map((a) => ticketService.subirEvidencia(ticketId, a.file, 'INICIAL')),
      )
      setIsUploadingEvidencias(false)

      const fallidas = resultados.filter((r) => r.status === 'rejected').length
      if (fallidas > 0) {
        const exitosas = attachments.length - fallidas
        if (exitosas > 0) {
          toast.warning(
            `Ticket creado. ${exitosas} evidencia(s) subida(s), pero ${fallidas} no pudieron guardarse.`,
          )
        } else {
          toast.error(
            'Ticket creado, pero ninguna evidencia pudo guardarse. Puedes subirlas desde el detalle del ticket.',
          )
        }
      }
    }

    attachments.forEach((a) => {
      if (a.preview) URL.revokeObjectURL(a.preview)
    })
    setCreatedCode(ticketId)
    setSubmitted(true)
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

  const empresaOptions = (empresasQuery.data ?? []).map((e) => ({
    value: e.id,
    label: e.nombreComercial,
  }))

  const sucursalOptions = sucursales.map((s) => ({ value: s.id, label: s.nombre }))

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
              {/* Empresa — solo SuperAdmin (interactivo) y Admin (informativo) */}
              {isSuperAdmin && (
                <FormField label="Empresa" required>
                  <SearchableSelect
                    options={empresaOptions}
                    value={selectedEmpresaId}
                    onChange={handleEmpresaChange}
                    placeholder="Seleccionar empresa..."
                    searchPlaceholder="Buscar empresa..."
                    emptyMessage="No se encontraron empresas."
                    loading={empresasQuery.isLoading}
                    hasError={false}
                  />
                </FormField>
              )}

              {isAdmin && (
                <FormField label="Empresa">
                  <div className="flex h-8 items-center rounded-md border border-input bg-muted/40 px-3 text-xs">
                    <span className="font-medium">
                      {empresasQuery.isLoading
                        ? 'Cargando...'
                        : (empresasQuery.data?.[0]?.nombreComercial ?? '—')}
                    </span>
                  </div>
                </FormField>
              )}

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

              {/* Categoría — asignada automáticamente por el sistema */}
              <FormField label="Categoría" error={categoriaError ?? errors.categoriaId?.message}>
                <div
                  className="flex h-8 items-center rounded-md border border-input bg-muted/40 px-3 text-xs"
                  aria-label="Categoría asignada automáticamente"
                >
                  {categoriasQuery.isLoading ? (
                    <span className="flex items-center gap-1.5 text-muted-foreground">
                      <Loader2 className="h-3 w-3 animate-spin" />
                      Cargando...
                    </span>
                  ) : (
                    <span className="font-medium">Servicios Generales</span>
                  )}
                </div>
              </FormField>

              {/* Sucursal — con búsqueda por escritura */}
              <FormField label="Sucursal" required error={errors.sucursalId?.message}>
                <Controller
                  control={control}
                  name="sucursalId"
                  render={({ field }) => (
                    <SearchableSelect
                      options={sucursalOptions}
                      value={field.value}
                      onChange={field.onChange}
                      placeholder={
                        isSuperAdmin && !selectedEmpresaId
                          ? 'Selecciona una empresa primero'
                          : 'Seleccionar sucursal...'
                      }
                      searchPlaceholder="Buscar sucursal..."
                      emptyMessage="No se encontraron sucursales."
                      loading={sucursalesQuery.isLoading}
                      disabled={isSuperAdmin && !selectedEmpresaId}
                      hasError={!!errors.sucursalId}
                    />
                  )}
                />
              </FormField>

              {/* Área */}
              <FormField label="Área" required error={errors.areaNombre?.message}>
                <Input
                  id="areaNombre"
                  className="h-8 text-xs"
                  placeholder="Ej: Sistemas, Contabilidad, Recursos Humanos..."
                  {...register('areaNombre')}
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

        {/* ── Ubicación específica del servicio ────────────────────────── */}
        <Card>
          <CardHeader className="px-3 pb-2 pt-3">
            <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Ubicación
            </CardTitle>
          </CardHeader>
          <CardContent className="p-3 pt-0">
            <FormField label="Ubicación específica del servicio" optional>
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
            <FormField label="Título" error={errors.title?.message}>
              <Input
                id="title"
                className="h-8 text-xs"
                placeholder="Describe brevemente el problema o solicitud..."
                {...register('title')}
              />
            </FormField>
            <FormField label="Descripción completa" error={errors.description?.message}>
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
            disabled={crearTicket.isPending || isUploadingEvidencias}
          >
            Cancelar
          </Button>
          <Button
            type="submit"
            disabled={crearTicket.isPending || isUploadingEvidencias}
            className="sm:min-w-32"
          >
            {isUploadingEvidencias ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Subiendo archivos...
              </>
            ) : crearTicket.isPending ? (
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
