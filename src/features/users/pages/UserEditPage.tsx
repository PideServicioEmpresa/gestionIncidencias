import { useState, useMemo } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { toast } from 'sonner'
import { ArrowLeft, List } from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/card'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@shared/ui/select'
import { FormField } from '@shared/components/FormField'
import { MOCK_USERS, MOCK_SUCURSALES, MOCK_AREAS } from '@mocks/data'
import type { MockArea } from '@mocks/data'
import type { UserRole } from '@types-app/index'
import { userDetailPath, ROUTES } from '@constants/index'

// ── Tipos de formulario ───────────────────────────────────────────────────────

interface UserFormState {
  name: string
  apellido: string
  correo: string
  telefono: string
  rol: UserRole
  sucursalId: string
  areaId: string
  estado: 'activo' | 'inactivo'
}

// ── UserEditPage ──────────────────────────────────────────────────────────────

export function UserEditPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()

  const user = useMemo(() => MOCK_USERS.find((u) => u.id === id), [id])

  const initialForm = useMemo<UserFormState>(() => {
    if (!user) {
      return {
        name: '',
        apellido: '',
        correo: '',
        telefono: '',
        rol: 'user',
        sucursalId: '',
        areaId: '',
        estado: 'activo',
      }
    }
    return {
      name: user.name,
      apellido: user.apellido,
      correo: user.correo,
      telefono: user.telefono ?? '',
      rol: user.rol,
      sucursalId: user.sucursalId,
      areaId: user.areaId ?? '',
      estado: user.activo ? 'activo' : 'inactivo',
    }
  }, [user])

  const [form, setForm] = useState<UserFormState>(initialForm)
  const [errors, setErrors] = useState<Partial<Record<keyof UserFormState, string>>>({})

  const areasFiltered = useMemo<MockArea[]>(
    () => MOCK_AREAS.filter((a) => a.sucursalId === form.sucursalId && a.activo),
    [form.sucursalId],
  )

  function handleChange(field: keyof UserFormState, value: string) {
    setForm((prev) => ({
      ...prev,
      [field]: value,
      ...(field === 'sucursalId' ? { areaId: '' } : {}),
    }))
    setErrors((prev) => ({ ...prev, [field]: undefined }))
  }

  function validate(): boolean {
    const next: Partial<Record<keyof UserFormState, string>> = {}
    if (!form.name.trim()) next.name = 'Ingresa tu nombre completo.'
    if (!form.apellido.trim()) next.apellido = 'Ingresa tu apellido completo.'
    if (!form.correo.trim()) next.correo = 'Ingresa un correo electronico valido.'
    if (!form.rol) next.rol = 'Selecciona un rol.'
    if (!form.sucursalId) next.sucursalId = 'Selecciona una empresa.'
    setErrors(next)
    return Object.keys(next).length === 0
  }

  function handleSave() {
    if (!validate()) return
    const fullName = `${form.name.trim()} ${form.apellido.trim()}`.trim()
    toast.success(`Usuario "${fullName}" actualizado correctamente`)
    navigate(userDetailPath(id ?? ''))
  }

  // ── Usuario no encontrado ──────────────────────────────────────────────────

  if (!user) {
    return (
      <div className="px-3 py-3 lg:px-5">
        <Card>
          <CardContent className="flex flex-col items-center gap-3 py-12 text-center">
            <p className="text-sm text-muted-foreground">
              No se encontro el usuario con id <strong>{id}</strong>.
            </p>
            <Button variant="outline" size="sm" onClick={() => navigate(ROUTES.USERS)}>
              <List className="mr-2 h-4 w-4" />
              Volver al listado
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }

  // ── Render ─────────────────────────────────────────────────────────────────

  return (
    <div className="px-3 py-3 lg:px-5">
      {/* Page header */}
      <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-base font-semibold tracking-tight">Editar Usuario</h2>
          <p className="text-xs text-muted-foreground">{user.fullName}</p>
        </div>
        <div className="flex flex-wrap gap-2">
          <Button variant="outline" size="sm" onClick={() => navigate(userDetailPath(id ?? ''))}>
            Cancelar
          </Button>
          <Button variant="ghost" size="sm" onClick={() => navigate(ROUTES.USERS)}>
            <ArrowLeft className="mr-1.5 h-4 w-4" />
            Volver al listado
          </Button>
        </div>
      </div>

      {/* Form card */}
      <Card className="mx-auto max-w-xl">
        <CardHeader className="px-3 pb-2 pt-3">
          <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            Datos del usuario
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4 p-3 pt-0">
          {/* Nombre y apellido */}
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <FormField label="Nombre completo" required error={errors.name}>
              <Input
                className="h-8 text-xs"
                placeholder="Nombre"
                value={form.name}
                onChange={(e) => handleChange('name', e.target.value)}
              />
            </FormField>
            <FormField label="Apellido completo" required error={errors.apellido}>
              <Input
                className="h-8 text-xs"
                placeholder="Apellido"
                value={form.apellido}
                onChange={(e) => handleChange('apellido', e.target.value)}
              />
            </FormField>
          </div>

          {/* Correo */}
          <FormField label="Correo electronico" required error={errors.correo}>
            <Input
              className="h-8 text-xs"
              placeholder="correo@empresa.com"
              type="email"
              value={form.correo}
              onChange={(e) => handleChange('correo', e.target.value)}
            />
          </FormField>

          {/* Telefono */}
          <FormField label="Telefono" optional>
            <Input
              className="h-8 text-xs"
              placeholder="+51 999 000 000"
              value={form.telefono}
              onChange={(e) => handleChange('telefono', e.target.value)}
            />
          </FormField>

          {/* Rol y estado */}
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <FormField label="Rol" required error={errors.rol}>
              <Select value={form.rol} onValueChange={(v) => handleChange('rol', v)}>
                <SelectTrigger className="h-8 text-xs">
                  <SelectValue placeholder="Seleccionar rol" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="superadmin">SuperAdmin</SelectItem>
                  <SelectItem value="admin">Administrador</SelectItem>
                  <SelectItem value="worker">Trabajador</SelectItem>
                  <SelectItem value="user">Usuario</SelectItem>
                </SelectContent>
              </Select>
            </FormField>

            <FormField label="Estado" required>
              <Select
                value={form.estado}
                onValueChange={(v) => handleChange('estado', v as 'activo' | 'inactivo')}
              >
                <SelectTrigger className="h-8 text-xs">
                  <SelectValue placeholder="Seleccionar estado" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="activo">Activo</SelectItem>
                  <SelectItem value="inactivo">Inactivo</SelectItem>
                </SelectContent>
              </Select>
            </FormField>
          </div>

          {/* Empresa */}
          <FormField label="Empresa" required error={errors.sucursalId}>
            <Select value={form.sucursalId} onValueChange={(v) => handleChange('sucursalId', v)}>
              <SelectTrigger className="h-8 text-xs">
                <SelectValue placeholder="Seleccionar empresa" />
              </SelectTrigger>
              <SelectContent>
                {MOCK_SUCURSALES.filter((s) => s.activo).map((s) => (
                  <SelectItem key={s.id} value={s.id}>
                    {s.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </FormField>

          {/* Sucursal (area) */}
          <FormField label="Sucursal" optional>
            <Select
              value={form.areaId}
              onValueChange={(v) => handleChange('areaId', v)}
              disabled={!form.sucursalId}
            >
              <SelectTrigger className="h-8 text-xs">
                <SelectValue
                  placeholder={
                    form.sucursalId ? 'Selecciona una sucursal.' : 'Primero selecciona una empresa.'
                  }
                />
              </SelectTrigger>
              <SelectContent>
                {areasFiltered.map((a) => (
                  <SelectItem key={a.id} value={a.id}>
                    {a.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </FormField>

          {/* Footer actions */}
          <div className="flex justify-end gap-2 border-t pt-4">
            <Button variant="outline" size="sm" onClick={() => navigate(userDetailPath(id ?? ''))}>
              Cancelar
            </Button>
            <Button size="sm" onClick={handleSave}>
              Guardar cambios
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
