namespace PideServicio.Infrastructure.Services;

using PideServicio.Application.Common.Interfaces;

public sealed class DateTimeService : IDateTimeService
{
    public DateTimeOffset AhoraUtc => DateTimeOffset.UtcNow;
    public DateOnly HoyUtc => DateOnly.FromDateTime(DateTime.UtcNow);
}
