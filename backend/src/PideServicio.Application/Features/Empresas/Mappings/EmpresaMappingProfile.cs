namespace PideServicio.Application.Features.Empresas.Mappings;

using Mapster;
using PideServicio.Application.Features.Empresas.DTOs;
using PideServicio.Domain.Entities;

public sealed class EmpresaMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Empresa, EmpresaDto>();
        config.NewConfig<Empresa, EmpresaResumenDto>();
    }
}
