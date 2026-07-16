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
import { Skeleton } from '@shared/ui/skeleton'

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

const FALLBACK_ROLES: RolDto[] = Object.keys(ROL_DEFAULTS).map((codigo) => ({
  codigo,
  nombre: ROL_DEFAULTS[codigo].label,
  descripcion: ROL_DEFAULTS[codigo].descripcion,
  activo: true,
}))

export function SeccionRoles() {
  const qc = useQueryClient()
  const user = useAuthStore((s) => s.user)
  const empresaId = user?.empresaId

  const { data: rolesData, isLoading: loadingRoles } = useQuery({
    queryKey: ['roles-sistema'],
    queryFn: () => apiClient.get<RolesListResult>('/roles'),
  })

  const { data: configData } = useQuery({
    queryKey: ['configuracion', empresaId],
    queryFn: () => configuracionService.listar(empresaId),
    retry: false,
  })

  const [labels, setLabels] = useState<Record<string, string>>({})
  const [descs, setDescs] = useState<Record<string, string>>({})
  const [saving, setSaving] = useState<string | null>(null)

  const roles = rolesData?.items?.length ? rolesData.items : FALLBACK_ROLES

  useEffect(() => {
    if (!configData) return
    const get = (clave: string) => configData.find((p) => p.clave === clave)?.valor
    const newLabels: Record<string, string> = {}
    const newDescs: Record<string, string> = {}
    Object.keys(ROL_DEFAULTS).forEach((codigo) => {
      newLabels[codigo] = get(`ROL_LABEL_${codigo}`) ?? ROL_DEFAULTS[codigo].label
      newDescs[codigo] = get(`ROL_DESC_${codigo}`) ?? ROL_DEFAULTS[codigo].descripcion
    })
    setLabels(newLabels)
    setDescs(newDescs)
  }, [configData])

  const { mutate: actualizar } = useMutation({
    mutationFn: ({ clave, valor }: { clave: string; valor: string }) =>
      configuracionService.actualizar(clave, valor, empresaId),
    onSuccess: () => void qc.invalidateQueries({ queryKey: ['configuracion'] }),
    onError: (e: Error) => toast.error(e.message),
  })

  function guardarRol(codigo: string) {
    setSaving(codigo)
    const label = labels[codigo]?.trim() || ROL_DEFAULTS[codigo].label
    const desc = descs[codigo]?.trim() || ROL_DEFAULTS[codigo].descripcion
    actualizar({ clave: `ROL_LABEL_${codigo}`, valor: label })
    actualizar({ clave: `ROL_DESC_${codigo}`, valor: desc }, { onSettled: () => setSaving(null) })
    toast.success('Etiqueta guardada')
  }

  function resetearRol(codigo: string) {
    setLabels((prev) => ({ ...prev, [codigo]: ROL_DEFAULTS[codigo].label }))
    setDescs((prev) => ({ ...prev, [codigo]: ROL_DEFAULTS[codigo].descripcion }))
  }

  return (
    <Card className="lg:col-span-2">
      <CardHeader className="px-3 pb-2 pt-3">
        <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
          <Users className="h-3.5 w-3.5 text-blue-500" />
          Roles del sistema
        </CardTitle>
      </CardHeader>
      <CardContent className="p-3 pt-0">
        {loadingRoles ? (
          <div className="space-y-2">
            {[1, 2, 3, 4, 5, 6].map((i) => (
              <Skeleton key={i} className="h-9 w-full" />
            ))}
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-xs">
              <thead>
                <tr className="border-b text-left text-[10px] font-semibold uppercase tracking-widest text-muted-foreground">
                  <th className="pb-2 pr-3">Rol</th>
                  <th className="pb-2 pr-3">Nombre visible</th>
                  <th className="hidden pb-2 pr-3 sm:table-cell">Descripción</th>
                  <th className="w-16 pb-2 text-right">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {roles.map((rol) => {
                  const color = ROL_COLORS[rol.codigo] ?? 'border-gray-200 bg-gray-50 text-gray-700'
                  const labelValue =
                    labels[rol.codigo] ?? ROL_DEFAULTS[rol.codigo]?.label ?? rol.nombre
                  const descValue = descs[rol.codigo] ?? ROL_DEFAULTS[rol.codigo]?.descripcion ?? ''
                  const isModified =
                    labelValue !== (ROL_DEFAULTS[rol.codigo]?.label ?? rol.nombre) ||
                    descValue !== (ROL_DEFAULTS[rol.codigo]?.descripcion ?? '')

                  return (
                    <tr key={rol.codigo}>
                      <td className="py-2 pr-3">
                        <Badge
                          variant="outline"
                          className={`whitespace-nowrap text-[10px] font-semibold ${color}`}
                        >
                          {rol.codigo}
                        </Badge>
                      </td>
                      <td className="py-2 pr-3">
                        <Input
                          className="h-7 min-w-[120px] text-xs"
                          value={labelValue}
                          onChange={(e) =>
                            setLabels((prev) => ({ ...prev, [rol.codigo]: e.target.value }))
                          }
                          placeholder={ROL_DEFAULTS[rol.codigo]?.label ?? rol.nombre}
                        />
                      </td>
                      <td className="hidden py-2 pr-3 sm:table-cell">
                        <Input
                          className="h-7 text-xs"
                          value={descValue}
                          onChange={(e) =>
                            setDescs((prev) => ({ ...prev, [rol.codigo]: e.target.value }))
                          }
                          placeholder={ROL_DEFAULTS[rol.codigo]?.descripcion ?? ''}
                        />
                      </td>
                      <td className="py-2">
                        <div className="flex justify-end gap-1">
                          {isModified && (
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-6 w-6"
                              title="Restablecer"
                              onClick={() => resetearRol(rol.codigo)}
                            >
                              <RotateCcw className="h-3 w-3 text-muted-foreground" />
                            </Button>
                          )}
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-6 w-6"
                            title="Guardar"
                            disabled={saving === rol.codigo}
                            onClick={() => guardarRol(rol.codigo)}
                          >
                            <Save className="h-3 w-3 text-primary" />
                          </Button>
                        </div>
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>
        )}
        <p className="mt-3 text-[10px] text-muted-foreground">
          Los permisos de cada rol son del sistema y no cambian. Solo se personaliza la etiqueta
          visible.
        </p>
      </CardContent>
    </Card>
  )
}
