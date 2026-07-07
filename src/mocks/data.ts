import type { TicketStatus, TicketPriority, UserRole } from '@types-app/index'

// ── Tipos internos de Mock ────────────────────────────────────────────────────

export interface MockUser {
  id: string
  name: string
  apellido: string
  fullName: string
  initials: string
  correo: string
  usuario: string
  rol: UserRole
  rolLabel: string
  sucursalId: string
  sucursal: string
  areaId?: string
  area?: string
  activo: boolean
  telefono?: string
  avatar?: string
  lastAccess: string
}

export interface MockSucursal {
  id: string
  name: string
  address: string
  ciudad: string
  activo: boolean
}

export interface MockArea {
  id: string
  name: string
  sucursalId: string
  responsableId?: string
  activo: boolean
}

export interface MockComment {
  id: string
  text: string
  author: Pick<MockUser, 'id' | 'fullName' | 'initials' | 'rol'>
  createdAt: string
  isInternal?: boolean
}

export interface MockHistoryEntry {
  id: string
  action: string
  fromStatus?: TicketStatus
  toStatus?: TicketStatus
  description: string
  author: Pick<MockUser, 'id' | 'fullName' | 'initials'>
  createdAt: string
}

export interface MockEvidencia {
  id: string
  name: string
  type: 'imagen' | 'pdf' | 'video'
  size: string
  tipo: 'inicial' | 'final'
}

export interface MockTicket {
  id: string
  code: string
  title: string
  description: string
  status: TicketStatus
  priority: TicketPriority
  type: string
  sucursalId: string
  sucursal: string
  areaId: string
  area: string
  location?: string
  createdBy: Pick<MockUser, 'id' | 'fullName' | 'initials'>
  assignedTo?: Pick<MockUser, 'id' | 'fullName' | 'initials'>
  createdAt: string
  updatedAt: string
  closedAt?: string
  comments: MockComment[]
  history: MockHistoryEntry[]
  evidencias: MockEvidencia[]
}

export interface MockNotification {
  id: string
  title: string
  body: string
  type: 'ticket_new' | 'ticket_assigned' | 'ticket_status' | 'comment' | 'system'
  read: boolean
  ticketId?: string
  ticketCode?: string
  createdAt: string
}

// ── Sucursales ────────────────────────────────────────────────────────────────

export const MOCK_SUCURSALES: MockSucursal[] = [
  {
    id: 's1',
    name: 'Sede Central',
    address: 'Av. Libertad 1200, Piso 4',
    ciudad: 'Lima',
    activo: true,
  },
  {
    id: 's2',
    name: 'Sucursal Norte',
    address: 'Jr. Los Pinos 450',
    ciudad: 'Trujillo',
    activo: true,
  },
  { id: 's3', name: 'Sucursal Sur', address: 'Calle Real 890', ciudad: 'Arequipa', activo: true },
  {
    id: 's4',
    name: 'Sucursal Este',
    address: 'Av. Industrial 230',
    ciudad: 'Iquitos',
    activo: false,
  },
]

// ── Áreas ─────────────────────────────────────────────────────────────────────

export const MOCK_AREAS: MockArea[] = [
  { id: 'a1', name: 'Sistemas / TI', sucursalId: 's1', responsableId: 'u2', activo: true },
  { id: 'a2', name: 'Recursos Humanos', sucursalId: 's1', activo: true },
  { id: 'a3', name: 'Administración', sucursalId: 's1', activo: true },
  { id: 'a4', name: 'Ventas', sucursalId: 's1', activo: true },
  { id: 'a5', name: 'Logística', sucursalId: 's1', activo: true },
  { id: 'a6', name: 'Mantenimiento', sucursalId: 's1', responsableId: 'u4', activo: true },
  { id: 'a7', name: 'Sistemas / TI', sucursalId: 's2', activo: true },
  { id: 'a8', name: 'Administración', sucursalId: 's2', activo: true },
  { id: 'a9', name: 'Sistemas / TI', sucursalId: 's3', activo: true },
]

// ── Tipos de servicio ────────────────────────────────────────────────────────

export const TICKET_TYPES = [
  'Incidencia de Hardware',
  'Incidencia de Software',
  'Incidencia de Red',
  'Solicitud de Servicio',
  'Solicitud de Software',
  'Mantenimiento Preventivo',
  'Mantenimiento Correctivo',
  'Riesgo Operacional',
] as const

export type TicketType = (typeof TICKET_TYPES)[number]

// ── Usuarios ──────────────────────────────────────────────────────────────────

export const MOCK_USERS: MockUser[] = [
  {
    id: 'u1',
    name: 'Gian Carlo',
    apellido: 'Barrionuevo',
    fullName: 'Gian Carlo Barrionuevo',
    initials: 'GB',
    correo: 'gcbarrionuevo@empresa.com',
    usuario: 'gbarrionuevo',
    rol: 'superadmin',
    rolLabel: 'SuperAdministrador',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    activo: true,
    telefono: '+51 999 000 001',
    lastAccess: '2026-06-29T13:00:00',
  },
  {
    id: 'u2',
    name: 'Juan',
    apellido: 'Pérez',
    fullName: 'Juan Pérez',
    initials: 'JP',
    correo: 'jperez@empresa.com',
    usuario: 'jperez',
    rol: 'admin',
    rolLabel: 'Administrador',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a1',
    area: 'Sistemas / TI',
    activo: true,
    telefono: '+51 999 000 002',
    lastAccess: '2026-06-29T12:45:00',
  },
  {
    id: 'u3',
    name: 'María',
    apellido: 'López',
    fullName: 'María López',
    initials: 'ML',
    correo: 'mlopez@empresa.com',
    usuario: 'mlopez',
    rol: 'worker',
    rolLabel: 'Trabajador',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a1',
    area: 'Sistemas / TI',
    activo: true,
    telefono: '+51 999 000 003',
    lastAccess: '2026-06-29T11:30:00',
  },
  {
    id: 'u4',
    name: 'Carlos',
    apellido: 'García',
    fullName: 'Carlos García',
    initials: 'CG',
    correo: 'cgarcia@empresa.com',
    usuario: 'cgarcia',
    rol: 'worker',
    rolLabel: 'Trabajador',
    sucursalId: 's2',
    sucursal: 'Sucursal Norte',
    areaId: 'a6',
    area: 'Mantenimiento',
    activo: true,
    telefono: '+51 999 000 004',
    lastAccess: '2026-06-28T16:00:00',
  },
  {
    id: 'u5',
    name: 'Ana',
    apellido: 'Ramírez',
    fullName: 'Ana Ramírez',
    initials: 'AR',
    correo: 'aramirez@empresa.com',
    usuario: 'aramirez',
    rol: 'user',
    rolLabel: 'Usuario',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a3',
    area: 'Administración',
    activo: true,
    telefono: '+51 999 000 005',
    lastAccess: '2026-06-29T09:15:00',
  },
  {
    id: 'u6',
    name: 'Pedro',
    apellido: 'Flores',
    fullName: 'Pedro Flores',
    initials: 'PF',
    correo: 'pflores@empresa.com',
    usuario: 'pflores',
    rol: 'worker',
    rolLabel: 'Trabajador',
    sucursalId: 's3',
    sucursal: 'Sucursal Sur',
    areaId: 'a9',
    area: 'Sistemas / TI',
    activo: true,
    telefono: '+51 999 000 006',
    lastAccess: '2026-06-29T08:00:00',
  },
  {
    id: 'u7',
    name: 'Luisa',
    apellido: 'Torres',
    fullName: 'Luisa Torres',
    initials: 'LT',
    correo: 'ltorres@empresa.com',
    usuario: 'ltorres',
    rol: 'worker',
    rolLabel: 'Trabajador',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a6',
    area: 'Mantenimiento',
    activo: false,
    lastAccess: '2026-06-20T10:00:00',
  },
]

// ── Tickets ───────────────────────────────────────────────────────────────────

const u = (id: string) => {
  const user = MOCK_USERS.find((u) => u.id === id)!
  return { id: user.id, fullName: user.fullName, initials: user.initials }
}

export const MOCK_TICKETS: MockTicket[] = [
  {
    id: 't1',
    code: 'PS-0001',
    title: 'Fallo en impresora del tercer piso — RRHH',
    description:
      'La impresora Ricoh MP 2014 ubicada en el área de Recursos Humanos, piso 3, no responde a los trabajos de impresión. Se reinició el equipo dos veces sin resultados. Otros equipos de la red funcionan correctamente. El error que muestra es "Printer offline" en todos los ordenadores del área.',
    status: 'en_proceso',
    priority: 'alta',
    type: 'Incidencia de Hardware',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a2',
    area: 'Recursos Humanos',
    location: 'Piso 3 — Módulo B',
    createdBy: u('u5'),
    assignedTo: u('u3'),
    createdAt: '2026-06-27T09:00:00',
    updatedAt: '2026-06-29T10:30:00',
    comments: [
      {
        id: 'c1',
        text: 'Revisé la impresora. El problema es el driver desactualizado. Actualizaré esta tarde.',
        author: { ...u('u3'), rol: 'worker' },
        createdAt: '2026-06-28T14:20:00',
      },
      {
        id: 'c2',
        text: 'Por favor apresúrense, necesitamos imprimir contratos urgentes.',
        author: { ...u('u5'), rol: 'user' },
        createdAt: '2026-06-28T15:00:00',
      },
      {
        id: 'c3',
        text: 'Driver actualizado. Realizando pruebas de impresión ahora.',
        author: { ...u('u3'), rol: 'worker' },
        createdAt: '2026-06-29T10:25:00',
        isInternal: true,
      },
    ],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado en el sistema.',
        author: u('u5'),
        createdAt: '2026-06-27T09:00:00',
        toStatus: 'sin_asignar',
      },
      {
        id: 'h2',
        action: 'Ticket asignado',
        description: 'Asignado a María López.',
        fromStatus: 'sin_asignar',
        toStatus: 'asignado',
        author: u('u2'),
        createdAt: '2026-06-27T11:30:00',
      },
      {
        id: 'h3',
        action: 'Inicio de trabajo',
        description: 'Trabajador inició atención del ticket.',
        fromStatus: 'asignado',
        toStatus: 'en_proceso',
        author: u('u3'),
        createdAt: '2026-06-28T14:00:00',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'foto-impresora.jpg', type: 'imagen', size: '2.4 MB', tipo: 'inicial' },
      { id: 'e2', name: 'error-pantalla.png', type: 'imagen', size: '890 KB', tipo: 'inicial' },
    ],
  },
  {
    id: 't2',
    code: 'PS-0002',
    title: 'PC no enciende en área de Administración',
    description:
      'El equipo HP EliteDesk 800 del escritorio 4 no enciende. Al presionar el botón de power no hay respuesta. Revisé el cable de corriente y el tomacorriente funciona con otros equipos.',
    status: 'asignado',
    priority: 'media',
    type: 'Incidencia de Hardware',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a3',
    area: 'Administración',
    location: 'Piso 2 — Escritorio 4',
    createdBy: u('u5'),
    assignedTo: u('u3'),
    createdAt: '2026-06-28T08:30:00',
    updatedAt: '2026-06-28T09:15:00',
    comments: [],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado.',
        author: u('u5'),
        createdAt: '2026-06-28T08:30:00',
        toStatus: 'sin_asignar',
      },
      {
        id: 'h2',
        action: 'Ticket asignado',
        description: 'Asignado a María López.',
        fromStatus: 'sin_asignar',
        toStatus: 'asignado',
        author: u('u2'),
        createdAt: '2026-06-28T09:15:00',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'pc-apagada.jpg', type: 'imagen', size: '1.8 MB', tipo: 'inicial' },
    ],
  },
  {
    id: 't3',
    code: 'PS-0003',
    title: 'Solicitud de instalación de software de diseño',
    description:
      'Necesito instalar Adobe Illustrator 2025 para el proyecto de rebranding que inicia el lunes. El equipo ya tiene licencia corporativa disponible.',
    status: 'sin_asignar',
    priority: 'baja',
    type: 'Solicitud de Software',
    sucursalId: 's2',
    sucursal: 'Sucursal Norte',
    areaId: 'a8',
    area: 'Administración',
    createdBy: u('u4'),
    createdAt: '2026-06-29T07:45:00',
    updatedAt: '2026-06-29T07:45:00',
    comments: [],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado.',
        author: u('u4'),
        createdAt: '2026-06-29T07:45:00',
        toStatus: 'sin_asignar',
      },
    ],
    evidencias: [],
  },
  {
    id: 't4',
    code: 'PS-0004',
    title: 'Instalación de cámara de seguridad — Almacén',
    description:
      'El almacén de la sucursal sur necesita una cámara de seguridad adicional en la zona de carga. El punto eléctrico ya está habilitado. Se requiere coordinar con el proveedor de CCTV.',
    status: 'pendiente_validacion',
    priority: 'alta',
    type: 'Solicitud de Servicio',
    sucursalId: 's3',
    sucursal: 'Sucursal Sur',
    areaId: 'a9',
    area: 'Sistemas / TI',
    location: 'Almacén — Zona de Carga',
    createdBy: u('u6'),
    assignedTo: u('u4'),
    createdAt: '2026-06-25T10:00:00',
    updatedAt: '2026-06-28T16:00:00',
    comments: [
      {
        id: 'c1',
        text: 'Instalación completada. Cámara funcionando correctamente. Por favor validar.',
        author: { ...u('u4'), rol: 'worker' },
        createdAt: '2026-06-28T15:50:00',
      },
    ],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado.',
        author: u('u6'),
        createdAt: '2026-06-25T10:00:00',
        toStatus: 'sin_asignar',
      },
      {
        id: 'h2',
        action: 'Ticket asignado',
        description: 'Asignado a Carlos García.',
        fromStatus: 'sin_asignar',
        toStatus: 'asignado',
        author: u('u1'),
        createdAt: '2026-06-25T11:00:00',
      },
      {
        id: 'h3',
        action: 'Inicio de trabajo',
        description: 'Trabajador inició atención.',
        fromStatus: 'asignado',
        toStatus: 'en_proceso',
        author: u('u4'),
        createdAt: '2026-06-26T09:00:00',
      },
      {
        id: 'h4',
        action: 'Pendiente de validación',
        description: 'Trabajo finalizado, esperando confirmación del solicitante.',
        fromStatus: 'en_proceso',
        toStatus: 'pendiente_validacion',
        author: u('u4'),
        createdAt: '2026-06-28T16:00:00',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'almacen-antes.jpg', type: 'imagen', size: '3.1 MB', tipo: 'inicial' },
      { id: 'e2', name: 'camara-instalada.jpg', type: 'imagen', size: '2.8 MB', tipo: 'final' },
      { id: 'e3', name: 'reporte-instalacion.pdf', type: 'pdf', size: '560 KB', tipo: 'final' },
    ],
  },
  {
    id: 't5',
    code: 'PS-0005',
    title: 'Aire acondicionado no funciona — Sala de reuniones principal',
    description:
      'El equipo de aire acondicionado Daikin de la sala de reuniones principal dejó de funcionar. Hace calor intenso y hay reuniones importantes esta semana. El equipo enciende pero no enfría.',
    status: 'cerrado',
    priority: 'critica',
    type: 'Mantenimiento Correctivo',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a6',
    area: 'Mantenimiento',
    location: 'Sala de Reuniones Principal — Piso 1',
    createdBy: u('u5'),
    assignedTo: u('u2'),
    closedAt: '2026-06-26T17:00:00',
    createdAt: '2026-06-24T08:00:00',
    updatedAt: '2026-06-26T17:00:00',
    comments: [
      {
        id: 'c1',
        text: 'El gas refrigerante estaba agotado. Se recargó y el equipo funciona perfectamente ahora.',
        author: { ...u('u2'), rol: 'admin' },
        createdAt: '2026-06-26T16:45:00',
      },
      {
        id: 'c2',
        text: 'Confirmado, el equipo funciona. Gracias por la rápida respuesta.',
        author: { ...u('u5'), rol: 'user' },
        createdAt: '2026-06-26T17:10:00',
      },
    ],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado con prioridad crítica.',
        author: u('u5'),
        createdAt: '2026-06-24T08:00:00',
        toStatus: 'sin_asignar',
      },
      {
        id: 'h2',
        action: 'Ticket asignado',
        description: 'Asignado a Juan Pérez por urgencia.',
        fromStatus: 'sin_asignar',
        toStatus: 'asignado',
        author: u('u1'),
        createdAt: '2026-06-24T08:30:00',
      },
      {
        id: 'h3',
        action: 'Inicio de trabajo',
        description: 'Revisión del equipo iniciada.',
        fromStatus: 'asignado',
        toStatus: 'en_proceso',
        author: u('u2'),
        createdAt: '2026-06-24T09:00:00',
      },
      {
        id: 'h4',
        action: 'Pendiente de validación',
        description: 'Mantenimiento completado.',
        fromStatus: 'en_proceso',
        toStatus: 'pendiente_validacion',
        author: u('u2'),
        createdAt: '2026-06-26T16:45:00',
      },
      {
        id: 'h5',
        action: 'Ticket cerrado',
        description: 'Solicitante confirmó la solución.',
        fromStatus: 'pendiente_validacion',
        toStatus: 'cerrado',
        author: u('u5'),
        createdAt: '2026-06-26T17:00:00',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'ac-dañado.jpg', type: 'imagen', size: '2.2 MB', tipo: 'inicial' },
      { id: 'e2', name: 'ac-reparado.jpg', type: 'imagen', size: '1.9 MB', tipo: 'final' },
    ],
  },
  {
    id: 't6',
    code: 'PS-0006',
    title: 'Actualización del sistema de nómina a versión 2025',
    description:
      'El sistema de nómina requiere actualización a la versión 2025 antes del cierre del mes. La actualización incluye nuevos módulos de impuestos y cambios en el cálculo de gratificaciones según la nueva legislación.',
    status: 'en_proceso',
    priority: 'alta',
    type: 'Solicitud de Software',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a2',
    area: 'Recursos Humanos',
    createdBy: u('u5'),
    assignedTo: u('u3'),
    createdAt: '2026-06-26T10:00:00',
    updatedAt: '2026-06-29T09:00:00',
    comments: [
      {
        id: 'c1',
        text: 'La actualización requiere ventana de mantenimiento de 4 horas. Propongo el sábado 30/06 de 6am a 10am.',
        author: { ...u('u3'), rol: 'worker' },
        createdAt: '2026-06-27T14:00:00',
        isInternal: true,
      },
      {
        id: 'c2',
        text: 'Aprobado para el sábado 30/06. Coordinaré con RRHH para no ingresar al sistema en ese horario.',
        author: { ...u('u2'), rol: 'admin' },
        createdAt: '2026-06-27T15:30:00',
        isInternal: true,
      },
    ],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado.',
        author: u('u5'),
        createdAt: '2026-06-26T10:00:00',
        toStatus: 'sin_asignar',
      },
      {
        id: 'h2',
        action: 'Ticket asignado',
        description: 'Asignado a María López.',
        fromStatus: 'sin_asignar',
        toStatus: 'asignado',
        author: u('u2'),
        createdAt: '2026-06-26T11:00:00',
      },
      {
        id: 'h3',
        action: 'Inicio de trabajo',
        description: 'Preparación del entorno de pruebas.',
        fromStatus: 'asignado',
        toStatus: 'en_proceso',
        author: u('u3'),
        createdAt: '2026-06-27T08:00:00',
      },
    ],
    evidencias: [],
  },
  {
    id: 't7',
    code: 'PS-0007',
    title: 'Mouse dañado — Sala de reuniones secundaria',
    description:
      'El mouse de la sala de reuniones del piso 2 no funciona. El scroll rueda no responde y el click izquierdo falla intermitentemente.',
    status: 'sin_asignar',
    priority: 'baja',
    type: 'Incidencia de Hardware',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a3',
    area: 'Administración',
    location: 'Piso 2 — Sala de Reuniones',
    createdBy: u('u5'),
    createdAt: '2026-06-29T11:00:00',
    updatedAt: '2026-06-29T11:00:00',
    comments: [],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado.',
        author: u('u5'),
        createdAt: '2026-06-29T11:00:00',
        toStatus: 'sin_asignar',
      },
    ],
    evidencias: [],
  },
  {
    id: 't8',
    code: 'PS-0008',
    title: 'Caída de internet — Todas las áreas de Sucursal Norte',
    description:
      'Desde las 7:30 de hoy no hay conexión a internet en toda la sucursal. El modem reinicia pero no conecta. La línea telefónica funciona. Se han perdido 3 horas de trabajo. El proveedor fue notificado pero no da respuesta.',
    status: 'reabierto',
    priority: 'critica',
    type: 'Incidencia de Red',
    sucursalId: 's2',
    sucursal: 'Sucursal Norte',
    areaId: 'a7',
    area: 'Sistemas / TI',
    createdBy: u('u4'),
    assignedTo: u('u2'),
    createdAt: '2026-06-28T07:30:00',
    updatedAt: '2026-06-29T07:00:00',
    comments: [
      {
        id: 'c1',
        text: 'Se restableció la conexión a las 6pm. Problema era del ISP.',
        author: { ...u('u2'), rol: 'admin' },
        createdAt: '2026-06-28T18:00:00',
      },
      {
        id: 'c2',
        text: 'Volvió a caerse esta mañana a las 7am. Mismo problema.',
        author: { ...u('u4'), rol: 'worker' },
        createdAt: '2026-06-29T07:05:00',
      },
    ],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado.',
        author: u('u4'),
        createdAt: '2026-06-28T07:30:00',
        toStatus: 'sin_asignar',
      },
      {
        id: 'h2',
        action: 'Ticket asignado',
        description: 'Asignado a Juan Pérez.',
        fromStatus: 'sin_asignar',
        toStatus: 'asignado',
        author: u('u1'),
        createdAt: '2026-06-28T08:00:00',
      },
      {
        id: 'h3',
        action: 'Inicio de trabajo',
        description: 'Coordinando con ISP.',
        fromStatus: 'asignado',
        toStatus: 'en_proceso',
        author: u('u2'),
        createdAt: '2026-06-28T08:15:00',
      },
      {
        id: 'h4',
        action: 'Pendiente validación',
        description: 'Conexión restablecida.',
        fromStatus: 'en_proceso',
        toStatus: 'pendiente_validacion',
        author: u('u2'),
        createdAt: '2026-06-28T17:55:00',
      },
      {
        id: 'h5',
        action: 'Ticket cerrado',
        description: 'Problema resuelto.',
        fromStatus: 'pendiente_validacion',
        toStatus: 'cerrado',
        author: u('u4'),
        createdAt: '2026-06-28T18:10:00',
      },
      {
        id: 'h6',
        action: 'Ticket reabierto',
        description: 'Problema recurrente.',
        fromStatus: 'cerrado',
        toStatus: 'reabierto',
        author: u('u4'),
        createdAt: '2026-06-29T07:00:00',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'error-modem.jpg', type: 'imagen', size: '1.1 MB', tipo: 'inicial' },
    ],
  },
  {
    id: 't9',
    code: 'PS-0009',
    title: 'Instalación de herramienta de videoconferencia corporativa',
    description:
      'Se requiere instalar y configurar Microsoft Teams en los 8 equipos del área de ventas de Sucursal Sur. La licencia corporativa ya está disponible.',
    status: 'asignado',
    priority: 'media',
    type: 'Solicitud de Software',
    sucursalId: 's3',
    sucursal: 'Sucursal Sur',
    areaId: 'a9',
    area: 'Sistemas / TI',
    createdBy: u('u6'),
    assignedTo: u('u3'),
    createdAt: '2026-06-27T15:00:00',
    updatedAt: '2026-06-28T09:00:00',
    comments: [],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado.',
        author: u('u6'),
        createdAt: '2026-06-27T15:00:00',
        toStatus: 'sin_asignar',
      },
      {
        id: 'h2',
        action: 'Ticket asignado',
        description: 'Asignado a María López.',
        fromStatus: 'sin_asignar',
        toStatus: 'asignado',
        author: u('u2'),
        createdAt: '2026-06-28T09:00:00',
      },
    ],
    evidencias: [],
  },
  {
    id: 't10',
    code: 'PS-0010',
    title: 'Mantenimiento preventivo de equipos — Sede Central',
    description:
      'Mantenimiento preventivo trimestral de todos los equipos de cómputo de la Sede Central. Incluye limpieza física, actualización de drivers y verificación de discos duros. 24 equipos en total.',
    status: 'cerrado',
    priority: 'media',
    type: 'Mantenimiento Preventivo',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a1',
    area: 'Sistemas / TI',
    createdBy: u('u2'),
    assignedTo: u('u3'),
    closedAt: '2026-06-20T17:00:00',
    createdAt: '2026-06-15T09:00:00',
    updatedAt: '2026-06-20T17:00:00',
    comments: [
      {
        id: 'c1',
        text: 'Mantenimiento completado en todos los equipos. Reporte adjunto.',
        author: { ...u('u3'), rol: 'worker' },
        createdAt: '2026-06-20T16:30:00',
      },
    ],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Programado por el administrador.',
        author: u('u2'),
        createdAt: '2026-06-15T09:00:00',
        toStatus: 'asignado',
      },
      {
        id: 'h2',
        action: 'Inicio de trabajo',
        description: 'Mantenimiento iniciado.',
        fromStatus: 'asignado',
        toStatus: 'en_proceso',
        author: u('u3'),
        createdAt: '2026-06-17T08:00:00',
      },
      {
        id: 'h3',
        action: 'Pendiente validación',
        description: 'Mantenimiento completado.',
        fromStatus: 'en_proceso',
        toStatus: 'pendiente_validacion',
        author: u('u3'),
        createdAt: '2026-06-20T16:30:00',
      },
      {
        id: 'h4',
        action: 'Ticket cerrado',
        description: 'Validado por administrador.',
        fromStatus: 'pendiente_validacion',
        toStatus: 'cerrado',
        author: u('u2'),
        createdAt: '2026-06-20T17:00:00',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'reporte-mantenimiento.pdf', type: 'pdf', size: '1.2 MB', tipo: 'final' },
    ],
  },
  {
    id: 't11',
    code: 'PS-0011',
    title: 'Error crítico en sistema de liquidación de nómina',
    description:
      'El módulo de cálculo de liquidaciones del sistema de nómina genera montos incorrectos para empleados con más de 10 años de antigüedad. El error afecta a 12 colaboradores del cierre de este mes. El sistema calcula mal la CTS.',
    status: 'en_proceso',
    priority: 'critica',
    type: 'Incidencia de Software',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a2',
    area: 'Recursos Humanos',
    createdBy: u('u5'),
    assignedTo: u('u2'),
    createdAt: '2026-06-29T06:00:00',
    updatedAt: '2026-06-29T11:00:00',
    comments: [
      {
        id: 'c1',
        text: 'Detectado el bug. Está en el módulo de cálculo de CTS. Contactando al proveedor del sistema ahora.',
        author: { ...u('u2'), rol: 'admin' },
        createdAt: '2026-06-29T09:30:00',
        isInternal: true,
      },
      {
        id: 'c2',
        text: '¿Cuándo estará resuelto? El cierre es el 30/06 y necesitamos los cálculos correctos.',
        author: { ...u('u5'), rol: 'user' },
        createdAt: '2026-06-29T10:00:00',
      },
      {
        id: 'c3',
        text: 'El proveedor confirmó un hotfix disponible para hoy. Se aplicará a las 2pm.',
        author: { ...u('u2'), rol: 'admin' },
        createdAt: '2026-06-29T11:00:00',
      },
    ],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado con prioridad crítica.',
        author: u('u5'),
        createdAt: '2026-06-29T06:00:00',
        toStatus: 'sin_asignar',
      },
      {
        id: 'h2',
        action: 'Ticket asignado',
        description: 'Asignado a Juan Pérez por urgencia.',
        fromStatus: 'sin_asignar',
        toStatus: 'asignado',
        author: u('u1'),
        createdAt: '2026-06-29T07:00:00',
      },
      {
        id: 'h3',
        action: 'Inicio de trabajo',
        description: 'Investigación del error iniciada.',
        fromStatus: 'asignado',
        toStatus: 'en_proceso',
        author: u('u2'),
        createdAt: '2026-06-29T09:00:00',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'reporte-error.pdf', type: 'pdf', size: '890 KB', tipo: 'inicial' },
    ],
  },
  {
    id: 't12',
    code: 'PS-0012',
    title: 'Solicitud de sillas ergonómicas para área de ventas',
    description:
      'El área de ventas de Sucursal Norte necesita 6 sillas ergonómicas. Los trabajadores presentan quejas de molestias en la espalda. El médico ocupacional ya emitió recomendación.',
    status: 'pendiente_validacion',
    priority: 'baja',
    type: 'Solicitud de Servicio',
    sucursalId: 's2',
    sucursal: 'Sucursal Norte',
    areaId: 'a8',
    area: 'Administración',
    createdBy: u('u4'),
    assignedTo: u('u5'),
    createdAt: '2026-06-22T10:00:00',
    updatedAt: '2026-06-27T16:00:00',
    comments: [
      {
        id: 'c1',
        text: 'Las 6 sillas fueron entregadas e instaladas. Favor confirmar recepción.',
        author: { ...u('u5'), rol: 'user' },
        createdAt: '2026-06-27T15:50:00',
      },
    ],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado.',
        author: u('u4'),
        createdAt: '2026-06-22T10:00:00',
        toStatus: 'sin_asignar',
      },
      {
        id: 'h2',
        action: 'Ticket asignado',
        description: 'Asignado a Ana Ramírez (adquisiciones).',
        fromStatus: 'sin_asignar',
        toStatus: 'asignado',
        author: u('u1'),
        createdAt: '2026-06-23T09:00:00',
      },
      {
        id: 'h3',
        action: 'Inicio de trabajo',
        description: 'Proceso de compra iniciado.',
        fromStatus: 'asignado',
        toStatus: 'en_proceso',
        author: u('u5'),
        createdAt: '2026-06-23T10:00:00',
      },
      {
        id: 'h4',
        action: 'Pendiente validación',
        description: 'Sillas entregadas.',
        fromStatus: 'en_proceso',
        toStatus: 'pendiente_validacion',
        author: u('u5'),
        createdAt: '2026-06-27T16:00:00',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'guia-remision.pdf', type: 'pdf', size: '320 KB', tipo: 'final' },
    ],
  },
  {
    id: 't13',
    code: 'PS-0013',
    title: 'Escáner de documentos no detectado por Windows',
    description:
      'El escáner Canon imageFormula DR-S130 dejó de ser reconocido por Windows 11 luego de una actualización automática. El equipo enciende pero no aparece en Dispositivos.',
    status: 'sin_asignar',
    priority: 'media',
    type: 'Incidencia de Hardware',
    sucursalId: 's3',
    sucursal: 'Sucursal Sur',
    areaId: 'a9',
    area: 'Sistemas / TI',
    createdBy: u('u6'),
    createdAt: '2026-06-29T10:00:00',
    updatedAt: '2026-06-29T10:00:00',
    comments: [],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado.',
        author: u('u6'),
        createdAt: '2026-06-29T10:00:00',
        toStatus: 'sin_asignar',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'dispositivos-error.png', type: 'imagen', size: '780 KB', tipo: 'inicial' },
    ],
  },
  {
    id: 't14',
    code: 'PS-0014',
    title: 'Actualización masiva de antivirus corporativo',
    description:
      'Se requiere actualizar el antivirus Kaspersky Endpoint Security en los 45 equipos de Sede Central. La versión actual tiene una vulnerabilidad crítica reportada por el fabricante.',
    status: 'asignado',
    priority: 'alta',
    type: 'Incidencia de Software',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a1',
    area: 'Sistemas / TI',
    createdBy: u('u2'),
    assignedTo: u('u6'),
    createdAt: '2026-06-28T14:00:00',
    updatedAt: '2026-06-29T08:00:00',
    comments: [],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket de seguridad registrado.',
        author: u('u2'),
        createdAt: '2026-06-28T14:00:00',
        toStatus: 'asignado',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'boletin-kaspersky.pdf', type: 'pdf', size: '420 KB', tipo: 'inicial' },
    ],
  },
  {
    id: 't15',
    code: 'PS-0015',
    title: 'Cable de red caído — Piso 2 completo',
    description:
      'Todos los equipos del piso 2 perdieron conexión a la red local. Los cables salen del mismo switch. Posible falla en el switch de distribución del piso.',
    status: 'cerrado',
    priority: 'alta',
    type: 'Incidencia de Red',
    sucursalId: 's1',
    sucursal: 'Sede Central',
    areaId: 'a1',
    area: 'Sistemas / TI',
    location: 'Piso 2 — Rack de Comunicaciones',
    createdBy: u('u5'),
    assignedTo: u('u3'),
    closedAt: '2026-06-27T12:00:00',
    createdAt: '2026-06-27T07:30:00',
    updatedAt: '2026-06-27T12:00:00',
    comments: [
      {
        id: 'c1',
        text: 'Switch reemplazado. Toda la red del piso 2 restaurada.',
        author: { ...u('u3'), rol: 'worker' },
        createdAt: '2026-06-27T11:45:00',
      },
    ],
    history: [
      {
        id: 'h1',
        action: 'Ticket creado',
        description: 'Ticket registrado.',
        author: u('u5'),
        createdAt: '2026-06-27T07:30:00',
        toStatus: 'sin_asignar',
      },
      {
        id: 'h2',
        action: 'Ticket asignado',
        description: 'Asignado a María López.',
        fromStatus: 'sin_asignar',
        toStatus: 'asignado',
        author: u('u2'),
        createdAt: '2026-06-27T07:45:00',
      },
      {
        id: 'h3',
        action: 'Inicio de trabajo',
        description: 'Diagnóstico iniciado.',
        fromStatus: 'asignado',
        toStatus: 'en_proceso',
        author: u('u3'),
        createdAt: '2026-06-27T08:00:00',
      },
      {
        id: 'h4',
        action: 'Pendiente validación',
        description: 'Red restablecida.',
        fromStatus: 'en_proceso',
        toStatus: 'pendiente_validacion',
        author: u('u3'),
        createdAt: '2026-06-27T11:45:00',
      },
      {
        id: 'h5',
        action: 'Ticket cerrado',
        description: 'Validado por Ana Ramírez.',
        fromStatus: 'pendiente_validacion',
        toStatus: 'cerrado',
        author: u('u5'),
        createdAt: '2026-06-27T12:00:00',
      },
    ],
    evidencias: [
      { id: 'e1', name: 'switch-dañado.jpg', type: 'imagen', size: '1.4 MB', tipo: 'inicial' },
      { id: 'e2', name: 'switch-reemplazado.jpg', type: 'imagen', size: '1.6 MB', tipo: 'final' },
    ],
  },
]

// ── Notificaciones ────────────────────────────────────────────────────────────

export const MOCK_NOTIFICATIONS: MockNotification[] = [
  {
    id: 'n1',
    title: 'Nuevo ticket asignado',
    body: 'Se te asignó el ticket PS-0009: "Instalación de herramienta de videoconferencia corporativa".',
    type: 'ticket_assigned',
    read: false,
    ticketId: 't9',
    ticketCode: 'PS-0009',
    createdAt: '2026-06-28T09:00:00',
  },
  {
    id: 'n2',
    title: 'Ticket reabierto',
    body: 'El ticket PS-0008 "Caída de internet — Sucursal Norte" fue reabierto.',
    type: 'ticket_status',
    read: false,
    ticketId: 't8',
    ticketCode: 'PS-0008',
    createdAt: '2026-06-29T07:00:00',
  },
  {
    id: 'n3',
    title: 'Nuevo comentario',
    body: 'Ana Ramírez comentó en PS-0001: "Por favor apresúrense, necesitamos imprimir contratos urgentes."',
    type: 'comment',
    read: false,
    ticketId: 't1',
    ticketCode: 'PS-0001',
    createdAt: '2026-06-28T15:00:00',
  },
  {
    id: 'n4',
    title: 'Ticket PS-0011 en proceso crítico',
    body: 'El ticket PS-0011 de prioridad crítica fue asignado a Juan Pérez.',
    type: 'ticket_status',
    read: false,
    ticketId: 't11',
    ticketCode: 'PS-0011',
    createdAt: '2026-06-29T07:00:00',
  },
  {
    id: 'n5',
    title: 'Ticket cerrado',
    body: 'El ticket PS-0010 "Mantenimiento preventivo de equipos" fue cerrado exitosamente.',
    type: 'ticket_status',
    read: true,
    ticketId: 't10',
    ticketCode: 'PS-0010',
    createdAt: '2026-06-20T17:00:00',
  },
  {
    id: 'n6',
    title: 'Nuevo ticket creado',
    body: 'Se creó el ticket PS-0013: "Escáner de documentos no detectado por Windows" en Sucursal Sur.',
    type: 'ticket_new',
    read: true,
    ticketId: 't13',
    ticketCode: 'PS-0013',
    createdAt: '2026-06-29T10:00:00',
  },
  {
    id: 'n7',
    title: 'Recordatorio: ticket sin atender',
    body: 'El ticket PS-0003 lleva más de 24 horas sin ser asignado.',
    type: 'system',
    read: true,
    ticketId: 't3',
    ticketCode: 'PS-0003',
    createdAt: '2026-06-29T08:00:00',
  },
  {
    id: 'n8',
    title: 'Ticket PS-0004 pendiente de validación',
    body: 'El ticket PS-0004 está esperando tu confirmación de cierre.',
    type: 'ticket_status',
    read: true,
    ticketId: 't4',
    ticketCode: 'PS-0004',
    createdAt: '2026-06-28T16:00:00',
  },
]

// ── Datos para gráficos del dashboard admin ───────────────────────────────────

export const MOCK_TREND_DATA = [
  { fecha: '14/06', creados: 2, resueltos: 1 },
  { fecha: '15/06', creados: 4, resueltos: 2 },
  { fecha: '16/06', creados: 1, resueltos: 3 },
  { fecha: '17/06', creados: 3, resueltos: 2 },
  { fecha: '18/06', creados: 0, resueltos: 1 },
  { fecha: '19/06', creados: 0, resueltos: 0 },
  { fecha: '20/06', creados: 5, resueltos: 4 },
  { fecha: '21/06', creados: 2, resueltos: 3 },
  { fecha: '22/06', creados: 3, resueltos: 2 },
  { fecha: '23/06', creados: 1, resueltos: 1 },
  { fecha: '24/06', creados: 4, resueltos: 1 },
  { fecha: '25/06', creados: 2, resueltos: 3 },
  { fecha: '26/06', creados: 3, resueltos: 2 },
  { fecha: '27/06', creados: 4, resueltos: 3 },
  { fecha: '28/06', creados: 5, resueltos: 2 },
  { fecha: '29/06', creados: 4, resueltos: 1 },
]

// ── Helpers ───────────────────────────────────────────────────────────────────

export function getTicketsByUser(userId: string): MockTicket[] {
  return MOCK_TICKETS.filter((t) => t.createdBy.id === userId || t.assignedTo?.id === userId)
}

export function getTicketById(id: string): MockTicket | undefined {
  return MOCK_TICKETS.find((t) => t.id === id)
}

export function getTicketsByStatus(status: TicketStatus): MockTicket[] {
  return MOCK_TICKETS.filter((t) => t.status === status)
}

export function getUserById(id: string): MockUser | undefined {
  return MOCK_USERS.find((u) => u.id === id)
}

export function getUnreadNotifications(): MockNotification[] {
  return MOCK_NOTIFICATIONS.filter((n) => !n.read)
}

export const TICKET_STATUS_COUNTS = {
  sin_asignar: MOCK_TICKETS.filter((t) => t.status === 'sin_asignar').length,
  asignado: MOCK_TICKETS.filter((t) => t.status === 'asignado').length,
  en_proceso: MOCK_TICKETS.filter((t) => t.status === 'en_proceso').length,
  pendiente_validacion: MOCK_TICKETS.filter((t) => t.status === 'pendiente_validacion').length,
  cerrado: MOCK_TICKETS.filter((t) => t.status === 'cerrado').length,
  reabierto: MOCK_TICKETS.filter((t) => t.status === 'reabierto').length,
}
