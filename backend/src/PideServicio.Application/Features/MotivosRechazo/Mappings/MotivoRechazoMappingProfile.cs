namespace PideServicio.Application.Features.MotivosRechazo.Mappings;

using Mapster;
using PideServicio.Application.Features.MotivosRechazo.DTOs;
using PideServicio.Domain.Entities;

public sealed class MotivoRechazoMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MotivoRechazo, MotivoRechazoResumenDto>()
            .Map(dest => dest.EsGlobal, src => src.EmpresaId == null);

        config.NewConfig<MotivoRechazo, MotivoRechazoDto>()
            .Map(dest => dest.EsGlobal, src => src.EmpresaId == null);
    }
}
