namespace PideServicio.Application.Features.TiposServicio.Mappings;

using Mapster;
using PideServicio.Application.Features.TiposServicio.DTOs;
using PideServicio.Domain.Entities;

public sealed class TipoServicioMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TipoServicio, TipoServicioResumenDto>();
    }
}
