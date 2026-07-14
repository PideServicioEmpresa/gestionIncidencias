namespace PideServicio.Application.Features.MotivosCancelacion.Mappings;

using Mapster;
using PideServicio.Application.Features.MotivosCancelacion.DTOs;
using PideServicio.Domain.Entities;

public sealed class MotivoCancelacionMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MotivoCancelacion, MotivoCancelacionResumenDto>()
            .Map(dest => dest.EsGlobal, src => src.EmpresaId == null);

        config.NewConfig<MotivoCancelacion, MotivoCancelacionDto>()
            .Map(dest => dest.EsGlobal, src => src.EmpresaId == null);
    }
}
