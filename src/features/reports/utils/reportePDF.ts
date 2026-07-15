import jsPDF from 'jspdf'
import autoTable from 'jspdf-autotable'

// ─── Paleta ──────────────────────────────────────────────────────────────────
const C_AZUL: [number, number, number] = [37, 99, 235]
const C_TEXTO: [number, number, number] = [17, 24, 39]
const C_GRIS: [number, number, number] = [107, 114, 128]
const C_FONDO: [number, number, number] = [248, 250, 252]

// ─── Tipos ───────────────────────────────────────────────────────────────────
export interface FiltrosPDF {
  desde?: string
  hasta?: string
  empresa?: string
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
  autoTable(doc, {
    startY: y,
    head: [columnas],
    body: filas,
    styles: { fontSize: 8, cellPadding: 3, textColor: C_TEXTO },
    headStyles: { fillColor: C_AZUL, textColor: [255, 255, 255], fontSize: 8, fontStyle: 'bold' },
    alternateRowStyles: { fillColor: C_FONDO },
    columnStyles: {},
    margin: { left: 14, right: 14 },
  })
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return (doc as any).lastAutoTable.finalY + 8
}

function etiquetaFiltros(filtros: FiltrosPDF): string {
  const partes: string[] = []
  if (filtros.desde) partes.push(`Desde: ${fechaLegible(filtros.desde)}`)
  if (filtros.hasta) partes.push(`Hasta: ${fechaLegible(filtros.hasta)}`)
  if (filtros.empresa && filtros.empresa !== 'all') partes.push(`Empresa: ${filtros.empresa}`)
  if (filtros.estado && filtros.estado !== 'all') partes.push(`Estado: ${filtros.estado}`)
  return partes.length ? partes.join('   |   ') : 'Sin filtros aplicados'
}

// ─── Datos placeholder (reflejo de los gráficos de la vista) ─────────────────
const DATA_POR_EMPRESA = [
  { sucursal: 'Sede Central', resueltos: 7, pendientes: 5 },
  { sucursal: 'Sucursal Norte', resueltos: 3, pendientes: 2 },
  { sucursal: 'Sucursal Sur', resueltos: 2, pendientes: 3 },
]

const DATA_POR_TIPO = [
  { tipo: 'Incidencia Hardware', cantidad: 5 },
  { tipo: 'Incidencia Software', cantidad: 4 },
  { tipo: 'Incidencia Red', cantidad: 3 },
  { tipo: 'Mantenimiento', cantidad: 3 },
  { tipo: 'Solicitud de servicio', cantidad: 2 },
  { tipo: 'Riesgo', cantidad: 1 },
]

const DATA_POR_PRIORIDAD = [
  { nombre: 'Baja', cantidad: 2 },
  { nombre: 'Media', cantidad: 4 },
  { nombre: 'Alta', cantidad: 5 },
  { nombre: 'Crítica', cantidad: 4 },
]

const DATA_POR_ESTADO = [
  { nombre: 'Sin asignar', cantidad: 3 },
  { nombre: 'Asignado', cantidad: 2 },
  { nombre: 'En proceso', cantidad: 4 },
  { nombre: 'Pend. validación', cantidad: 2 },
  { nombre: 'Cerrado', cantidad: 4 },
]

const DATA_TENDENCIA = [
  { semana: 'Semana 1', tickets: 3 },
  { semana: 'Semana 2', tickets: 5 },
  { semana: 'Semana 3', tickets: 4 },
  { semana: 'Semana 4', tickets: 7 },
  { semana: 'Semana 5', tickets: 6 },
  { semana: 'Semana 6', tickets: 9 },
]

// ─── Exportar datos generales ─────────────────────────────────────────────────
export function exportarDatosGeneralesPDF(filtros: FiltrosPDF = {}): void {
  const doc = new jsPDF({ format: 'a4', unit: 'mm' })
  let y = crearEncabezado(doc, 'Exportación de datos generales', etiquetaFiltros(filtros))

  const totalTickets = DATA_POR_EMPRESA.reduce((s, r) => s + r.resueltos + r.pendientes, 0)
  const totalResueltos = DATA_POR_EMPRESA.reduce((s, r) => s + r.resueltos, 0)
  const totalPendientes = DATA_POR_EMPRESA.reduce((s, r) => s + r.pendientes, 0)

  // Tarjetas de resumen
  const tarjetas = [
    ['Total de tickets', String(totalTickets)],
    ['Resueltos', String(totalResueltos)],
    ['Pendientes', String(totalPendientes)],
    ['Tasa de resolución', `${Math.round((totalResueltos / totalTickets) * 100)}%`],
  ]
  const cw = (196 - 14) / tarjetas.length
  tarjetas.forEach(([etiq, val], i) => {
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
  y += 22

  y = seccion(doc, 'Tickets por empresa / sucursal', y)
  y = tablaBase(
    doc,
    y,
    ['Empresa / Sucursal', 'Resueltos', 'Pendientes', 'Total'],
    DATA_POR_EMPRESA.map((r) => [
      r.sucursal,
      r.resueltos,
      r.pendientes,
      r.resueltos + r.pendientes,
    ]),
  )

  y = seccion(doc, 'Tickets por tipo de servicio', y)
  y = tablaBase(
    doc,
    y,
    ['Tipo de servicio', 'Cantidad', '% del total'],
    DATA_POR_TIPO.map((r) => [
      r.tipo,
      r.cantidad,
      `${Math.round((r.cantidad / totalTickets) * 100)}%`,
    ]),
  )

  y = seccion(doc, 'Distribución por prioridad', y)
  y = tablaBase(
    doc,
    y,
    ['Prioridad', 'Cantidad', '% del total'],
    DATA_POR_PRIORIDAD.map((r) => [
      r.nombre,
      r.cantidad,
      `${Math.round((r.cantidad / totalTickets) * 100)}%`,
    ]),
  )

  y = seccion(doc, 'Distribución por estado', y)
  y = tablaBase(
    doc,
    y,
    ['Estado', 'Cantidad', '% del total'],
    DATA_POR_ESTADO.map((r) => [
      r.nombre,
      r.cantidad,
      `${Math.round((r.cantidad / totalTickets) * 100)}%`,
    ]),
  )

  y = seccion(doc, 'Tendencia semanal de tickets registrados', y)
  tablaBase(
    doc,
    y,
    ['Período', 'Tickets registrados'],
    DATA_TENDENCIA.map((r) => [r.semana, r.tickets]),
  )

  piePagina(doc)
  doc.save(`pide-servicio-datos-${new Date().toISOString().slice(0, 10)}.pdf`)
}

// ─── Reporte mensual de tickets ───────────────────────────────────────────────
export function exportarReporteMensualPDF(): void {
  const doc = new jsPDF({ format: 'a4', unit: 'mm' })
  const mes = new Date().toLocaleDateString('es-PE', { month: 'long', year: 'numeric' })
  let y = crearEncabezado(doc, 'Reporte mensual de tickets', `Período: ${mes}`)

  const totalTickets = 18
  const cerrados = 13
  const pendientes = 5

  const tarjetas = [
    ['Registrados', String(totalTickets)],
    ['Cerrados', String(cerrados)],
    ['Pendientes', String(pendientes)],
    ['% Resolución', `${Math.round((cerrados / totalTickets) * 100)}%`],
  ]
  const cw = (196 - 14) / tarjetas.length
  tarjetas.forEach(([etiq, val], i) => {
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
  y += 22

  y = seccion(doc, 'Resumen por tipo de servicio', y)
  y = tablaBase(
    doc,
    y,
    ['Tipo de servicio', 'Registrados', 'Cerrados', 'Pendientes'],
    [
      ['Incidencia Hardware', 5, 4, 1],
      ['Incidencia Software', 4, 3, 1],
      ['Incidencia Red', 3, 2, 1],
      ['Mantenimiento', 3, 3, 0],
      ['Solicitud de servicio', 2, 1, 1],
      ['Riesgo', 1, 0, 1],
    ],
  )

  y = seccion(doc, 'Resumen por prioridad y estado', y)
  tablaBase(
    doc,
    y,
    ['Prioridad', 'Total', 'Cerrados', 'En proceso', 'Pendientes'],
    [
      ['Crítica', 4, 2, 1, 1],
      ['Alta', 5, 4, 1, 0],
      ['Media', 4, 3, 0, 1],
      ['Baja', 2, 2, 0, 0],
    ],
  )

  piePagina(doc)
  doc.save(`reporte-mensual-${new Date().toISOString().slice(0, 7)}.pdf`)
}

// ─── Rendimiento por trabajador ───────────────────────────────────────────────
export function exportarRendimientoPDF(): void {
  const doc = new jsPDF({ format: 'a4', unit: 'mm' })
  let y = crearEncabezado(
    doc,
    'Rendimiento por trabajador',
    'Métricas de resolución por técnico asignado',
  )

  y = seccion(doc, 'Indicadores por trabajador', y)
  y = tablaBase(
    doc,
    y,
    ['Trabajador', 'Asignados', 'Resueltos', 'Pendientes', '% Cierre', 'Tiempo prom.'],
    [
      ['Carlos Mendoza', 8, 7, 1, '88%', '4.2 h'],
      ['Ana Torres', 6, 5, 1, '83%', '3.8 h'],
      ['Luis Ramos', 5, 4, 1, '80%', '5.1 h'],
      ['María Quispe', 4, 3, 1, '75%', '6.0 h'],
    ],
  )

  y = seccion(doc, 'Distribución de tickets asignados por sucursal', y)
  tablaBase(
    doc,
    y,
    ['Sucursal', 'Trabajador', 'Tickets asignados'],
    [
      ['Sede Central', 'Carlos Mendoza', 5],
      ['Sede Central', 'Ana Torres', 3],
      ['Sucursal Norte', 'Luis Ramos', 4],
      ['Sucursal Sur', 'María Quispe', 3],
      ['Sucursal Sur', 'Ana Torres', 2],
    ],
  )

  piePagina(doc)
  doc.save(`rendimiento-trabajadores-${new Date().toISOString().slice(0, 10)}.pdf`)
}

// ─── Análisis de incidencias críticas ────────────────────────────────────────
export function exportarIncidenciasCriticasPDF(): void {
  const doc = new jsPDF({ format: 'a4', unit: 'mm', orientation: 'landscape' })
  let y = crearEncabezado(
    doc,
    'Análisis de incidencias críticas',
    'Tickets con prioridad CRÍTICA — tiempo de respuesta y resolución',
  )

  y = seccion(doc, 'Detalle de incidencias críticas', y)
  y = tablaBase(
    doc,
    y,
    ['Código', 'Título', 'Área', 'Técnico', 'Estado', 'Registrado', 'Resuelto', 'Tiempo'],
    [
      [
        'PS-000003',
        'Servidor caído en sede central',
        'Sistemas',
        'Carlos Mendoza',
        'Cerrado',
        '01/06/2026',
        '01/06/2026',
        '2.5 h',
      ],
      [
        'PS-000007',
        'Caída del sistema de facturación',
        'Contabilidad',
        'Ana Torres',
        'Cerrado',
        '05/06/2026',
        '06/06/2026',
        '18 h',
      ],
      [
        'PS-000012',
        'Red inoperativa sucursal sur',
        'Redes',
        'Luis Ramos',
        'En proceso',
        '10/06/2026',
        '—',
        '—',
      ],
      [
        'PS-000015',
        'Pérdida de datos en base de datos',
        'Sistemas',
        'Carlos Mendoza',
        'Cerrado',
        '14/06/2026',
        '14/06/2026',
        '4 h',
      ],
    ],
  )

  y = seccion(doc, 'Resumen de tiempos de respuesta', y)
  tablaBase(
    doc,
    y,
    ['Métrica', 'Valor'],
    [
      ['Total incidencias críticas', '4'],
      ['Incidencias resueltas', '3'],
      ['Incidencias pendientes', '1'],
      ['Tiempo promedio de resolución', '8.2 horas'],
      ['Incidencia más rápida', '2.5 horas (PS-000003)'],
      ['Incidencia más lenta', '18 horas (PS-000007)'],
    ],
  )

  piePagina(doc)
  doc.save(`incidencias-criticas-${new Date().toISOString().slice(0, 10)}.pdf`)
}

// ─── Índice de cierre por sucursal ────────────────────────────────────────────
export function exportarCierrePorSucursalPDF(): void {
  const doc = new jsPDF({ format: 'a4', unit: 'mm' })
  let y = crearEncabezado(
    doc,
    'Índice de cierre por sucursal',
    'Porcentaje de tickets cerrados por empresa y sucursal',
  )

  y = seccion(doc, 'Cierre por sucursal', y)
  y = tablaBase(
    doc,
    y,
    ['Sucursal', 'Total', 'Cerrados', 'En proceso', 'Pendientes', '% Cierre'],
    [
      ['Sede Central', 12, 7, 3, 2, '58%'],
      ['Sucursal Norte', 5, 3, 1, 1, '60%'],
      ['Sucursal Sur', 5, 2, 2, 1, '40%'],
      ['TOTAL', 22, 12, 6, 4, '55%'],
    ],
  )

  y = seccion(doc, 'Cierre por tipo de servicio y sucursal', y)
  tablaBase(
    doc,
    y,
    ['Tipo de servicio', 'Sede Central', 'Suc. Norte', 'Suc. Sur', 'Total cerrados'],
    [
      ['Incidencia Hardware', '4/5', '2/2', '1/2', '7/9'],
      ['Incidencia Software', '2/3', '1/1', '0/1', '3/5'],
      ['Incidencia Red', '1/2', '0/1', '1/1', '2/4'],
      ['Mantenimiento', '0/1', '0/0', '0/1', '0/2'],
      ['Solicitud', '0/1', '0/1', '0/0', '0/2'],
    ],
  )

  piePagina(doc)
  doc.save(`cierre-por-sucursal-${new Date().toISOString().slice(0, 10)}.pdf`)
}
