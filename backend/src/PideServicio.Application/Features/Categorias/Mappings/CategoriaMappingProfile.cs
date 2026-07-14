namespace PideServicio.Application.Features.Categorias.Mappings;

using Mapster;
using PideServicio.Application.Features.Categorias.DTOs;
using PideServicio.Domain.Entities;

public sealed class CategoriaMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Categoria, CategoriaResumenDto>()
            .Map(dest => dest.EsGlobal, src => src.EmpresaId == null);

        config.NewConfig<Categoria, CategoriaDto>()
            .Map(dest => dest.EsGlobal, src => src.EmpresaId == null);
    }
}
