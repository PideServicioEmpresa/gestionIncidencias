namespace PideServicio.Application.Features.Usuarios.Mappings;

using Mapster;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Entities;

public sealed class UsuarioMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Usuario, UsuarioDto>()
            .Map(dest => dest.Correo, src => src.Correo.Valor)
            .Map(dest => dest.Rol, src => src.Rol.ToString())
            .Map(dest => dest.EstadoLaboral, src => src.EstadoLaboral.ToString());

        config.NewConfig<Usuario, UsuarioResumenDto>()
            .Map(dest => dest.Correo, src => src.Correo.Valor)
            .Map(dest => dest.Rol, src => src.Rol.ToString())
            .Map(dest => dest.EstadoLaboral, src => src.EstadoLaboral.ToString());
    }
}
