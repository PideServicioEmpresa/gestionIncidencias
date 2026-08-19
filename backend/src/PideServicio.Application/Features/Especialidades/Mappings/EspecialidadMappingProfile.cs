namespace PideServicio.Application.Features.Especialidades.Mappings;

using Mapster;
using PideServicio.Application.Features.Especialidades.DTOs;
using PideServicio.Domain.Entities;

public sealed class EspecialidadMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Especialidad, EspecialidadResumenDto>()
            .Map(dest => dest.EsGlobal, src => src.EmpresaId == null);

        config.NewConfig<Especialidad, EspecialidadDto>()
            .Map(dest => dest.EsGlobal, src => src.EmpresaId == null);
    }
}
