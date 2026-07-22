import ExcelJS from 'exceljs'
import type { DashboardResumenDto } from '@features/dashboard/hooks/useDashboard'
import type { FiltrosPDF } from './reportePDF'

// ─── Paleta ───────────────────────────────────────────────────────────────────
const C_AZUL = 'FF2563EB'
const C_BLANCO = 'FFFFFFFF'
const C_FILA_PAR = 'FFF8FAFC'

// ─── Helpers ─────────────────────────────────────────────────────────────────
function fechaLegible(iso?: string): string {
  if (!iso) return '—'
  const [y, m, d] = iso.split('-')
  return `${d}/${m}/${y}`
}

function fechaHoy(): string {
  return new Date().toLocaleDateString('es-PE', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
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

// ─── Utilidades de formato ────────────────────────────────────────────────────
type ColDef = { header: string; key: string; width: number }

function crearHoja(wb: ExcelJS.Workbook, nombre: string, cols: ColDef[]): ExcelJS.Worksheet {
  const sheet = wb.addWorksheet(nombre)
  sheet.columns = cols.map((c) => ({ header: c.header, key: c.key, width: c.width }))

  const headerRow = sheet.getRow(1)
  headerRow.height = 18
  for (let c = 1; c <= cols.length; c++) {
    const cell = headerRow.getCell(c)
    cell.font = { bold: true, color: { argb: C_BLANCO }, size: 9, name: 'Calibri' }
    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: C_AZUL } }
    cell.alignment = { vertical: 'middle', horizontal: 'left' }
  }
  headerRow.commit()
  return sheet
}

function finalizarHoja(sheet: ExcelJS.Worksheet): void {
  const colCount = sheet.columnCount
  sheet.eachRow({ includeEmpty: false }, (row, rowNumber) => {
    if (rowNumber === 1) return
    row.height = 15
    if (rowNumber % 2 === 0) {
      for (let c = 1; c <= colCount; c++) {
        const cell = row.getCell(c)
        cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: C_FILA_PAR } }
      }
    }
    row.commit()
  })
}

// ─── Exportación principal ────────────────────────────────────────────────────
export async function exportarExcel(
  resumen: DashboardResumenDto,
  filtros: FiltrosPDF = {},
): Promise<void> {
  const wb = new ExcelJS.Workbook()
  wb.creator = 'Pide Servicio'
  wb.created = new Date()

  const total = totalGeneral(resumen)
  const cerrados = cerradosGeneral(resumen)

  // ── Hoja 1: Resumen ─────────────────────────────────────────────────────────
  {
    const sheet = crearHoja(wb, 'Resumen', [
      { header: 'Indicador', key: 'indicador', width: 30 },
      { header: 'Valor', key: 'valor', width: 20 },
    ])
    // Metadatos de filtros aplicados
    sheet.addRow({ indicador: 'Fecha de generación', valor: fechaHoy() })
    sheet.addRow({ indicador: 'Empresa', valor: filtros.empresa ?? 'Todas' })
    sheet.addRow({ indicador: 'Sucursal', valor: filtros.sucursal ?? 'Todas' })
    sheet.addRow({ indicador: 'Área', valor: filtros.area ?? 'Todas' })
    if (filtros.desde) sheet.addRow({ indicador: 'Desde', valor: fechaLegible(filtros.desde) })
    if (filtros.hasta) sheet.addRow({ indicador: 'Hasta', valor: fechaLegible(filtros.hasta) })
    sheet.addRow({})
    // KPIs
    sheet.addRow({ indicador: 'Total de tickets', valor: total })
    sheet.addRow({ indicador: 'Tickets abiertos', valor: resumen.totalAbiertos })
    sheet.addRow({ indicador: 'Tickets cerrados', valor: cerrados })
    sheet.addRow({ indicador: 'Tickets críticos (abiertos)', valor: resumen.criticos })
    sheet.addRow({ indicador: 'Cerrados hoy', valor: resumen.cerradosHoy })
    sheet.addRow({ indicador: 'Tasa de resolución', valor: `${resumen.tasaResolucionPct}%` })
    finalizarHoja(sheet)
  }

  // ── Hoja 2: Por Estado ──────────────────────────────────────────────────────
  {
    const sheet = crearHoja(wb, 'Por Estado', [
      { header: 'Estado', key: 'estado', width: 24 },
      { header: 'Cantidad', key: 'cantidad', width: 12 },
      { header: '% del total', key: 'pct', width: 14 },
    ])
    for (const e of resumen.porEstado) {
      sheet.addRow({ estado: estadoLabel(e.estado), cantidad: e.total, pct: pct(e.total, total) })
    }
    finalizarHoja(sheet)
  }

  // ── Hoja 3: Por Prioridad ───────────────────────────────────────────────────
  {
    const sheet = crearHoja(wb, 'Por Prioridad', [
      { header: 'Prioridad', key: 'prioridad', width: 16 },
      { header: 'Cantidad', key: 'cantidad', width: 12 },
      { header: '% del total', key: 'pct', width: 14 },
    ])
    for (const p of resumen.porPrioridad) {
      sheet.addRow({
        prioridad: prioridadLabel(p.prioridad),
        cantidad: p.total,
        pct: pct(p.total, total),
      })
    }
    finalizarHoja(sheet)
  }

  // ── Hoja 4: Por Sucursal ────────────────────────────────────────────────────
  {
    const sheet = crearHoja(wb, 'Por Sucursal', [
      { header: 'Sucursal', key: 'sucursal', width: 28 },
      { header: 'Total', key: 'total', width: 10 },
      { header: 'Cerrados', key: 'cerrados', width: 12 },
      { header: 'Abiertos', key: 'abiertos', width: 12 },
      { header: '% Cierre', key: 'pctCierre', width: 12 },
    ])
    for (const s of resumen.porSucursal) {
      const areasDeEsta = resumen.porArea.filter((a) => a.sucursalId === s.sucursalId)
      const c = areasDeEsta.reduce((acc, a) => acc + a.cerrados, 0)
      const ab = areasDeEsta.reduce((acc, a) => acc + a.abiertos, 0)
      sheet.addRow({
        sucursal: s.sucursalNombre,
        total: s.total,
        cerrados: c,
        abiertos: ab,
        pctCierre: pct(c, s.total),
      })
    }
    finalizarHoja(sheet)
  }

  // ── Hoja 5: Por Área ────────────────────────────────────────────────────────
  {
    const sheet = crearHoja(wb, 'Por Área', [
      { header: 'Área', key: 'area', width: 24 },
      { header: 'Sucursal', key: 'sucursal', width: 24 },
      { header: 'Abiertos', key: 'abiertos', width: 12 },
      { header: 'Cerrados', key: 'cerrados', width: 12 },
      { header: 'Total', key: 'total', width: 10 },
    ])
    for (const a of resumen.porArea) {
      const suc = resumen.porSucursal.find((s) => s.sucursalId === a.sucursalId)
      sheet.addRow({
        area: a.areaNombre,
        sucursal: suc?.sucursalNombre ?? '—',
        abiertos: a.abiertos,
        cerrados: a.cerrados,
        total: a.abiertos + a.cerrados,
      })
    }
    finalizarHoja(sheet)
  }

  // ── Hoja 6: Por Tipo de Servicio ────────────────────────────────────────────
  {
    const sheet = crearHoja(wb, 'Por Tipo de Servicio', [
      { header: 'Tipo de servicio', key: 'tipo', width: 30 },
      { header: 'Total', key: 'total', width: 10 },
      { header: '% del total', key: 'pct', width: 14 },
    ])
    for (const t of resumen.porTipoServicio) {
      sheet.addRow({ tipo: t.tipoServicioNombre, total: t.total, pct: pct(t.total, total) })
    }
    finalizarHoja(sheet)
  }

  // ── Hoja 7: Por Técnico ─────────────────────────────────────────────────────
  {
    const sheet = crearHoja(wb, 'Por Técnico', [
      { header: 'Técnico', key: 'tecnico', width: 30 },
      { header: 'Tickets asignados', key: 'tickets', width: 18 },
      { header: '% del total', key: 'pct', width: 14 },
    ])
    const totalTec = resumen.porTecnico.reduce((s, t) => s + t.total, 0)
    for (const t of [...resumen.porTecnico].sort((a, b) => b.total - a.total)) {
      sheet.addRow({ tecnico: t.tecnicoNombre, tickets: t.total, pct: pct(t.total, totalTec) })
    }
    finalizarHoja(sheet)
  }

  // ── Hoja 8: Tendencia Diaria ────────────────────────────────────────────────
  {
    const sheet = crearHoja(wb, 'Tendencia Diaria', [
      { header: 'Fecha', key: 'fecha', width: 14 },
      { header: 'Creados', key: 'creados', width: 12 },
      { header: 'Resueltos', key: 'resueltos', width: 12 },
    ])
    for (const d of resumen.tendencia16Dias) {
      sheet.addRow({ fecha: d.fecha, creados: d.creados, resueltos: d.resueltos })
    }
    finalizarHoja(sheet)
  }

  // ── Hoja 9: Tendencia Semanal ───────────────────────────────────────────────
  {
    const sheet = crearHoja(wb, 'Tendencia Semanal', [
      { header: 'Semana', key: 'semana', width: 14 },
      { header: 'Creados', key: 'creados', width: 12 },
      { header: 'Resueltos', key: 'resueltos', width: 12 },
    ])
    for (const s of resumen.tendenciaSemanal) {
      sheet.addRow({ semana: s.semana, creados: s.creados, resueltos: s.resueltos })
    }
    finalizarHoja(sheet)
  }

  // ── Descarga ─────────────────────────────────────────────────────────────────
  const buffer = await wb.xlsx.writeBuffer()
  const blob = new Blob([buffer], {
    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  })
  const url = URL.createObjectURL(blob)
  const anchor = document.createElement('a')
  anchor.href = url
  anchor.download = `pide-servicio-${new Date().toISOString().slice(0, 10)}.xlsx`
  anchor.click()
  URL.revokeObjectURL(url)
}
