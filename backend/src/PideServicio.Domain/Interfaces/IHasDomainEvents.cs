namespace PideServicio.Domain.Interfaces;

using PideServicio.Domain.Events;

public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> ObtenerEventos();
    void LimpiarEventos();
}
