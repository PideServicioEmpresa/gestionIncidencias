import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Building2 } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Skeleton } from '@shared/ui/skeleton'
import { FormField } from '@shared/components/FormField'
import { useEmpresa, useActualizarEmpresa } from '../hooks/useEmpresas'
import { ROUTES, empresaDetailPath } from '@constants/index'

const ZONAS_HORARIAS = [
  'America/Lima',
  'America/Bogota',
  'America/Santiago',
  'America/Buenos_Aires',
  'America/Caracas',
  'America/Mexico_City',
  'America/New_York',
  'UTC',
]

interface FormState {
  nombreComercial: string
  razonSocial: string
  zonaHoraria: string
  logoUrl: string
  colorPrimario: string
  colorSecundario: string
}

export function EmpresaEditPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: empresa, isLoading } = useEmpresa(id ?? '')
  const actualizarEmpresa = useActualizarEmpresa(id ?? '')

  const [form, setForm] = useState<FormState>({
    nombreComercial: '',
    razonSocial: '',
    zonaHoraria: 'America/Lima',
    logoUrl: '',
    colorPrimario: '',
    colorSecundario: '',
  })
  const [errors, setErrors] = useState<Partial<Record<keyof FormState, string>>>({})
  const [ready, setReady] = useState(false)

  useEffect(() => {
    if (empresa && !ready) {
      setForm({
        nombreComercial: empresa.nombreComercial,
        razonSocial: empresa.razonSocial,
        zonaHoraria: empresa.zonaHoraria,
        logoUrl: empresa.logoUrl ?? '',
        colorPrimario: empresa.colorPrimario ?? '',
        colorSecundario: empresa.colorSecundario ?? '',
      })
      setReady(true)
    }
  }, [empresa, ready])

  function handleChange(field: keyof FormState, value: string) {
    setForm((prev) => ({ ...prev, [field]: value }))
    setErrors((prev) => ({ ...prev, [field]: undefined }))
  }

  function validate(): boolean {
    const next: Partial<Record<keyof FormState, string>> = {}
    if (!form.nombreComercial.trim()) next.nombreComercial = 'Ingresa el nombre comercial.'
    if (!form.razonSocial.trim()) next.razonSocial = 'Ingresa la razón social.'
    if (!form.zonaHoraria.trim()) next.zonaHoraria = 'Selecciona una zona horaria.'
    if (form.colorPrimario && !/^#[0-9A-Fa-f]{6}$/.test(form.colorPrimario))
      next.colorPrimario = 'Formato inválido. Usa #RRGGBB (ej. #2563eb).'
    if (form.colorSecundario && !/^#[0-9A-Fa-f]{6}$/.test(form.colorSecundario))
      next.colorSecundario = 'Formato inválido. Usa #RRGGBB (ej. #64748b).'
    setErrors(next)
    return Object.keys(next).length === 0
  }

  function handleSubmit() {
    if (!validate() || !id) return
    actualizarEmpresa.mutate(
      {
        nombreComercial: form.nombreComercial.trim(),
        razonSocial: form.razonSocial.trim(),
        zonaHoraria: form.zonaHoraria.trim(),
        logoUrl: form.logoUrl.trim() || undefined,
        colorPrimario: form.colorPrimario.trim() || undefined,
        colorSecundario: form.colorSecundario.trim() || undefined,
      },
      { onSuccess: () => navigate(empresaDetailPath(id)) },
    )
  }

  if (isLoading) {
    return (
      <div className="px-3 py-3 lg:px-5">
        <div className="mb-4 flex items-center gap-2">
          <Skeleton className="h-8 w-8 rounded" />
          <Skeleton className="h-5 w-48" />
        </div>
        <Card>
          <CardContent className="space-y-3 p-4">
            <Skeleton className="h-9 w-full" />
            <Skeleton className="h-9 w-full" />
            <Skeleton className="h-9 w-full" />
          </CardContent>
        </Card>
      </div>
    )
  }

  const backPath = id ? empresaDetailPath(id) : ROUTES.EMPRESAS

  return (
    <div className="px-3 py-3 lg:px-5">
      {/* Header */}
      <div className="mb-4 flex items-center justify-between gap-3">
        <div className="flex items-center gap-2">
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8 shrink-0"
            onClick={() => navigate(backPath)}
            aria-label="Volver"
          >
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <div>
            <h2 className="text-base font-semibold tracking-tight">Editar empresa</h2>
            <p className="text-xs text-muted-foreground">{empresa?.nombreComercial}</p>
          </div>
        </div>
        <Button variant="outline" size="sm" onClick={() => navigate(backPath)}>
          Cancelar
        </Button>
      </div>

      <div className="flex flex-col gap-4 lg:flex-row lg:items-start">
        {/* Formulario */}
        <div className="flex-1 space-y-4">
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-sm font-semibold">Información general</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 p-3 pt-0">
              <FormField label="Nombre comercial" required error={errors.nombreComercial}>
                <Input
                  className="h-9 text-sm"
                  placeholder="Ej. Empresa ABC"
                  value={form.nombreComercial}
                  onChange={(e) => handleChange('nombreComercial', e.target.value)}
                />
              </FormField>
              <FormField label="Razón social" required error={errors.razonSocial}>
                <Input
                  className="h-9 text-sm"
                  placeholder="Ej. Empresa ABC Sociedad Anónima Cerrada"
                  value={form.razonSocial}
                  onChange={(e) => handleChange('razonSocial', e.target.value)}
                />
              </FormField>
              <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                <FormField label="RUC / ID Fiscal">
                  <Input
                    className="h-9 text-sm"
                    value={empresa?.identificacionFiscal ?? ''}
                    disabled
                    title="El RUC no se puede modificar"
                  />
                </FormField>
                <FormField label="Zona horaria" required error={errors.zonaHoraria}>
                  <select
                    className="flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
                    value={form.zonaHoraria}
                    onChange={(e) => handleChange('zonaHoraria', e.target.value)}
                  >
                    {ZONAS_HORARIAS.map((z) => (
                      <option key={z} value={z}>
                        {z}
                      </option>
                    ))}
                  </select>
                </FormField>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-sm font-semibold">Personalización (opcional)</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 p-3 pt-0">
              <FormField label="URL del logo" optional>
                <Input
                  className="h-9 text-sm"
                  placeholder="https://empresa.com/logo.png"
                  value={form.logoUrl}
                  onChange={(e) => handleChange('logoUrl', e.target.value)}
                />
              </FormField>
              <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                <FormField label="Color primario" optional error={errors.colorPrimario}>
                  <div className="flex gap-2">
                    <Input
                      className="h-9 text-sm"
                      placeholder="#2563eb"
                      value={form.colorPrimario}
                      onChange={(e) => handleChange('colorPrimario', e.target.value)}
                    />
                    {form.colorPrimario && /^#[0-9A-Fa-f]{6}$/.test(form.colorPrimario) && (
                      <div
                        className="h-9 w-9 shrink-0 rounded-md border"
                        style={{ backgroundColor: form.colorPrimario }}
                      />
                    )}
                  </div>
                </FormField>
                <FormField label="Color secundario" optional error={errors.colorSecundario}>
                  <div className="flex gap-2">
                    <Input
                      className="h-9 text-sm"
                      placeholder="#64748b"
                      value={form.colorSecundario}
                      onChange={(e) => handleChange('colorSecundario', e.target.value)}
                    />
                    {form.colorSecundario && /^#[0-9A-Fa-f]{6}$/.test(form.colorSecundario) && (
                      <div
                        className="h-9 w-9 shrink-0 rounded-md border"
                        style={{ backgroundColor: form.colorSecundario }}
                      />
                    )}
                  </div>
                </FormField>
              </div>
            </CardContent>
          </Card>

          <div className="flex items-center justify-end gap-2 pb-2">
            <Button variant="outline" onClick={() => navigate(backPath)}>
              Cancelar
            </Button>
            <Button disabled={actualizarEmpresa.isPending} onClick={handleSubmit}>
              {actualizarEmpresa.isPending ? 'Guardando...' : 'Guardar cambios'}
            </Button>
          </div>
        </div>

        {/* Panel de resumen */}
        <div className="w-full lg:w-72 lg:shrink-0">
          <Card className="lg:sticky lg:top-4">
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-sm font-semibold">Vista previa</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4 p-3 pt-0">
              <div className="flex flex-col items-center gap-3 py-2">
                <div className="flex h-14 w-14 items-center justify-center rounded-xl bg-primary/10 text-primary">
                  <Building2 className="h-7 w-7" />
                </div>
                <div className="text-center">
                  <p className="text-sm font-semibold">
                    {form.nombreComercial.trim() || empresa?.nombreComercial || 'Nombre comercial'}
                  </p>
                  {form.razonSocial.trim() && (
                    <p className="mt-0.5 text-xs text-muted-foreground">
                      {form.razonSocial.trim()}
                    </p>
                  )}
                  {empresa?.identificacionFiscal && (
                    <p className="mt-0.5 text-xs text-muted-foreground">
                      RUC: {empresa.identificacionFiscal}
                    </p>
                  )}
                </div>
              </div>
              <div className="space-y-1.5 text-xs">
                <div className="flex justify-between gap-2">
                  <span className="text-muted-foreground">Zona horaria</span>
                  <span className="font-medium">{form.zonaHoraria}</span>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
