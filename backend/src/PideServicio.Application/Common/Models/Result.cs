namespace PideServicio.Application.Common.Models;

public enum ResultTipo
{
    Exito,
    ErrorNegocio,
    ErrorValidacion,
    NoEncontrado,
    NoAutorizado,
    NoPermitido
}

public class Result
{
    public bool EsExitoso { get; }
    public bool EsFallido => !EsExitoso;
    public string Error { get; }
    public ResultTipo Tipo { get; }
    public IReadOnlyDictionary<string, string[]>? Errores { get; }

    protected Result(bool esExitoso, string error, ResultTipo tipo, IReadOnlyDictionary<string, string[]>? errores = null)
    {
        EsExitoso = esExitoso;
        Error = error;
        Tipo = tipo;
        Errores = errores;
    }

    public static Result Exito() => new(true, string.Empty, ResultTipo.Exito);
    public static Result Fallo(string error) => new(false, error, ResultTipo.ErrorNegocio);
    public static Result NoEncontrado(string mensaje) => new(false, mensaje, ResultTipo.NoEncontrado);
    public static Result NoEncontrado(string entidad, Guid id) => new(false, $"{entidad} con ID '{id}' no fue encontrado.", ResultTipo.NoEncontrado);
    public static Result ErrorValidacion(string campo, string mensaje) =>
        new(false, "Errores de validación.", ResultTipo.ErrorValidacion,
            new Dictionary<string, string[]> { [campo] = [mensaje] });
    public static Result ErrorValidacion(IReadOnlyDictionary<string, string[]> errores) =>
        new(false, "Errores de validación.", ResultTipo.ErrorValidacion, errores);
    public static Result NoAutorizado(string? mensaje = null) =>
        new(false, mensaje ?? "No autenticado.", ResultTipo.NoAutorizado);
    public static Result NoPermitido(string? mensaje = null) =>
        new(false, mensaje ?? "No tiene permisos para realizar esta acción.", ResultTipo.NoPermitido);

    public static Result<T> Exito<T>(T valor) => Result<T>.Exito(valor);
    public static Result<T> Fallo<T>(string error) => Result<T>.Fallo(error);
    public static Result<T> NoEncontrado<T>(string mensaje) => Result<T>.NoEncontrado(mensaje);
    public static Result<T> NoEncontrado<T>(string entidad, Guid id) => Result<T>.NoEncontrado(entidad, id);
    public static Result<T> ErrorValidacion<T>(string campo, string mensaje) => Result<T>.ErrorValidacion(campo, mensaje);
    public static Result<T> ErrorValidacion<T>(IReadOnlyDictionary<string, string[]> errores) => Result<T>.ErrorValidacion(errores);
    public static Result<T> NoAutorizado<T>(string? mensaje = null) => Result<T>.NoAutorizado(mensaje);
    public static Result<T> NoPermitido<T>(string? mensaje = null) => Result<T>.NoPermitido(mensaje);
}

public sealed class Result<T> : Result
{
    public T? Valor { get; }

    private Result(bool esExitoso, T? valor, string error, ResultTipo tipo, IReadOnlyDictionary<string, string[]>? errores = null)
        : base(esExitoso, error, tipo, errores)
    {
        Valor = valor;
    }

    public static Result<T> Exito(T valor) => new(true, valor, string.Empty, ResultTipo.Exito);
    public new static Result<T> Fallo(string error) => new(false, default, error, ResultTipo.ErrorNegocio);
    public new static Result<T> NoEncontrado(string mensaje) => new(false, default, mensaje, ResultTipo.NoEncontrado);
    public new static Result<T> NoEncontrado(string entidad, Guid id) =>
        new(false, default, $"{entidad} con ID '{id}' no fue encontrado.", ResultTipo.NoEncontrado);
    public new static Result<T> ErrorValidacion(string campo, string mensaje) =>
        new(false, default, "Errores de validación.", ResultTipo.ErrorValidacion,
            new Dictionary<string, string[]> { [campo] = [mensaje] });
    public new static Result<T> ErrorValidacion(IReadOnlyDictionary<string, string[]> errores) =>
        new(false, default, "Errores de validación.", ResultTipo.ErrorValidacion, errores);
    public new static Result<T> NoAutorizado(string? mensaje = null) =>
        new(false, default, mensaje ?? "No autenticado.", ResultTipo.NoAutorizado);
    public new static Result<T> NoPermitido(string? mensaje = null) =>
        new(false, default, mensaje ?? "No tiene permisos para realizar esta acción.", ResultTipo.NoPermitido);
}
