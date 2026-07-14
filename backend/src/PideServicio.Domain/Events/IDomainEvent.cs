namespace PideServicio.Domain.Events;

public interface IDomainEvent
{
    DateTimeOffset OcurridoEn { get; }
}
