namespace PideServicio.Application.Features.Notificaciones.Mappings;

using Mapster;
using PideServicio.Application.Features.Notificaciones.DTOs;
using PideServicio.Domain.Entities;

public sealed class NotificacionMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Notificacion, NotificacionDto>()
            .Map(dest => dest.Canal, src => src.Canal.ToString())
            .Map(dest => dest.EstadoEntrega, src => src.EstadoEntrega.ToString())
            .Map(dest => dest.EsLeida, src => src.LeidoEn.HasValue);
    }
}
