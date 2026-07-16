import { useState, useEffect } from 'react'
import { Save, Users, RotateCcw } from 'lucide-react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { useAuthStore } from '@store/auth.store'
import { apiClient } from '@services/apiClient'
import { configuracionService } from '../services/configuracionService'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Button } from '@shared/ui/button'
import { Badge } from '@shared/ui/badge'
import { Input } from '@shared/ui/input'
import { Textarea } from '@shared/ui/textarea'
import { Skeleton } from '@shared/ui/skeleton'

// ─── Tipos ───────────────────────────────────────────────────────────────────

interface RolDto {
  codigo: string
  nombre: string
  descripcion: string | null
  activo: boolean
}

interface RolesListResult {
  items: RolDto[]
  total: number
}

// ─── Constantes ──────────────────────────────────────────────────────────────

const ROL_COLORS: Record<string, string> = {
  SUPERADMIN: 'border-purple-200 bg-purple-50 text-purple-700',
  ADMIN: 'border-blue-200 bg-blue-50 text-blue-700',
  SUPERVISOR: 'border-cyan-200 bg-cyan-50 text-cyan-700',
  TECNICO: 'border-green-200 bg-green-50 text-green-700',
  TRABAJADOR: 'border-amber-200 bg-amber-50 text-amber-700',
  USUARIO: 'border-slate-200 bg-slate-50 text-slate-700',
}

const ROL_DEFAULTS: Record<string, { label: string; descripcion: string }> = {
  SUPERADMIN: {
    label: 'Super Administrador',
    descripcion: 'Acceso total. Gestiona empresas, usuarios, configuración y catálogos globales.',
  },
  ADMIN: {
    label: 'Administrador',
    descripcion: 'Gestiona su empresa: sucursales, usuarios, tickets y configuración.',
  },
  SUPERVISOR: {
    label: 'Supervisor',
    descripcion: 'Supervisa tickets y equipos. Sin acceso a configuración del sistema.',
  },
  TECNICO: {
    label: 'Técnico',
    descripcion: 'Atiende los tickets asignados, actualiza estados y registra soluciones.',
  },
  TRABAJADOR: {
    label: 'Trabajador',
    descripcion: 'Crea tickets y hace seguimiento de sus propias solicitudes.',
  },
  USUARIO: {
    label: 'Usuario',
    descripcion: 'Usuario externo o libre. Puede crear tickets sin pertenecer al equipo interno.',
  },
}

// ─── Helpers de clave de configuración ───────────────────────────────────────

function claveLabelRol(codigo: string) {
  return `ROL_LABEL_${codigo}`
}

function claveDescRol(codigo: string) {
  return `ROL_DESC_${codigo}`
}

// ─── Componente principal ─────────────────────────────────────────────────────

export function SeccionRoles() {
  const qc = useQueryClient()
  const user = useAuthStore((s) => s.user)
  const empresaId = user?.empresaId

  // ── Datos del backend ─────────────────────────────────────────────────────
  const { data: rolesData, isLoading: loadingRoles } = useQuery({
    queryKey: ['roles-sistema'],
    queryFn: () => apiClient.get<RolesListResult>('/roles'),
  })

  const { data: configData } = useQuery({
    queryKey: ['configuracion', empresaId],
    queryFn: () => configuracionService.listar(empresaId),
    retry: false,
  })

  // ── Estado local de etiquetas y descripciones ─────────────────────────────
  const [labels, setLabels] = useState<Record<string, string>>({})
  const [descs, setDescs] = useState<Record<string, string>>({})
  const [saving, setSaving] = useState<string | null>(null)

  const roles = rolesData?.items ?? []

  // Sincronizar valores desde configuración al cargar
  useEffect(() => {
    if (!configData) return
    const get = (clave: string) => configData.find((p) => p.clave === clave)?.valor
    const newLabels: Record<string, string> = {}
    const newDescs: Record<string, string> = {}
    Object.keys(ROL_DEFAULTS).forEach((codigo) => {
      const labelVal = get(claveLabelRol(codigo))
      const descVal = get(claveDescRol(codigo))
      newLabels[codigo] = labelVal ?? ROL_DEFAULTS[codigo].label
      newDescs[codigo] = descVal ?? ROL_DEFAULTS[codigo].descripcion
    })
    setLabels(newLabels)
    setDescs(newDescs)
  }, [configData])

  // ── Guardar etiquetas de un rol específico ────────────────────────────────
  const { mutate: actualizar } = useMutation({
    mutationFn: ({ clave, valor }: { clave: string; valor: string }) =>
      configuracionService.actualizar(clave, valor, empresaId),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: ['configuracion'] })
    },
    onError: (e: Error) => toast.error(e.message),
  })

  async function guardarRol(codigo: string) {
    setSaving(codigo)
    try {
      const label = labels[codigo]?.trim() || ROL_DEFAULTS[codigo].label
      const desc = descs[codigo]?.trim() || ROL_DEFAULTS[codigo].descripcion
      actualizar({ clave: claveLabelRol(codigo), valor: label })
      actualizar({ clave: claveDescRol(codigo), valor: desc })
      toast.success(`Etiqueta de "${ROL_DEFAULTS[codigo].label}" guardada`)
    } finally {
      setSaving(null)
    }
  }

  function resetearRol(codigo: string) {
    setLabels((prev) => ({ ...prev, [codigo]: ROL_DEFAULTS[codigo].label }))
    setDescs((prev) => ({ ...prev, [codigo]: ROL_DEFAULTS[codigo].descripcion }))
  }

  return (
    <Card className="lg:col-span-2">
      <CardHeader className="px-3 pb-2 pt-3">
        <div className="flex flex-col gap-0.5">
          <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            <Users className="h-3.5 w-3.5 text-blue-500" />
            Roles del sistema
          </CardTitle>
          <p className="text-xs text-muted-foreground">
            Los roles controlan los permisos de acceso. Puedes personalizar el nombre y la
            descripción que verán los usuarios en cada empresa.
          </p>
        </div>
      </CardHeader>
      <CardContent className="p-3 pt-0">
        {loadingRoles ? (
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
            {[1, 2, 3, 4, 5, 6].map((i) => (
              <Skeleton key={i} className="h-36 w-full rounded-lg" />
            ))}
          </div>
        ) : (
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
            {(roles.length > 0
              ? roles
              : Object.keys(ROL_DEFAULTS).map((codigo): RolDto => ({
                  codigo,
                  nombre: ROL_DEFAULTS[codigo].label,
                  descripcion: ROL_DEFAULTS[codigo].descripcion,
                  activo: true,
                }))
            ).map((rol) => {
              const color = ROL_COLORS[rol.codigo] ?? 'border-gray-200 bg-gray-50 text-gray-700'
              const labelValue = labels[rol.codigo] ?? ROL_DEFAULTS[rol.codigo]?.label ?? rol.nombre
              const descValue = descs[rol.codigo] ?? ROL_DEFAULTS[rol.codigo]?.descripcion ?? ''
              const isSaving = saving === rol.codigo
              const isModified =
                labelValue !== (ROL_DEFAULTS[rol.codigo]?.label ?? rol.nombre) ||
                descValue !== (ROL_DEFAULTS[rol.codigo]?.descripcion ?? '')

              return (
                <div
                  key={rol.codigo}
                  className="flex flex-col gap-2.5 rounded-lg border bg-card p-3 shadow-sm"
                >
                  {/* Badge del rol del sistema */}
                  <div className="flex items-center justify-between">
                    <Badge
                      variant="outline"
                      className={`border text-[10px] font-semibold ${color}`}
                    >
                      {rol.codigo}
                    </Badge>
                    <div className="flex gap-1">
                      {isModified && (
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-5 w-5"
                          title="Restablecer valores por defecto"
                          onClick={() => resetearRol(rol.codigo)}
                        >
                          <RotateCcw className="h-3 w-3 text-muted-foreground" />
                        </Button>
                      )}
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-5 w-5"
                        title="Guardar etiqueta"
                        disabled={isSaving}
                        onClick={() => guardarRol(rol.codigo)}
                      >
                        <Save className="h-3 w-3 text-primary" />
                      </Button>
                    </div>
                  </div>

                  {/* Nombre personalizable */}
                  <div className="space-y-1">
                    <label className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">
                      Nombre visible
                    </label>
                    <Input
                      className="h-7 text-xs"
                      value={labelValue}
                      onChange={(e) =>
                        setLabels((prev) => ({ ...prev, [rol.codigo]: e.target.value }))
                      }
                      placeholder={ROL_DEFAULTS[rol.codigo]?.label ?? rol.nombre}
                    />
                  </div>

                  {/* Descripción personalizable */}
                  <div className="space-y-1">
                    <label className="text-[10px] font-medium uppercase tracking-widest text-muted-foreground">
                      Descripción
                    </label>
                    <Textarea
                      className="resize-none text-xs leading-relaxed"
                      rows={2}
                      value={descValue}
                      onChange={(e) =>
                        setDescs((prev) => ({ ...prev, [rol.codigo]: e.target.value }))
                      }
                      placeholder={ROL_DEFAULTS[rol.codigo]?.descripcion ?? ''}
                    />
                  </div>
                </div>
              )
            })}
          </div>
        )}

        <p className="mt-3 text-[10px] text-muted-foreground">
          Los cambios en las etiquetas son por empresa y solo afectan la visualización. Los permisos
          de cada rol están definidos a nivel del sistema y no cambian.
        </p>
      </CardContent>
    </Card>
  )
}
