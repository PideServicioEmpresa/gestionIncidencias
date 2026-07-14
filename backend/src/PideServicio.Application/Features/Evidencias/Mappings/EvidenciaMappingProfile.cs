namespace PideServicio.Application.Features.Evidencias.Mappings;

using Mapster;
using PideServicio.Application.Features.Evidencias.DTOs;
using PideServicio.Domain.Entities;

public sealed class EvidenciaMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TicketEvidencia, EvidenciaDto>()
            .Map(dest => dest.Tipo, src => src.Tipo.ToString());
    }
}
