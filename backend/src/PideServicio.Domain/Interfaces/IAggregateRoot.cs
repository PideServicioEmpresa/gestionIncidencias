namespace PideServicio.Domain.Interfaces;

/// <summary>
/// Marca una entidad como raíz de agregado. Solo los aggregate roots
/// exponen operaciones que modifican el estado del agregado.
/// </summary>
public interface IAggregateRoot : IHasDomainEvents { }
