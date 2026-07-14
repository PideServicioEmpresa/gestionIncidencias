namespace PideServicio.Application.Features.Comentarios.Mappings;

using Mapster;
using PideServicio.Application.Features.Comentarios.DTOs;
using PideServicio.Domain.Entities;

public sealed class ComentarioMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TicketComentario, ComentarioDto>();
    }
}
