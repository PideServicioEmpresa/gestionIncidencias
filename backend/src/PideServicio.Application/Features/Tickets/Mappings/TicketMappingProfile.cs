namespace PideServicio.Application.Features.Tickets.Mappings;

using Mapster;
using PideServicio.Application.Features.Tickets.DTOs;
using PideServicio.Domain.Entities;

public sealed class TicketMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Ticket, TicketResumenDto>()
            .Map(dest => dest.Estado, src => src.Estado.ToString())
            .Map(dest => dest.PrioridadEfectiva, src => src.PrioridadEfectiva.ToString())
            .Map(dest => dest.Codigo, src => src.Codigo.Valor);

        config.NewConfig<Ticket, TicketDetalleDto>()
            .Map(dest => dest.Estado, src => src.Estado.ToString())
            .Map(dest => dest.PrioridadSolicitante, src => src.PrioridadSolicitante.ToString())
            .Map(dest => dest.PrioridadAdmin, src => src.PrioridadAdmin.HasValue ? src.PrioridadAdmin.Value.ToString() : null)
            .Map(dest => dest.PrioridadEfectiva, src => src.PrioridadEfectiva.ToString())
            .Map(dest => dest.Codigo, src => src.Codigo.Valor);

        config.NewConfig<TicketHistorialEntrada, TicketHistorialDto>()
            .Map(dest => dest.TipoEvento, src => src.TipoEvento.ToString())
            .Map(dest => dest.EstadoAnterior, src => src.EstadoAnterior.HasValue ? src.EstadoAnterior.Value.ToString() : null)
            .Map(dest => dest.EstadoNuevo, src => src.EstadoNuevo.HasValue ? src.EstadoNuevo.Value.ToString() : null);
    }
}
