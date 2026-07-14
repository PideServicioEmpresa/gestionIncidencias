namespace PideServicio.Application.Features.Evidencias.Commands.EliminarEvidencia;

using PideServicio.Application.Common.CQRS;

public sealed record EliminarEvidenciaCommand(Guid EvidenciaId) : ICommand;
