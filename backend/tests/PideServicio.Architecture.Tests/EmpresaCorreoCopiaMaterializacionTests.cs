namespace PideServicio.Architecture.Tests;

using System.Data;
using Dapper;
using PideServicio.Application.Features.Empresas.DTOs;
using Xunit;

/// <summary>
/// Dapper materializa en runtime: un desajuste de tipos entre las columnas y el
/// constructor pasa el build y revienta en producción. Estos tests simulan el lector
/// que devuelve Npgsql para 'empresa_correos_copia' —donde created_at (timestamptz)
/// llega como DateTime— sin necesitar base de datos.
/// </summary>
public class EmpresaCorreoCopiaMaterializacionTests
{
    /// <summary>Equivalente a la fila interna del repositorio: propiedades asignables.</summary>
    private sealed class CorreoCopiaRow
    {
        public Guid Id { get; init; }
        public string Correo { get; init; } = string.Empty;
        public bool Activo { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }

    private static IDataReader LectorSimulado(Guid id, string correo, bool activo, DateTime creadoUtc)
    {
        var tabla = new DataTable();
        tabla.Columns.Add("Id", typeof(Guid));
        tabla.Columns.Add("Correo", typeof(string));
        tabla.Columns.Add("Activo", typeof(bool));
        // Npgsql entrega timestamptz como DateTime, no como DateTimeOffset:
        // ese es exactamente el desajuste que rompía la materialización.
        tabla.Columns.Add("CreatedAt", typeof(DateTime));
        tabla.Rows.Add(id, correo, activo, creadoUtc);
        return tabla.CreateDataReader();
    }

    [Fact]
    public void La_fila_con_propiedades_se_materializa_y_construye_el_DTO()
    {
        var id = Guid.NewGuid();
        var creadoUtc = new DateTime(2026, 8, 19, 12, 30, 0, DateTimeKind.Utc);

        using var lector = LectorSimulado(id, "serviciosgenerales@corporacionvega.pe", true, creadoUtc);
        lector.Read();

        var fila = lector.GetRowParser<CorreoCopiaRow>()(lector);
        var dto = new EmpresaCorreoCopiaDto(fila.Id, fila.Correo, fila.Activo, fila.CreatedAt);

        Assert.Equal(id, dto.Id);
        Assert.Equal("serviciosgenerales@corporacionvega.pe", dto.Correo);
        Assert.True(dto.Activo);
        // Lo que se comprueba es que la conversión DateTime → DateTimeOffset ocurre sin
        // lanzar y conserva la marca temporal. No se compara el instante UTC absoluto
        // porque DataTable descarta el DateTimeKind y el offset acaba siendo el local;
        // con Npgsql la columna timestamptz sí llega como Utc.
        Assert.Equal(creadoUtc.TimeOfDay, dto.CreatedAt.DateTime.TimeOfDay);
        Assert.Equal(creadoUtc.Date, dto.CreatedAt.Date);
    }

    [Fact]
    public void Materializar_el_record_posicional_directamente_falla_y_por_eso_existe_la_fila()
    {
        var creadoUtc = new DateTime(2026, 8, 19, 12, 30, 0, DateTimeKind.Utc);
        using var lector = LectorSimulado(Guid.NewGuid(), "correo@empresa.pe", true, creadoUtc);
        lector.Read();

        // Documenta el bug original: sin la fila intermedia, Dapper no encuentra un
        // constructor que acepte DateTime para un parámetro DateTimeOffset.
        Assert.ThrowsAny<Exception>(() => lector.GetRowParser<EmpresaCorreoCopiaDto>()(lector));
    }
}
