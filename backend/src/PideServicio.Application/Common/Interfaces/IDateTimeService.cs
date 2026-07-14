namespace PideServicio.Application.Common.Interfaces;

public interface IDateTimeService
{
    DateTimeOffset AhoraUtc { get; }
    DateOnly HoyUtc { get; }
}
