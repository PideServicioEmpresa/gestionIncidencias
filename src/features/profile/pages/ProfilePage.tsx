import { useRef, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import {
  Camera,
  Save,
  Lock,
  Bell,
  Moon,
  Loader2,
  Check,
  Eye,
  EyeOff,
  Palette,
  Ticket,
  ClipboardList,
  Building2,
  MapPin,
} from 'lucide-react'
import { Button } from '@shared/ui/button'
import { Input } from '@shared/ui/input'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@shared/ui/card'
import { Switch } from '@shared/ui/switch'
import { Separator } from '@shared/ui/separator'
import { Badge } from '@shared/ui/badge'
import { FormField } from '@shared/components/FormField'
import { useAuthStore } from '@store/auth.store'
import { toast } from 'sonner'
import type { UserRole } from '@types-app/index'
import { MOCK_TICKETS } from '@/mocks/data'

// ── Schemas ────────────────────────────────────────────────────────────────────

const profileSchema = z.object({
  nombre: z.string().min(2, 'Mínimo 2 caracteres'),
  apellido: z.string().min(2, 'Mínimo 2 caracteres'),
  correo: z.string().email('Ingresa un correo electronico valido.'),
  telefono: z.string().optional(),
})
type ProfileForm = z.infer<typeof profileSchema>

const passwordSchema = z
  .object({
    actual: z.string().min(1, 'Ingresa tu contraseña actual'),
    nueva: z.string().min(8, 'Mínimo 8 caracteres'),
    confirmar: z.string(),
  })
  .refine((d) => d.nueva === d.confirmar, {
    message: 'Las contraseñas no coinciden',
    path: ['confirmar'],
  })
type PasswordForm = z.infer<typeof passwordSchema>

// ── Constantes ────────────────────────────────────────────────────────────────

const ROL_COLORS: Record<UserRole, string> = {
  superadmin: 'bg-purple-500/20 text-purple-400 border border-purple-500/30',
  admin: 'bg-blue-500/20 text-blue-400 border border-blue-500/30',
  worker: 'bg-emerald-500/20 text-emerald-400 border border-emerald-500/30',
  user: 'bg-gray-500/20 text-gray-300 border border-gray-500/40',
}

const ROL_LABEL: Record<UserRole, string> = {
  superadmin: 'SuperAdministrador',
  admin: 'Administrador',
  worker: 'Trabajador',
  user: 'Usuario',
}

// ── Componente principal ───────────────────────────────────────────────────────

export function ProfilePage() {
  const user = useAuthStore((s) => s.user)

  // ── Estado ─────────────────────────────────────────────────────────────────
  const [savingProfile, setSavingProfile] = useState(false)
  const [profileSaved, setProfileSaved] = useState(false)
  const [savingPw, setSavingPw] = useState(false)
  const [photoPreview, setPhotoPreview] = useState<string | null>(null)

  // Visibilidad de contraseñas
  const [showActual, setShowActual] = useState(false)
  const [showNueva, setShowNueva] = useState(false)
  const [showConfirmar, setShowConfirmar] = useState(false)

  const [notifications, setNotifications] = useState({
    email: true,
    push: true,
    assignments: true,
    statusChanges: true,
    comments: false,
  })
  const [accentColor, setAccentColor] = useState<'blue' | 'violet' | 'green' | 'orange'>('blue')

  // Ref para input file oculto
  const fileInputRef = useRef<HTMLInputElement>(null)

  // ── Formularios ────────────────────────────────────────────────────────────
  const {
    register: regProfile,
    handleSubmit: handleProfile,
    formState: { errors: profileErrors },
  } = useForm<ProfileForm>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      nombre: user?.nombre ?? '',
      apellido: user?.apellido ?? '',
      correo: user?.correo ?? '',
      telefono: '',
    },
  })

  const {
    register: regPw,
    handleSubmit: handlePw,
    reset: resetPw,
    formState: { errors: pwErrors },
  } = useForm<PasswordForm>({ resolver: zodResolver(passwordSchema) })

  // ── Estadísticas calculadas desde mocks ───────────────────────────────────
  const userId = user?.id ?? ''
  const ticketsCreados = userId ? MOCK_TICKETS.filter((t) => t.createdBy.id === userId).length : 0
  const ticketsAsignados = userId
    ? MOCK_TICKETS.filter((t) => t.assignedTo?.id === userId).length
    : 0

  // ── Handlers ───────────────────────────────────────────────────────────────
  const onSaveProfile = (_data: ProfileForm) => {
    setSavingProfile(true)
    setTimeout(() => {
      setSavingProfile(false)
      setProfileSaved(true)
      toast.success('Datos personales guardados correctamente')
      setTimeout(() => setProfileSaved(false), 3000)
    }, 1000)
  }

  const onSavePw = (data: PasswordForm) => {
    if (!data.actual || !data.nueva || !data.confirmar) {
      toast.error('Todos los campos de contraseña son obligatorios')
      return
    }
    if (data.nueva.length < 8) {
      toast.error('La nueva contraseña debe tener al menos 8 caracteres')
      return
    }
    if (data.nueva !== data.confirmar) {
      toast.error('La nueva contraseña y la confirmación no coinciden')
      return
    }
    setSavingPw(true)
    setTimeout(() => {
      setSavingPw(false)
      resetPw()
      toast.success('Contraseña actualizada correctamente')
    }, 1000)
  }

  const handleAvatarClick = () => {
    fileInputRef.current?.click()
  }

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    const reader = new FileReader()
    reader.onload = (ev) => {
      const result = ev.target?.result
      if (typeof result === 'string') {
        setPhotoPreview(result)
        toast.success('Foto de perfil actualizada')
      }
    }
    reader.readAsDataURL(file)
    // Limpiar el input para permitir seleccionar el mismo archivo nuevamente
    e.target.value = ''
  }

  const initials = user
    ? `${user.nombre.charAt(0)}${user.apellido?.charAt(0) ?? ''}`.toUpperCase()
    : '??'

  // Paleta de colores de acento
  const ACCENT_OPTIONS: {
    id: 'blue' | 'violet' | 'green' | 'orange'
    label: string
    ring: string
    dot: string
  }[] = [
    { id: 'blue', label: 'Azul', ring: 'ring-blue-500', dot: 'bg-blue-500' },
    { id: 'violet', label: 'Violeta', ring: 'ring-violet-500', dot: 'bg-violet-500' },
    { id: 'green', label: 'Verde', ring: 'ring-emerald-500', dot: 'bg-emerald-500' },
    { id: 'orange', label: 'Naranja', ring: 'ring-orange-500', dot: 'bg-orange-500' },
  ]

  // ── Render ─────────────────────────────────────────────────────────────────
  return (
    <div className="p-3 lg:p-5">
      {/* Input file oculto — único en el DOM */}
      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        className="hidden"
        onChange={handleFileChange}
      />

      {/* Header de página */}
      <div className="mb-4">
        <h2 className="text-base font-semibold tracking-tight">Mi perfil</h2>
        <p className="text-xs text-muted-foreground">
          Gestiona tu información personal y preferencias.
        </p>
      </div>

      {/* Layout: mobile = columna única, desktop = 2 columnas */}
      <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:gap-6">
        {/* ── COLUMNA IZQUIERDA (sidebar fijo en desktop) ──────────────────── */}
        <div className="w-full shrink-0 lg:w-72 xl:w-80 2xl:w-96">
          <Card className="lg:sticky lg:top-4">
            <CardContent className="p-4">
              {/* Avatar grande */}
              <div className="flex flex-col items-center gap-3 text-center">
                <div className="relative">
                  <button
                    type="button"
                    onClick={handleAvatarClick}
                    className="group relative flex h-24 w-24 cursor-pointer items-center justify-center overflow-hidden rounded-full bg-primary/20 text-xl font-bold text-primary ring-2 ring-transparent transition-all hover:ring-primary/40 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary"
                    title="Cambiar foto de perfil"
                  >
                    {photoPreview ? (
                      <img
                        src={photoPreview}
                        alt="Foto de perfil"
                        className="h-full w-full object-cover"
                      />
                    ) : (
                      initials
                    )}
                    {/* Overlay al hover */}
                    <span className="absolute inset-0 flex items-center justify-center rounded-full bg-black/40 opacity-0 transition-opacity group-hover:opacity-100">
                      <Camera className="h-6 w-6 text-white" />
                    </span>
                  </button>

                  {/* Badge de cámara */}
                  <button
                    type="button"
                    onClick={handleAvatarClick}
                    className="absolute -bottom-1 -right-1 flex h-7 w-7 items-center justify-center rounded-full bg-primary text-primary-foreground shadow-md hover:bg-primary/90 focus-visible:outline-none"
                    title="Cambiar foto"
                    tabIndex={-1}
                  >
                    <Camera className="h-3.5 w-3.5" />
                  </button>
                </div>

                {/* Nombre y correo */}
                <div>
                  <p className="text-lg font-semibold leading-tight">
                    {user?.nombre} {user?.apellido}
                  </p>
                  <p className="mt-0.5 text-xs text-muted-foreground">{user?.correo}</p>
                </div>

                {/* Badges de rol / empresa / sucursal */}
                <div className="flex flex-wrap justify-center gap-1.5">
                  {user?.rol && (
                    <Badge className={`text-[11px] ${ROL_COLORS[user.rol]}`}>
                      {ROL_LABEL[user.rol] ?? user.rol}
                    </Badge>
                  )}
                  <Badge
                    variant="outline"
                    className="border-green-500/30 bg-green-500/15 text-[11px] text-green-400"
                  >
                    Activo
                  </Badge>
                </div>

                {/* Empresa y sucursal como texto compacto */}
                <div className="flex flex-col gap-1 text-xs text-muted-foreground">
                  <span className="flex items-center justify-center gap-1">
                    <Building2 className="h-3 w-3 shrink-0" />
                    Sede Central
                  </span>
                  <span className="flex items-center justify-center gap-1">
                    <MapPin className="h-3 w-3 shrink-0" />
                    Sistemas / TI
                  </span>
                </div>

                {/* Botón cambiar foto */}
                <button
                  type="button"
                  onClick={handleAvatarClick}
                  className="text-xs text-primary underline-offset-2 hover:underline"
                >
                  Cambiar foto
                </button>
              </div>

              <Separator className="my-4" />

              {/* Estadísticas del usuario */}
              <div>
                <p className="mb-3 text-[10px] font-semibold uppercase tracking-wider text-muted-foreground">
                  Estadísticas
                </p>
                <div className="grid grid-cols-2 gap-3">
                  <div className="flex flex-col items-center gap-1 rounded-lg border border-border/50 bg-muted/30 p-3">
                    <Ticket className="h-4 w-4 text-primary" />
                    <span className="text-lg font-bold leading-none">{ticketsCreados}</span>
                    <span className="text-center text-[10px] leading-tight text-muted-foreground">
                      Tickets creados
                    </span>
                  </div>
                  <div className="flex flex-col items-center gap-1 rounded-lg border border-border/50 bg-muted/30 p-3">
                    <ClipboardList className="h-4 w-4 text-emerald-400" />
                    <span className="text-lg font-bold leading-none">{ticketsAsignados}</span>
                    <span className="text-center text-[10px] leading-tight text-muted-foreground">
                      Asignados
                    </span>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* ── COLUMNA DERECHA (contenido principal) ────────────────────────── */}
        <div className="min-w-0 flex-1 space-y-4">
          {/* Card: Información personal */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                Información personal
              </CardTitle>
            </CardHeader>
            <CardContent className="p-3 pt-0">
              <form onSubmit={handleProfile(onSaveProfile)} className="space-y-3">
                <div className="grid gap-3 sm:grid-cols-2">
                  <FormField label="Nombre completo" required error={profileErrors.nombre?.message}>
                    <Input id="nombre" className="h-8 text-xs" {...regProfile('nombre')} />
                  </FormField>
                  <FormField
                    label="Apellido completo"
                    required
                    error={profileErrors.apellido?.message}
                  >
                    <Input id="apellido" className="h-8 text-xs" {...regProfile('apellido')} />
                  </FormField>
                </div>

                <FormField
                  label="Correo electrónico"
                  required
                  error={profileErrors.correo?.message}
                >
                  <Input
                    id="correo"
                    type="email"
                    className="h-8 text-xs"
                    {...regProfile('correo')}
                  />
                </FormField>

                <FormField label="Teléfono" optional>
                  <Input
                    id="telefono"
                    type="tel"
                    placeholder="+51 999 000 000"
                    className="h-8 text-xs"
                    {...regProfile('telefono')}
                  />
                </FormField>

                {/* Info de solo lectura */}
                <Separator />
                <div className="grid gap-3 sm:grid-cols-2">
                  <div>
                    <p className="text-[10px] font-medium uppercase tracking-wide text-muted-foreground">
                      Empresa
                    </p>
                    <p className="text-xs font-medium">Sede Central</p>
                  </div>
                  <div>
                    <p className="text-[10px] font-medium uppercase tracking-wide text-muted-foreground">
                      Sucursal
                    </p>
                    <p className="text-xs font-medium">Sistemas / TI</p>
                  </div>
                  <div>
                    <p className="text-[10px] font-medium uppercase tracking-wide text-muted-foreground">
                      Usuario
                    </p>
                    <p className="text-xs font-medium">{user?.usuario ?? 'N/A'}</p>
                  </div>
                </div>

                <div className="flex justify-end">
                  <Button type="submit" disabled={savingProfile} className="h-8 min-w-28 text-xs">
                    {savingProfile ? (
                      <>
                        <Loader2 className="mr-1.5 h-3.5 w-3.5 animate-spin" />
                        Guardando...
                      </>
                    ) : profileSaved ? (
                      <>
                        <Check className="mr-1.5 h-3.5 w-3.5" />
                        Guardado
                      </>
                    ) : (
                      <>
                        <Save className="mr-1.5 h-3.5 w-3.5" />
                        Guardar
                      </>
                    )}
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>

          {/* Card: Seguridad */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                <Lock className="h-3.5 w-3.5" />
                Seguridad
              </CardTitle>
              <CardDescription className="text-xs">
                Tu contraseña debe tener al menos 8 caracteres.
              </CardDescription>
            </CardHeader>
            <CardContent className="p-3 pt-0">
              <form onSubmit={handlePw(onSavePw)} className="space-y-3">
                {/* Contraseña actual */}
                <FormField label="Contraseña actual" required error={pwErrors.actual?.message}>
                  <div className="relative">
                    <Input
                      id="actual"
                      type={showActual ? 'text' : 'password'}
                      className="h-8 pr-9 text-xs"
                      {...regPw('actual')}
                    />
                    <button
                      type="button"
                      tabIndex={-1}
                      onClick={() => setShowActual((v) => !v)}
                      className="absolute right-2.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground focus-visible:outline-none"
                    >
                      {showActual ? (
                        <EyeOff className="h-3.5 w-3.5" />
                      ) : (
                        <Eye className="h-3.5 w-3.5" />
                      )}
                    </button>
                  </div>
                </FormField>

                {/* Nueva contraseña */}
                <FormField label="Nueva contraseña" required error={pwErrors.nueva?.message}>
                  <div className="relative">
                    <Input
                      id="nueva"
                      type={showNueva ? 'text' : 'password'}
                      className="h-8 pr-9 text-xs"
                      {...regPw('nueva')}
                    />
                    <button
                      type="button"
                      tabIndex={-1}
                      onClick={() => setShowNueva((v) => !v)}
                      className="absolute right-2.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground focus-visible:outline-none"
                    >
                      {showNueva ? (
                        <EyeOff className="h-3.5 w-3.5" />
                      ) : (
                        <Eye className="h-3.5 w-3.5" />
                      )}
                    </button>
                  </div>
                </FormField>

                {/* Confirmar contraseña */}
                <FormField
                  label="Confirmar contraseña"
                  required
                  error={pwErrors.confirmar?.message}
                >
                  <div className="relative">
                    <Input
                      id="confirmar"
                      type={showConfirmar ? 'text' : 'password'}
                      className="h-8 pr-9 text-xs"
                      {...regPw('confirmar')}
                    />
                    <button
                      type="button"
                      tabIndex={-1}
                      onClick={() => setShowConfirmar((v) => !v)}
                      className="absolute right-2.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground focus-visible:outline-none"
                    >
                      {showConfirmar ? (
                        <EyeOff className="h-3.5 w-3.5" />
                      ) : (
                        <Eye className="h-3.5 w-3.5" />
                      )}
                    </button>
                  </div>
                </FormField>

                <div className="flex justify-end">
                  <Button
                    type="submit"
                    variant="outline"
                    disabled={savingPw}
                    className="h-8 min-w-36 text-xs"
                  >
                    {savingPw ? (
                      <>
                        <Loader2 className="mr-1.5 h-3.5 w-3.5 animate-spin" />
                        Actualizando...
                      </>
                    ) : (
                      'Cambiar contraseña'
                    )}
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>

          {/* Card: Apariencia */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                <Palette className="h-3.5 w-3.5" />
                Apariencia
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4 p-3 pt-0">
              {/* Tema actual */}
              <div className="flex items-center gap-3 rounded-md border border-border/50 bg-muted/30 px-3 py-2">
                <Moon className="h-4 w-4 shrink-0 text-muted-foreground" />
                <div className="min-w-0">
                  <p className="text-xs font-medium">Tema oscuro</p>
                  <p className="text-[10px] text-muted-foreground">
                    Predeterminado · único modo disponible
                  </p>
                </div>
                <Badge
                  variant="outline"
                  className="ml-auto shrink-0 border-muted-foreground/30 text-[10px] text-muted-foreground"
                >
                  Activo
                </Badge>
              </div>

              {/* Selector de color de acento */}
              <div>
                <p className="mb-2 text-xs font-medium">Color de acento</p>
                <div className="flex gap-2">
                  {ACCENT_OPTIONS.map((opt) => (
                    <button
                      key={opt.id}
                      type="button"
                      title={opt.label}
                      onClick={() => {
                        setAccentColor(opt.id)
                        toast.success(`Color de acento: ${opt.label}`)
                      }}
                      className={`flex h-7 w-7 items-center justify-center rounded-full ring-2 ring-offset-2 ring-offset-background transition-all focus-visible:outline-none ${
                        accentColor === opt.id ? opt.ring : 'ring-transparent hover:ring-border'
                      }`}
                    >
                      <span className={`h-4 w-4 rounded-full ${opt.dot}`} />
                      <span className="sr-only">{opt.label}</span>
                    </button>
                  ))}
                </div>
                <p className="mt-1.5 text-[10px] text-muted-foreground">
                  Afecta botones, enlaces e indicadores de estado.
                </p>
              </div>
            </CardContent>
          </Card>

          {/* Card: Notificaciones */}
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <CardTitle className="flex items-center gap-2 text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
                <Bell className="h-3.5 w-3.5" />
                Notificaciones
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-3 p-3 pt-0">
              {[
                {
                  id: 'email',
                  label: 'Notificaciones por correo',
                  description: 'Recibe un email cuando cambie el estado de tus tickets',
                },
                {
                  id: 'push',
                  label: 'Notificaciones push',
                  description: 'Alertas en tiempo real en el navegador',
                },
                {
                  id: 'assignments',
                  label: 'Nuevas asignaciones',
                  description: 'Cuando te asignen un ticket',
                },
                {
                  id: 'statusChanges',
                  label: 'Cambios de estado',
                  description: 'Actualizaciones de tickets que sigues',
                },
                {
                  id: 'comments',
                  label: 'Comentarios',
                  description: 'Cuando alguien comente en tus tickets',
                },
              ].map((pref) => (
                <div key={pref.id} className="flex items-center justify-between gap-4">
                  <div className="min-w-0 flex-1">
                    <p className="text-xs font-medium">{pref.label}</p>
                    <p className="text-xs text-muted-foreground">{pref.description}</p>
                  </div>
                  <Switch
                    checked={notifications[pref.id as keyof typeof notifications]}
                    onCheckedChange={(v) => {
                      setNotifications((prev) => ({ ...prev, [pref.id]: v }))
                      toast.success('Preferencia guardada')
                    }}
                  />
                </div>
              ))}
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
