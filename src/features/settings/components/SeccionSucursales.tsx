import { useState } from 'react'
import { Plus, ExternalLink, Power, PowerOff, MapPin } from 'lucide-react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { toast } from 'sonner'
import { Link } from 'react-router-dom'
import { sucursalService } from '@features/sucursales/services/sucursalService'
import { empresaService } from '@features/empresas/services/empresaService'
import { useAuthStore } from '@store/auth.store'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Button } from '@shared/ui/button'
import { Badge } from '@shared/ui/badge'
import { Input } from '@shared/ui/input'
import { Textarea } from '@shared/ui/textarea'
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@shared/ui/dialog'
import { FormField } from '@shared/components/FormField'
import { SearchableSelect } from '@shared/components/SearchableSelect'
import { Skeleton } from '@shared/ui/skeleton'
import { ROUTES } from '@constants/index'

const schema = z.object({
  empresaId: z.string().min(1, 'Selecciona una empresa.'),
  nombre: z.string().min(1, 'Requerido').max(100, 'Máximo 100 caracteres'),
  descripcion: z.string().max(300, 'Máximo 300 caracteres').optional(),
  direccion: z.string().max(200, 'Máximo 200 caracteres').optional(),
})
type FormValues = z.infer<typeof schema>

export function SeccionSucursales() {
  const qc = useQueryClient()
  const user = useAuthStore((s) => s.user)
  const isSuperAdmin = user?.rol === 'superadmin'
  const empresaId = user?.empresaId

  const [dialogOpen, setDialogOpen] = useState(false)

  const { data, isLoading } = useQuery({
    queryKey: ['sucursales-config', empresaId],
    queryFn: () =>
      sucursalService.listar({ tamanoPagina: 100, ...(empresaId ? { empresaId } : {}) }),
  })

  const empresasQuery = useQuery({
    queryKey: ['empresas', 'settings-sucursal'],
    queryFn: () => empresaService.listar({ soloActivas: true, tamanoPagina: 100 }),
    enabled: isSuperAdmin,
    staleTime: 1000 * 60 * 5,
    select: (d) => d.items ?? [],
  })
  const empresas = empresasQuery.data ?? []

  const items = data?.items ?? []

  const {
    register,
    handleSubmit,
    reset,
    control,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { empresaId: '', nombre: '', descripcion: '', direccion: '' },
  })

  const { mutateAsync: crear } = useMutation({
    mutationFn: (v: FormValues) =>
      sucursalService.crear({
        empresaId: v.empresaId,
        nombre: v.nombre.trim(),
        descripcion: v.descripcion?.trim() || undefined,
        direccion: v.direccion?.trim() || undefined,
      }),
    onSuccess: () => {
      void qc.invalidateQueries({ queryKey: ['sucursales-config'] })
      void qc.invalidateQueries({ queryKey: ['sucursales'] })
      toast.success('Sucursal creada')
      setDialogOpen(false)
    },
    onError: (e: Error) => toast.error(e.message),
  })

  const { mutate: toggle } = useMutation({
    mutationFn: ({ id, activa }: { id: string; activa: boolean }) =>
      activa ? sucursalService.desactivar(id) : sucursalService.activar(id),
    onSuccess: (_, { activa }) => {
      void qc.invalidateQueries({ queryKey: ['sucursales-config'] })
      void qc.invalidateQueries({ queryKey: ['sucursales'] })
      toast.success(activa ? 'Sucursal desactivada' : 'Sucursal activada')
    },
    onError: (e: Error) => toast.error(e.message),
  })

  function openCreate() {
    reset({
      empresaId: isSuperAdmin ? '' : (empresaId ?? ''),
      nombre: '',
      descripcion: '',
      direccion: '',
    })
    setDialogOpen(true)
  }

  async function onSubmit(v: FormValues) {
    await crear(v)
  }

  return (
    <>
      <Card className="lg:col-span-2">
        <CardHeader className="px-3 pb-2 pt-3">
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              <MapPin className="h-3.5 w-3.5 text-blue-500" />
              Sucursales
            </CardTitle>
            <Button size="sm" className="h-7 gap-1 text-xs" onClick={openCreate}>
              <Plus className="h-3.5 w-3.5" />
              Nueva sucursal
            </Button>
          </div>
        </CardHeader>
        <CardContent className="p-3 pt-0">
          {isLoading ? (
            <div className="space-y-2">
              {[1, 2, 3].map((i) => (
                <Skeleton key={i} className="h-10 w-full" />
              ))}
            </div>
          ) : items.length === 0 ? (
            <p className="py-6 text-center text-xs text-muted-foreground">
              No hay sucursales registradas.{' '}
              <button
                className="text-primary underline-offset-4 hover:underline"
                onClick={openCreate}
              >
                Crea la primera.
              </button>
            </p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-xs">
                <thead>
                  <tr className="border-b text-left text-[10px] font-semibold uppercase tracking-widest text-muted-foreground">
                    <th className="pb-2 pr-3">Nombre</th>
                    <th className="hidden pb-2 pr-3 sm:table-cell">Dirección</th>
                    <th className="w-20 pb-2 pr-3 text-center">Estado</th>
                    <th className="w-16 pb-2 text-right">Acciones</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {items.map((item) => (
                    <tr key={item.id}>
                      <td className="py-2.5 pr-3 font-medium">{item.nombre}</td>
                      <td className="hidden py-2.5 pr-3 text-muted-foreground sm:table-cell">—</td>
                      <td className="py-2.5 pr-3 text-center">
                        <Badge
                          variant={item.activa ? 'default' : 'secondary'}
                          className="text-[10px]"
                        >
                          {item.activa ? 'Activa' : 'Inactiva'}
                        </Badge>
                      </td>
                      <td className="py-2.5">
                        <div className="flex justify-end gap-1">
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-6 w-6"
                            title="Ver detalle"
                            asChild
                          >
                            <Link to={ROUTES.SUCURSALES_DETAIL.replace(':id', item.id)}>
                              <ExternalLink className="h-3 w-3" />
                            </Link>
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-6 w-6"
                            title={item.activa ? 'Desactivar' : 'Activar'}
                            onClick={() => toggle({ id: item.id, activa: item.activa })}
                          >
                            {item.activa ? (
                              <PowerOff className="h-3 w-3 text-amber-500" />
                            ) : (
                              <Power className="h-3 w-3 text-emerald-500" />
                            )}
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="max-w-sm">
          <DialogHeader>
            <DialogTitle>Nueva sucursal</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-3 pt-1">
            {isSuperAdmin ? (
              <FormField label="Empresa" required error={errors.empresaId?.message}>
                <Controller
                  name="empresaId"
                  control={control}
                  render={({ field }) => (
                    <SearchableSelect
                      options={empresas.map((e) => ({ value: e.id, label: e.nombreComercial }))}
                      value={field.value}
                      onChange={field.onChange}
                      placeholder="Seleccionar empresa"
                      searchPlaceholder="Buscar empresa..."
                      emptyMessage="Sin empresas."
                      hasError={!!errors.empresaId}
                      loading={empresasQuery.isLoading}
                    />
                  )}
                />
              </FormField>
            ) : (
              <input type="hidden" {...register('empresaId')} />
            )}
            <FormField label="Nombre" required error={errors.nombre?.message}>
              <Input
                className="h-8 text-sm"
                placeholder="Ej: Sede Central"
                {...register('nombre')}
              />
            </FormField>
            <FormField label="Descripción" error={errors.descripcion?.message}>
              <Textarea
                className="resize-none text-sm"
                rows={2}
                placeholder="Descripción opcional..."
                {...register('descripcion')}
              />
            </FormField>
            <FormField label="Dirección" error={errors.direccion?.message}>
              <Input
                className="h-8 text-sm"
                placeholder="Av. Principal 123, Lima"
                {...register('direccion')}
              />
            </FormField>
            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() => setDialogOpen(false)}
              >
                Cancelar
              </Button>
              <Button type="submit" size="sm" disabled={isSubmitting}>
                Crear sucursal
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>
    </>
  )
}
