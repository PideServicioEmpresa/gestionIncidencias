namespace PideServicio.Application.Features.Configuracion.Commands.UpdateParametro;

using PideServicio.Application.Common.CQRS;

public sealed record UpdateParametroCommand(
    string Clave,
    string NuevoValor,
    Guid? EmpresaId) : ICommand;
