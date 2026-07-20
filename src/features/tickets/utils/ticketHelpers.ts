export function getTituloTicket(ticket: { titulo?: string | null; codigo: string }): string {
  return ticket.titulo?.trim() || ticket.codigo
}
