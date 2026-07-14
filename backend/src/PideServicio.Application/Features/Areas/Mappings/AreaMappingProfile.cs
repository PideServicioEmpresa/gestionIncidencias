namespace PideServicio.Application.Features.Areas.Mappings;

using Mapster;
using PideServicio.Application.Features.Areas.DTOs;
using PideServicio.Domain.Entities;

public sealed class AreaMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Area, AreaDto>();
        config.NewConfig<Area, AreaResumenDto>();
    }
}
