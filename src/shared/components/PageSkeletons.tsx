import { Skeleton } from '@shared/ui/skeleton'
import { Card, CardContent, CardHeader } from '@shared/ui/card'

// ── PageSkeleton ─────────────────────────────────────────────────────────────
// Fallback genérico mientras se carga un chunk lazy de página

export function PageSkeleton() {
  return (
    <div className="space-y-4 p-3 lg:p-4">
      <div className="space-y-1.5">
        <Skeleton className="h-4 w-40" />
        <Skeleton className="h-3 w-28" />
      </div>
      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4">
        {Array.from({ length: 4 }).map((_, i) => (
          <Card key={i} className="border-border/60">
            <CardContent className="p-3">
              <Skeleton className="h-16 w-full" />
            </CardContent>
          </Card>
        ))}
      </div>
      <Card className="border-border/60">
        <CardHeader className="px-3 pb-2 pt-3">
          <Skeleton className="h-3 w-24" />
        </CardHeader>
        <CardContent className="space-y-2 p-3 pt-0">
          {Array.from({ length: 5 }).map((_, i) => (
            <Skeleton key={i} className="h-10 w-full rounded-md" />
          ))}
        </CardContent>
      </Card>
    </div>
  )
}

// ── DashboardSkeleton ────────────────────────────────────────────────────────
// Refleja: 4 KPI cards en grid 2x2, fila de 6 status pills, grid de charts

export function DashboardSkeleton() {
  return (
    <div className="space-y-4 p-3 lg:p-4">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div className="space-y-1.5">
          <Skeleton className="h-4 w-40" />
          <Skeleton className="h-3 w-28" />
        </div>
        <Skeleton className="h-7 w-28 rounded-md" />
      </div>

      {/* KPI cards 2x2 (mobile) / 4 columnas (desktop) */}
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
        {Array.from({ length: 4 }).map((_, i) => (
          <Card key={i} className="border-border/60">
            <CardContent className="p-3">
              <div className="flex items-center justify-between">
                <Skeleton className="h-2.5 w-16" />
                <Skeleton className="h-5 w-5 rounded" />
              </div>
              <Skeleton className="mt-2 h-7 w-12" />
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Status pills row */}
      <div className="grid grid-cols-3 gap-2 lg:grid-cols-6">
        {Array.from({ length: 6 }).map((_, i) => (
          <div key={i} className="rounded-lg border border-border/50 bg-card p-2.5">
            <Skeleton className="mb-1 h-3.5 w-3.5 rounded" />
            <Skeleton className="h-5 w-8" />
            <Skeleton className="mt-1 h-2.5 w-14" />
          </div>
        ))}
      </div>

      {/* Chart grid 1x2 */}
      <div className="grid grid-cols-1 gap-3 lg:grid-cols-2">
        {Array.from({ length: 4 }).map((_, i) => (
          <Card key={i} className="border-border/60">
            <CardHeader className="px-3 pb-2 pt-3">
              <Skeleton className="h-2.5 w-24" />
              <Skeleton className="mt-1 h-2 w-36" />
            </CardHeader>
            <CardContent className="p-3 pt-0">
              <Skeleton className="h-[200px] w-full rounded-md" />
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Tickets recientes */}
      <Card>
        <CardHeader className="px-3 pb-2 pt-3">
          <Skeleton className="h-2.5 w-28" />
          <Skeleton className="mt-1 h-2 w-40" />
        </CardHeader>
        <CardContent className="p-0">
          <div className="divide-y">
            {Array.from({ length: 5 }).map((_, i) => (
              <div key={i} className="flex items-start gap-3 px-4 py-2.5">
                <div className="min-w-0 flex-1 space-y-1.5">
                  <div className="flex items-center gap-2">
                    <Skeleton className="h-3 w-16" />
                    <Skeleton className="h-4 w-10 rounded-full" />
                  </div>
                  <Skeleton className="h-3.5 w-3/4" />
                  <Skeleton className="h-2.5 w-1/2" />
                </div>
                <Skeleton className="h-5 w-20 rounded-full" />
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

// ── TicketListSkeleton ───────────────────────────────────────────────────────
// Refleja: filtros en fila + 6 ticket rows (mobile cards / desktop table)

export function TicketListSkeleton() {
  return (
    <div className="space-y-3 p-3 lg:p-4">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div className="space-y-1.5">
          <Skeleton className="h-4 w-36" />
          <Skeleton className="h-3 w-24" />
        </div>
        <Skeleton className="h-8 w-28 rounded-md" />
      </div>

      {/* Filtros */}
      <div className="flex flex-col gap-1.5 lg:flex-row">
        <Skeleton className="h-8 flex-1 rounded-md" />
        <Skeleton className="h-8 w-full rounded-md lg:w-40" />
        <Skeleton className="h-8 w-full rounded-md lg:w-36" />
        <Skeleton className="h-8 w-full rounded-md lg:w-36" />
      </div>

      {/* Mobile: card list */}
      <div className="grid gap-2 lg:hidden">
        {Array.from({ length: 6 }).map((_, i) => (
          <Card key={i} className="border-border/60">
            <CardContent className="p-3">
              <div className="flex items-start gap-3">
                {/* Avatar circle */}
                <Skeleton className="h-8 w-8 shrink-0 rounded-full" />
                <div className="min-w-0 flex-1 space-y-1.5">
                  <div className="flex items-center gap-2">
                    <Skeleton className="h-3 w-16" />
                    <Skeleton className="h-4 w-12 rounded-full" />
                  </div>
                  <Skeleton className="h-3.5 w-4/5" />
                  <Skeleton className="h-2.5 w-1/2" />
                </div>
                <Skeleton className="h-5 w-20 shrink-0 rounded-full" />
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Desktop: table skeleton */}
      <div className="hidden rounded-lg border lg:block">
        {/* Header row */}
        <div className="flex items-center gap-4 border-b px-4 py-2">
          {[60, 200, 120, 80, 80, 100, 70].map((w, i) => (
            <Skeleton key={i} className="h-2.5" style={{ width: w }} />
          ))}
        </div>
        {/* Data rows */}
        {Array.from({ length: 6 }).map((_, i) => (
          <div key={i} className="flex items-center gap-4 border-b px-4 py-2.5">
            <Skeleton className="h-3 w-16" />
            <div className="flex w-48 flex-col gap-1">
              <Skeleton className="h-3 w-full" />
              <Skeleton className="h-2.5 w-3/4" />
            </div>
            <div className="flex w-28 flex-col gap-1">
              <Skeleton className="h-3 w-full" />
              <Skeleton className="h-2.5 w-2/3" />
            </div>
            <Skeleton className="h-5 w-16 rounded-full" />
            <Skeleton className="h-5 w-20 rounded-full" />
            <div className="flex items-center gap-1.5">
              <Skeleton className="h-7 w-7 rounded-full" />
              <Skeleton className="h-3 w-20" />
            </div>
            <Skeleton className="h-3 w-16" />
          </div>
        ))}
      </div>
    </div>
  )
}

// ── TicketDetailSkeleton ─────────────────────────────────────────────────────
// Refleja: header con breadcrumb + 2 cols (desc+tabs izq, info card der)

export function TicketDetailSkeleton() {
  return (
    <div className="space-y-4 p-3 lg:p-4">
      {/* Breadcrumb / back */}
      <div className="flex items-center gap-2">
        <Skeleton className="h-7 w-7 rounded-md" />
        <Skeleton className="h-3 w-24" />
      </div>

      {/* Ticket header */}
      <div className="space-y-2">
        <div className="flex flex-wrap items-center gap-2">
          <Skeleton className="h-3 w-16" />
          <Skeleton className="h-5 w-14 rounded-full" />
          <Skeleton className="h-5 w-16 rounded-full" />
        </div>
        <Skeleton className="h-5 w-4/5" />
        <Skeleton className="h-3.5 w-3/5" />
      </div>

      {/* 2-column layout */}
      <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">
        {/* Left: description + tabs */}
        <div className="space-y-3 lg:col-span-2">
          {/* Tabs bar */}
          <div className="flex gap-2 border-b pb-1">
            {Array.from({ length: 3 }).map((_, i) => (
              <Skeleton key={i} className="h-7 w-24 rounded-md" />
            ))}
          </div>
          {/* Description block */}
          <Card>
            <CardContent className="space-y-2 p-3">
              <Skeleton className="h-3 w-full" />
              <Skeleton className="h-3 w-full" />
              <Skeleton className="h-3 w-4/5" />
              <Skeleton className="h-3 w-3/5" />
            </CardContent>
          </Card>
          {/* Comment / history items */}
          {Array.from({ length: 3 }).map((_, i) => (
            <div key={i} className="flex items-start gap-3">
              <Skeleton className="h-8 w-8 shrink-0 rounded-full" />
              <div className="flex-1 space-y-1.5">
                <div className="flex items-center gap-2">
                  <Skeleton className="h-3 w-24" />
                  <Skeleton className="h-2.5 w-16" />
                </div>
                <Skeleton className="h-3 w-full" />
                <Skeleton className="h-3 w-4/5" />
              </div>
            </div>
          ))}
        </div>

        {/* Right: info card */}
        <div className="space-y-3">
          <Card>
            <CardHeader className="px-3 pb-2 pt-3">
              <Skeleton className="h-3 w-20" />
            </CardHeader>
            <CardContent className="space-y-3 p-3 pt-0">
              {Array.from({ length: 5 }).map((_, i) => (
                <div key={i} className="space-y-1">
                  <Skeleton className="h-2.5 w-20" />
                  <Skeleton className="h-3.5 w-32" />
                </div>
              ))}
            </CardContent>
          </Card>
          {/* Action button area */}
          <Skeleton className="h-8 w-full rounded-md" />
          <Skeleton className="h-8 w-full rounded-md" />
        </div>
      </div>
    </div>
  )
}

// ── UserListSkeleton ─────────────────────────────────────────────────────────
// Refleja: 5 UserRow (avatar + nombre + rol badge + acciones)

export function UserListSkeleton() {
  return (
    <div className="space-y-3 p-3 lg:p-4">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div className="space-y-1.5">
          <Skeleton className="h-4 w-28" />
          <Skeleton className="h-3 w-20" />
        </div>
        <Skeleton className="h-8 w-32 rounded-md" />
      </div>

      {/* Search */}
      <Skeleton className="h-8 w-full rounded-md" />

      {/* User rows */}
      <div className="space-y-2">
        {Array.from({ length: 5 }).map((_, i) => (
          <Card key={i} className="border-border/60">
            <CardContent className="p-3">
              <div className="flex items-center gap-3">
                {/* Avatar */}
                <Skeleton className="h-9 w-9 shrink-0 rounded-full" />
                {/* Name + meta */}
                <div className="min-w-0 flex-1 space-y-1.5">
                  <Skeleton className="h-3.5 w-32" />
                  <Skeleton className="h-2.5 w-48" />
                </div>
                {/* Badge */}
                <Skeleton className="h-5 w-20 shrink-0 rounded-full" />
                {/* Actions */}
                <Skeleton className="h-7 w-7 shrink-0 rounded-md" />
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  )
}

// ── NotificationsSkeleton ────────────────────────────────────────────────────
// Refleja: 6 notification items (icon circle + 2 lineas de texto)

export function NotificationsSkeleton() {
  return (
    <div className="space-y-3 p-3 lg:p-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <Skeleton className="h-4 w-32" />
        <Skeleton className="h-7 w-24 rounded-md" />
      </div>

      {/* Notification items */}
      <div className="space-y-2">
        {Array.from({ length: 6 }).map((_, i) => (
          <div
            key={i}
            className="flex items-start gap-3 rounded-lg border border-border/60 bg-card p-3"
          >
            {/* Icon circle */}
            <Skeleton className="h-8 w-8 shrink-0 rounded-full" />
            {/* Content */}
            <div className="min-w-0 flex-1 space-y-1.5">
              <Skeleton className="h-3.5 w-4/5" />
              <Skeleton className="h-2.5 w-2/5" />
            </div>
            {/* Time */}
            <Skeleton className="h-2.5 w-12 shrink-0" />
          </div>
        ))}
      </div>
    </div>
  )
}

// ── ReportsSkeleton ──────────────────────────────────────────────────────────
// Refleja: filtros + 2 charts grandes + tabla de 4 filas

export function ReportsSkeleton() {
  return (
    <div className="space-y-4 p-3 lg:p-4">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div className="space-y-1.5">
          <Skeleton className="h-4 w-24" />
          <Skeleton className="h-3 w-36" />
        </div>
        <Skeleton className="h-8 w-28 rounded-md" />
      </div>

      {/* Filter row */}
      <div className="flex flex-wrap gap-2">
        <Skeleton className="h-8 w-32 rounded-md" />
        <Skeleton className="h-8 w-32 rounded-md" />
        <Skeleton className="h-8 w-24 rounded-md" />
      </div>

      {/* Charts grid */}
      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        {Array.from({ length: 2 }).map((_, i) => (
          <Card key={i} className="border-border/60">
            <CardHeader className="px-3 pb-2 pt-3">
              <Skeleton className="h-2.5 w-28" />
              <Skeleton className="mt-1 h-2 w-40" />
            </CardHeader>
            <CardContent className="p-3 pt-0">
              <Skeleton className="h-[220px] w-full rounded-md" />
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Tabla de resumen */}
      <Card>
        <CardHeader className="px-3 pb-2 pt-3">
          <Skeleton className="h-2.5 w-24" />
        </CardHeader>
        <CardContent className="p-0">
          {/* Table header */}
          <div className="flex items-center gap-6 border-b px-4 py-2">
            {[80, 100, 60, 60, 60].map((w, i) => (
              <Skeleton key={i} className="h-2.5" style={{ width: w }} />
            ))}
          </div>
          {/* Table rows */}
          {Array.from({ length: 4 }).map((_, i) => (
            <div key={i} className="flex items-center gap-6 border-b px-4 py-2.5">
              <Skeleton className="h-3 w-20" />
              <Skeleton className="h-3 w-24" />
              <Skeleton className="h-3 w-14" />
              <Skeleton className="h-3 w-14" />
              <Skeleton className="h-3 w-14" />
            </div>
          ))}
        </CardContent>
      </Card>
    </div>
  )
}

// ── ProfileSkeleton ──────────────────────────────────────────────────────────
// Refleja: avatar grande centrado + 4 campos de formulario

export function ProfileSkeleton() {
  return (
    <div className="space-y-4 p-3 lg:p-4">
      {/* Header */}
      <div className="space-y-1">
        <Skeleton className="h-4 w-20" />
        <Skeleton className="h-3 w-36" />
      </div>

      {/* Avatar grande */}
      <div className="flex flex-col items-center gap-3 py-2">
        <Skeleton className="h-20 w-20 rounded-full" />
        <Skeleton className="h-7 w-28 rounded-md" />
      </div>

      {/* Form fields */}
      <Card>
        <CardContent className="space-y-4 p-3">
          {Array.from({ length: 4 }).map((_, i) => (
            <div key={i} className="space-y-1.5">
              <Skeleton className="h-3 w-24" />
              <Skeleton className="h-8 w-full rounded-md" />
            </div>
          ))}
          <Skeleton className="h-8 w-28 rounded-md" />
        </CardContent>
      </Card>
    </div>
  )
}

// ── SettingsSkeleton ─────────────────────────────────────────────────────────
// Refleja: 3 Card sections, cada una con title + 3 toggle rows

export function SettingsSkeleton() {
  return (
    <div className="space-y-4 p-3 lg:p-4">
      {/* Header */}
      <div className="space-y-1">
        <Skeleton className="h-4 w-32" />
        <Skeleton className="h-3 w-48" />
      </div>

      {Array.from({ length: 3 }).map((_, sectionIdx) => (
        <Card key={sectionIdx} className="border-border/60">
          <CardHeader className="px-3 pb-2 pt-3">
            <Skeleton className="h-3.5 w-32" />
            <Skeleton className="mt-1 h-2.5 w-48" />
          </CardHeader>
          <CardContent className="space-y-1 p-3 pt-0">
            {Array.from({ length: 3 }).map((_, toggleIdx) => (
              <div
                key={toggleIdx}
                className="flex items-center justify-between rounded-md border border-border/40 px-3 py-2.5"
              >
                <div className="space-y-1">
                  <Skeleton className="h-3 w-36" />
                  <Skeleton className="h-2.5 w-52" />
                </div>
                {/* Toggle pill */}
                <Skeleton className="h-5 w-9 shrink-0 rounded-full" />
              </div>
            ))}
          </CardContent>
        </Card>
      ))}
    </div>
  )
}
