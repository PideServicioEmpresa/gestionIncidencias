namespace PideServicio.Application.Features.Usuarios.Mappings;

using Mapster;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Entities;

public sealed class UsuarioMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // ConstructUsing es obligatorio aquí: UsuarioResumenDto es un record con
        // constructor posicional y, además, una propiedad init fuera de ese constructor
        // (Especialidades). Ante esa combinación Mapster deja de usar el constructor
        // posicional y busca uno sin parámetros, que no existe → CompileException.
        // Indicándole cómo construirlo, el mapeo vuelve a resolverse sin ambigüedad.
        config.NewConfig<Usuario, UsuarioResumenDto>()
            .ConstructUsing(src => new UsuarioResumenDto(
                src.Id,
                src.NombreCompleto,
                src.Correo.Valor,
                src.Rol.ToString(),
                src.EstadoLaboral.ToString(),
                src.Activo,
                src.CreatedAt))
            // Las especialidades no vienen de la entidad: las resuelve el handler del
            // listado con una consulta agregada aparte.
            .Ignore(dest => dest.Especialidades);
    }
}
