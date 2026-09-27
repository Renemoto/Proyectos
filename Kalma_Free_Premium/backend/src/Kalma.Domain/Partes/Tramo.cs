namespace Kalma.Domain.Partes;

public readonly record struct Tramo
{
    private readonly byte _value;
    private Tramo(byte value) => _value = value;

    public static Tramo Manana { get; } = new(1);
    public static Tramo Mediodia { get; } = new(2);
    public static Tramo Tarde { get; } = new(3);
    public static Tramo Noche { get; } = new(4);

    public bool IsValid => _value is >= 1 and <= 4;

    public override string ToString() => _value switch
    {
        1 => nameof(Manana), 2 => nameof(Mediodia),
        3 => nameof(Tarde), 4 => nameof(Noche), _ => "Invalid"
    };

    public static PeriodoTramo ForInstant(DateTimeOffset occurredAt)
    {
        var madrid = TimeZoneInfo.ConvertTime(occurredAt,
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid"));
        var hour = madrid.Hour;
        var tramo = hour switch
        {
            >= 7 and < 12 => Manana,
            >= 12 and < 16 => Mediodia,
            >= 16 and < 20 => Tarde,
            _ => Noche
        };
        var day = DateOnly.FromDateTime(madrid.DateTime);
        if (hour < 7) day = day.AddDays(-1);
        return new PeriodoTramo(occurredAt, day, tramo);
    }
}

public readonly record struct PeriodoTramo(DateTimeOffset OccurredAt, DateOnly CareDay, Tramo Tramo);
