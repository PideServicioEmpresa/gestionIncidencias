namespace PideServicio.Application.Features.Auditoria.Mappings;

using Mapster;
using PideServicio.Application.Features.Auditoria.DTOs;
using PideServicio.Domain.Entities;

public sealed class AuditLogMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AuditLog, AuditLogDto>();
    }
}
