namespace PideServicio.Application.Features.Sucursales.Mappings;

using Mapster;
using PideServicio.Application.Features.Sucursales.DTOs;
using PideServicio.Domain.Entities;

public sealed class SucursalMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Sucursal, SucursalDto>();
        config.NewConfig<Sucursal, SucursalResumenDto>();
    }
}
