namespace PideServicio.Architecture.Tests;

using System.IO;
using System.Text.RegularExpressions;
using Xunit;

/// <summary>
/// Valida el SQL del repositorio de empresa_correos_copia contra el esquema declarado
/// en schema.sql. Sin base de datos no hay test de integración posible, pero este
/// comprueba lo que de verdad falló en producción: que el código no escriba columnas
/// inexistentes (error 42703).
/// </summary>
public class EmpresaCorreosCopiaEsquemaTests
{
    /// <summary>Columnas reales de la tabla. Auditoría reducida a propósito.</summary>
    private static readonly string[] ColumnasEsperadas =
        ["id", "empresa_id", "correo", "activo", "created_at"];

    /// <summary>Columnas de auditoría que el resto del modelo sí tiene y esta tabla NO.</summary>
    private static readonly string[] ColumnasAusentes =
        ["created_by", "updated_at", "updated_by", "deleted_at", "deleted_by"];

    private static string RaizRepo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "schema.sql")))
            dir = dir.Parent;

        Assert.NotNull(dir);
        return dir!.FullName;
    }

    private static string LeerDefinicionDeTabla()
    {
        var schema = File.ReadAllText(Path.Combine(RaizRepo(), "schema.sql"));
        var match = Regex.Match(
            schema,
            @"CREATE TABLE public\.empresa_correos_copia \((?<cuerpo>.*?)\);",
            RegexOptions.Singleline);

        Assert.True(match.Success, "No se encontró la definición de empresa_correos_copia en schema.sql");
        return match.Groups["cuerpo"].Value;
    }

    [Fact]
    public void La_tabla_tiene_auditoria_reducida_a_proposito()
    {
        var cuerpo = LeerDefinicionDeTabla();

        foreach (var columna in ColumnasEsperadas)
            Assert.Contains(columna, cuerpo);

        // Si alguna de estas apareciera, la nota del repositorio dejaría de ser cierta
        // y habría que revisar las consultas.
        foreach (var columna in ColumnasAusentes)
            Assert.DoesNotContain(columna, cuerpo);
    }

    [Fact]
    public void El_repositorio_no_referencia_columnas_de_auditoria_inexistentes()
    {
        var ruta = Path.Combine(
            RaizRepo(),
            "backend", "src", "PideServicio.Persistence", "Repositories",
            "EmpresaCorreoCopiaRepository.cs");

        Assert.True(File.Exists(ruta), $"No se encontró el repositorio en {ruta}");

        // Solo el SQL: los comentarios explican precisamente que estas columnas no existen.
        var codigo = File.ReadAllText(ruta);
        var sentencias = Regex.Matches(codigo, @"(INSERT|UPDATE|SELECT)\s.*?""""""", RegexOptions.Singleline);

        foreach (Match sentencia in sentencias)
        {
            foreach (var columna in ColumnasAusentes)
            {
                Assert.False(
                    Regex.IsMatch(sentencia.Value, $@"\b{columna}\b"),
                    $"El SQL del repositorio referencia '{columna}', que no existe en empresa_correos_copia.");
            }
        }
    }
}
