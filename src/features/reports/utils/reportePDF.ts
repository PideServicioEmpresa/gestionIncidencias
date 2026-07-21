import jsPDF from 'jspdf'
import autoTable from 'jspdf-autotable'
import type { DashboardResumenDto } from '@features/dashboard/hooks/useDashboard'

// ─── Paleta ──────────────────────────────────────────────────────────────────
const C_AZUL: [number, number, number] = [37, 99, 235]
const C_TEXTO: [number, number, number] = [17, 24, 39]
const C_GRIS: [number, number, number] = [107, 114, 128]
const C_FONDO: [number, number, number] = [248, 250, 252]

// ─── Tipos ───────────────────────────────────────────────────────────────────
export interface FiltrosPDF {
  desde?: string
  hasta?: string
  empresa?: string // etiqueta de display (nombre de empresa)
  sucursal?: string // etiqueta de display (nombre de sucursal)
  estado?: string
}

// ─── Helpers ─────────────────────────────────────────────────────────────────
function fechaLegible(iso?: string): string {
  if (!iso) return ''
  const [y, m, d] = iso.split('-')
  return `${d}/${m}/${y}`
}

function fechaHoy(): string {
  return new Date().toLocaleDateString('es-PE', { day: '2-digit', month: 'long', year: 'numeric' })
}

function pct(n: number, total: number): string {
  if (!total || !n) return '0%'
  return `${Math.round((n / total) * 100)}%`
}

function totalGeneral(resumen: DashboardResumenDto): number {
  return resumen.porEstado.reduce((s, e) => s + e.total, 0)
}

function cerradosGeneral(resumen: DashboardResumenDto): number {
  return resumen.porEstado.find((e) => e.estado === 'CERRADO')?.total ?? 0
}

function estadoLabel(e: string): string {
  const map: Record<string, string> = {
    NUEVO: 'Nuevo',
    SIN_ASIGNAR: 'Sin asignar',
    ASIGNADO: 'Asignado',
    EN_PROCESO: 'En proceso',
    EN_ESPERA: 'En espera',
    PENDIENTE_VALIDACION: 'Pend. validación',
    REABIERTO: 'Reabierto',
    CERRADO: 'Cerrado',
    CANCELADO: 'Cancelado',
  }
  return map[e] ?? e
}

function prioridadLabel(p: string): string {
  const map: Record<string, string> = {
    BAJA: 'Baja',
    MEDIA: 'Media',
    ALTA: 'Alta',
    CRITICA: 'Crítica',
  }
  return map[p] ?? p
}

function sinDatos(cols: number): (string | number)[][] {
  const row: (string | number)[] = Array(cols).fill('')
  row[0] = 'Sin datos disponibles'
  return [row]
}

function crearEncabezado(doc: jsPDF, titulo: string, subtitulo?: string): number {
  doc.setFillColor(...C_AZUL)
  doc.rect(0, 0, 210, 26, 'F')

  doc.setTextColor(255, 255, 255)
  doc.setFontSize(13)
  doc.setFont('helvetica', 'bold')
  doc.text('Pide Servicio', 14, 10)

  doc.setFontSize(8)
  doc.setFont('helvetica', 'normal')
  doc.text('Sistema de gestión de solicitudes e incidencias', 14, 16)

  doc.setTextColor(191, 219, 254)
  doc.text(fechaHoy(), 196, 10, { align: 'right' })

  let y = 34
  doc.setTextColor(...C_TEXTO)
  doc.setFontSize(13)
  doc.setFont('helvetica', 'bold')
  doc.text(titulo, 14, y)

  if (subtitulo) {
    y += 6
    doc.setFontSize(8)
    doc.setFont('helvetica', 'normal')
    doc.setTextColor(...C_GRIS)
    doc.text(subtitulo, 14, y)
  }

  y += 5
  doc.setDrawColor(229, 231, 235)
  doc.setLineWidth(0.3)
  doc.line(14, y, 196, y)

  return y + 6
}

function seccion(doc: jsPDF, titulo: string, y: number): number {
  doc.setFontSize(8)
  doc.setFont('helvetica', 'bold')
  doc.setTextColor(...C_AZUL)
  doc.text(titulo.toUpperCase(), 14, y)
  return y + 2
}

function piePagina(doc: jsPDF): void {
  const total = doc.getNumberOfPages()
  for (let i = 1; i <= total; i++) {
    doc.setPage(i)
    doc.setFontSize(6.5)
    doc.setTextColor(...C_GRIS)
    doc.text(
      'Generado automáticamente por Pide Servicio · Los datos reflejan registros del sistema a la fecha de generación.',
      14,
      289,
    )
    doc.text(`Página ${i} de ${total}`, 196, 289, { align: 'right' })
  }
}

function tablaBase(
  doc: jsPDF,
  y: number,
  columnas: string[],
  filas: (string | number)[][],
): number {
  const body = filas.length > 0 ? filas : sinDatos(columnas.length)
  autoTable(doc, {
    startY: y,
    head: [columnas],
    body,
    styles: { fontSize: 8, cellPadding: 3, textColor: C_TEXTO },
    headStyles: { fillColor: C_AZUL, textColor: [255, 255, 255], fontSize: 8, fontStyle: 'bold' },
    alternateRowStyles: { fillColor: C_FONDO },
    columnStyles: {},
    margin: { left: 14, right: 14 },
  })
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return (doc as any).lastAutoTable.finalY + 8
}

function tarjetas(doc: jsPDF, y: number, items: { etiq: string; val: string }[]): number {
  const cw = (196 - 14) / items.length
  items.forEach(({ etiq, val }, i) => {
    const x = 14 + i * cw
    doc.setFillColor(...C_FONDO)
    doc.roundedRect(x, y, cw - 3, 16, 2, 2, 'F')
    doc.setFontSize(15)
    doc.setFont('helvetica', 'bold')
    doc.setTextColor(...C_AZUL)
    doc.text(val, x + (cw - 3) / 2, y + 8, { align: 'center' })
    doc.setFontSize(7)
    doc.setFont('helvetica', 'normal')
    doc.setTextColor(...C_GRIS)
    doc.text(etiq, x + (cw - 3) / 2, y + 13, { align: 'center' })
  })
  return y + 22
}

function etiquetaFiltros(filtros: FiltrosPDF): string {
  const partes: string[] = []
  if (filtros.desde) partes.push(`Desde: ${fechaLegible(filtros.desde)}`)
  if (filtros.hasta) partes.push(`Hasta: ${fechaLegible(filtros.hasta)}`)
  if (filtros.empresa) partes.push(`Empresa: ${filtros.empresa}`)
  if (filtros.sucursal) partes.push(`Sucursal: ${filtros.sucursal}`)
  if (filtros.estado && filtros.estado !== 'all') partes.push(`Estado: ${filtros.estado}`)
  return partes.length ? partes.join('   |   ') : 'Sin filtros aplicados'
}

// ─── Exportar datos generales ─────────────────────────────────────────────────
export function exportarDatosGeneralesPDF(
  resumen: DashboardResumenDto,
  filtros: FiltrosPDF = {},
): void {
  const doc = new jsPDF({ format: 'a4', unit: 'mm' })
  let y = crearEncabezado(doc, 'Exportación de datos generales', etiquetaFiltros(filtros))

  const total = totalGeneral(resumen)
  const cerrados = cerradosGeneral(resumen)
  const abiertos = resumen.totalAbiertos

  y = tarjetas(doc, y, [
    { etiq: 'Total de tickets', val: String(total) },
    { etiq: 'Cerrados', val: String(cerrados) },
    { etiq: 'Abiertos', val: String(abiertos) },
    { etiq: 'Tasa de resolución', val: `${resumen.tasaResolucionPct}%` },
  ])

  // Sucursales: cruza porSucursal + porArea para obtener cerrados/abiertos
  const filasSucursal = resumen.porSucursal.map((s) => {
    const areas = resumen.porArea.filter((a) => a.sucursalId === s.sucursalId)
    const c = areas.reduce((acc, a) => acc + a.cerrados, 0)
    const ab = areas.reduce((acc, a) => acc + a.abiertos, 0)
    return [s.sucursalNombre, c, ab, s.total]
  })
  y = seccion(doc, 'Tickets por sucursal', y)
  y = tablaBase(doc, y, ['Sucursal', 'Cerrados', 'Abiertos', 'Total'], filasSucursal)

  // Tipo de servicio
  const filasTipo = resumen.porTipoServicio.map((t) => [
    t.tipoServicioNombre,
    t.total,
    pct(t.total, total),
  ])
  y = seccion(doc, 'Tickets por tipo de servicio', y)
  y = tablaBase(doc, y, ['Tipo de servicio', 'Cantidad', '% del total'], filasTipo)

  // Prioridad
  const filasPrioridad = resumen.porPrioridad.map((p) => [
    prioridadLabel(p.prioridad),
    p.total,
    pct(p.total, total),
  ])
  y = seccion(doc, 'Distribución por prioridad', y)
  y = tablaBase(doc, y, ['Prioridad', 'Cantidad', '% del total'], filasPrioridad)

  // Estado
  const filasEstado = resumen.porEstado.map((e) => [
    estadoLabel(e.estado),
    e.total,
    pct(e.total, total),
  ])
  y = seccion(doc, 'Distribución por estado', y)
  y = tablaBase(doc, y, ['Estado', 'Cantidad', '% del total'], filasEstado)

  // Tendencia semanal
  const filasTendencia = resumen.tendenciaSemanal.map((s) => [s.semana, s.creados, s.resueltos])
  y = seccion(doc, 'Tendencia semanal', y)
  tablaBase(doc, y, ['Período', 'Tickets creados', 'Tickets resueltos'], filasTendencia)

  piePagina(doc)
  doc.save(`pide-servicio-datos-${new Date().toISOString().slice(0, 10)}.pdf`)
}

// ─── Reporte mensual de tickets ───────────────────────────────────────────────
export function exportarReporteMensualPDF(resumen: DashboardResumenDto): void {
  const doc = new jsPDF({ format: 'a4', unit: 'mm' })
  const mes = new Date().toLocaleDateString('es-PE', { month: 'long', year: 'numeric' })
  let y = crearEncabezado(doc, 'Reporte mensual de tickets', `Período: ${mes}`)

  const total = totalGeneral(resumen)
  const cerrados = cerradosGeneral(resumen)
  const abiertos = resumen.totalAbiertos

  y = tarjetas(doc, y, [
    { etiq: 'Registrados', val: String(total) },
    { etiq: 'Cerrados', val: String(cerrados) },
    { etiq: 'Abiertos', val: String(abiertos) },
    { etiq: '% Resolución', val: `${resumen.tasaResolucionPct}%` },
  ])

  const filasTipo = resumen.porTipoServicio.map((t) => [t.tipoServicioNombre, t.total])
  y = seccion(doc, 'Resumen por tipo de servicio', y)
  y = tablaBase(doc, y, ['Tipo de servicio', 'Total'], filasTipo)

  const filasPrioridad = resumen.porPrioridad.map((p) => [prioridadLabel(p.prioridad), p.total])
  y = seccion(doc, 'Resumen por prioridad', y)
  y = tablaBase(doc, y, ['Prioridad', 'Total'], filasPrioridad)

  const filasEstado = resumen.porEstado.map((e) => [estadoLabel(e.estado), e.total])
  y = seccion(doc, 'Distribución por estado', y)
  tablaBase(doc, y, ['Estado', 'Total'], filasEstado)

  piePagina(doc)
  doc.save(`reporte-mensual-${new Date().toISOString().slice(0, 7)}.pdf`)
}

// ─── Rendimiento por técnico ──────────────────────────────────────────────────
export function exportarRendimientoPDF(resumen: DashboardResumenDto): void {
  const doc = new jsPDF({ format: 'a4', unit: 'mm' })
  let y = crearEncabezado(doc, 'Rendimiento por técnico', 'Tickets asociados por técnico asignado')

  const totalTecnicos = resumen.porTecnico.reduce((s, t) => s + t.total, 0)
  const filasTecnico = resumen.porTecnico
    .sort((a, b) => b.total - a.total)
    .map((t) => [t.tecnicoNombre, t.total, pct(t.total, totalTecnicos)])

  y = seccion(doc, 'Carga por técnico', y)
  y = tablaBase(doc, y, ['Técnico', 'Tickets asignados', '% del total'], filasTecnico)

  const filasSucursal = resumen.porSucursal.map((s) => {
    const areas = resumen.porArea.filter((a) => a.sucursalId === s.sucursalId)
    const c = areas.reduce((acc, a) => acc + a.cerrados, 0)
    const ab = areas.reduce((acc, a) => acc + a.abiertos, 0)
    return [s.sucursalNombre, s.total, c, ab]
  })
  y = seccion(doc, 'Carga por sucursal', y)
  tablaBase(doc, y, ['Sucursal', 'Total', 'Cerrados', 'Abiertos'], filasSucursal)

  piePagina(doc)
  doc.save(`rendimiento-tecnicos-${new Date().toISOString().slice(0, 10)}.pdf`)
}

// ─── Resumen de tickets críticos ──────────────────────────────────────────────
export function exportarIncidenciasCriticasPDF(resumen: DashboardResumenDto): void {
  const doc = new jsPDF({ format: 'a4', unit: 'mm' })
  let y = crearEncabezado(
    doc,
    'Resumen de tickets críticos',
    'Indicadores de tickets con prioridad CRÍTICA',
  )

  const total = totalGeneral(resumen)
  const cerrados = cerradosGeneral(resumen)

  y = tarjetas(doc, y, [
    { etiq: 'Total tickets', val: String(total) },
    { etiq: 'Críticos', val: String(resumen.criticos) },
    { etiq: 'Cerrados hoy', val: String(resumen.cerradosHoy) },
    { etiq: 'Cerrados total', val: String(cerrados) },
  ])

  const filasEstado = resumen.porEstado.map((e) => [
    estadoLabel(e.estado),
    e.total,
    pct(e.total, total),
  ])
  y = seccion(doc, 'Distribución por estado', y)
  y = tablaBase(doc, y, ['Estado', 'Cantidad', '% del total'], filasEstado)

  const filasTecnico = resumen.porTecnico
    .sort((a, b) => b.total - a.total)
    .map((t) => [t.tecnicoNombre, t.total])
  y = seccion(doc, 'Carga por técnico', y)
  y = tablaBase(doc, y, ['Técnico', 'Tickets asignados'], filasTecnico)

  const filasSucursal = resumen.porSucursal.map((s) => {
    const areas = resumen.porArea.filter((a) => a.sucursalId === s.sucursalId)
    const c = areas.reduce((acc, a) => acc + a.cerrados, 0)
    const ab = areas.reduce((acc, a) => acc + a.abiertos, 0)
    return [s.sucursalNombre, s.total, c, ab]
  })
  y = seccion(doc, 'Estado por sucursal', y)
  tablaBase(doc, y, ['Sucursal', 'Total', 'Cerrados', 'Abiertos'], filasSucursal)

  piePagina(doc)
  doc.save(`incidencias-criticas-${new Date().toISOString().slice(0, 10)}.pdf`)
}

// ─── Índice de cierre por sucursal ────────────────────────────────────────────
export function exportarCierrePorSucursalPDF(resumen: DashboardResumenDto): void {
  const doc = new jsPDF({ format: 'a4', unit: 'mm' })
  let y = crearEncabezado(
    doc,
    'Índice de cierre por sucursal',
    'Porcentaje de tickets cerrados por sucursal',
  )

  const filasSucursal = resumen.porSucursal.map((s) => {
    const areas = resumen.porArea.filter((a) => a.sucursalId === s.sucursalId)
    const c = areas.reduce((acc, a) => acc + a.cerrados, 0)
    const ab = areas.reduce((acc, a) => acc + a.abiertos, 0)
    return { nombre: s.sucursalNombre, total: s.total, cerrados: c, abiertos: ab }
  })

  const totalGlobal = filasSucursal.reduce((s, r) => s + r.total, 0)
  const cerradosGlobal = filasSucursal.reduce((s, r) => s + r.cerrados, 0)
  const abiertosGlobal = filasSucursal.reduce((s, r) => s + r.abiertos, 0)

  const filasTabla: (string | number)[][] = [
    ...filasSucursal.map((r) => [
      r.nombre,
      r.total,
      r.cerrados,
      r.abiertos,
      pct(r.cerrados, r.total),
    ]),
    ['TOTAL', totalGlobal, cerradosGlobal, abiertosGlobal, pct(cerradosGlobal, totalGlobal)],
  ]

  y = seccion(doc, 'Cierre por sucursal', y)
  y = tablaBase(doc, y, ['Sucursal', 'Total', 'Cerrados', 'Abiertos', '% Cierre'], filasTabla)

  if (resumen.porArea.length > 0) {
    const filasArea = resumen.porArea.map((a) => {
      const suc = resumen.porSucursal.find((s) => s.sucursalId === a.sucursalId)
      return [
        suc?.sucursalNombre ?? '—',
        a.areaNombre,
        a.cerrados + a.abiertos,
        a.cerrados,
        a.abiertos,
        pct(a.cerrados, a.cerrados + a.abiertos),
      ]
    })
    y = seccion(doc, 'Detalle por área', y)
    tablaBase(doc, y, ['Sucursal', 'Área', 'Total', 'Cerrados', 'Abiertos', '% Cierre'], filasArea)
  }

  piePagina(doc)
  doc.save(`cierre-por-sucursal-${new Date().toISOString().slice(0, 10)}.pdf`)
}
