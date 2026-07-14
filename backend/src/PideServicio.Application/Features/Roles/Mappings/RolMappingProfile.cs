namespace PideServicio.Application.Features.Roles.Mappings;

using Mapster;
using PideServicio.Application.Features.Roles.DTOs;
using PideServicio.Domain.Entities;

public sealed class RolMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Rol, RolDto>()
            .Map(dest => dest.Codigo, src => src.Codigo.ToString());

        config.NewConfig<Permiso, PermisoDto>();
    }
}
