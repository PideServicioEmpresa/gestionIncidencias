namespace PideServicio.Architecture.Tests;

using Mapster;
using PideServicio.Application.Features.Usuarios.DTOs;
using PideServicio.Domain.Entities;
using PideServicio.Domain.Enums;
using Xunit;

/// <summary>
/// Los mapeos de Mapster se compilan en runtime: un DTO mal configurado pasa el build
/// y revienta en producción. Este test los ejercita de verdad.
/// </summary>
public class UsuarioMappingTests
{
    [Fact]
    public void Usuario_se_mapea_a_UsuarioResumenDto_sin_error_de_compilacion()
    {
        var config = new TypeAdapterConfig();
        config.Scan(typeof(UsuarioResumenDto).Assembly);

        var usuario = Usuario.Crear(
            authId: Guid.NewGuid(),
            empresaId: Guid.NewGuid(),
            sucursalId: Guid.NewGuid(),
            nombre: "Milagros",
            apellido: "Maco",
            correo: "milagros@inmoveg.pe",
            nombreUsuario: "milagros",
            rol: RolTipo.SUPERADMIN);

        // Esto es lo que revienta en producción si el mapeo está mal configurado
        var dto = usuario.Adapt<UsuarioResumenDto>(config);

        Assert.Equal(usuario.Id, dto.Id);
        Assert.Equal("Milagros Maco", dto.NombreCompleto);
        Assert.Equal("milagros@inmoveg.pe", dto.Correo);
        Assert.Equal("SUPERADMIN", dto.Rol);
        Assert.True(dto.Activo);
        Assert.Empty(dto.Especialidades);

        // Y el `with` que usa el handler para inyectar las especialidades
        var conEspecialidades = dto with { Especialidades = new[] { "Grifería", "Electricidad" } };
        Assert.Equal(2, conEspecialidades.Especialidades.Count);
    }
}
