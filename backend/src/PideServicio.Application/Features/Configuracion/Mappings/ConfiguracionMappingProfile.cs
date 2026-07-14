namespace PideServicio.Application.Features.Configuracion.Mappings;

using Mapster;
using PideServicio.Application.Features.Configuracion.DTOs;
using PideServicio.Domain.Entities;

public sealed class ConfiguracionMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Parametro, ParametroDto>();
    }
}
