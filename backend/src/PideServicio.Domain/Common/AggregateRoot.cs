namespace PideServicio.Domain.Common;

using PideServicio.Domain.Events;
using PideServicio.Domain.Interfaces;

public abstract class AggregateRoot : SoftDeletableEntity, IAggregateRoot
{
    private List<IDomainEvent>? _eventos;

    public IReadOnlyList<IDomainEvent> ObtenerEventos() => _eventos?.AsReadOnly() ?? [];

    public void LimpiarEventos() => _eventos?.Clear();

    protected void AgregarEvento(IDomainEvent evento)
    {
        _eventos ??= [];
        _eventos.Add(evento);
    }
}
