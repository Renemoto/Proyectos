using Kalma.Domain.Partes;
using Xunit;

namespace Kalma.Domain.Tests;

public class ParteTests
{
    private static readonly Guid SpaceId = Guid.NewGuid();
    private static readonly Guid AuthorId = Guid.NewGuid();

    private static Parte Create(
        DateTimeOffset? occurredAt = null,
        Tramo? tramo = null,
        string? note = null,
        Guid? correctsId = null) =>
        Parte.Registrar(Guid.NewGuid(), SpaceId, AuthorId,
            occurredAt ?? new DateTimeOffset(2026, 9, 28, 14, 5, 0, TimeSpan.FromHours(2)),
            tramo ?? Tramo.Mediodia,
            Valoracion.Bien, Valoracion.Regular, EstadoDeAnimo.Tranquilo,
            Valoracion.Bien, Valoracion.NoAplica, Medicacion.Si, note, correctsId);

    [Fact]
    public void Registration_preserves_required_values_and_optional_note()
    {
        var part = Create();
        Assert.Equal(SpaceId, part.SpaceId);
        Assert.Equal(AuthorId, part.AuthorId);
        Assert.Equal(Valoracion.NoAplica, part.Actividad);
        Assert.Equal(Medicacion.Si, part.Medicacion);
        Assert.Null(part.Note);
        Assert.Equal(Tramo.Mediodia, part.Tramo);
        Assert.Equal(new DateOnly(2026, 9, 28), part.CareDay);
    }

    [Fact]
    public void Required_values_reject_undefined_options()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Parte.Registrar(
            Guid.NewGuid(), SpaceId, AuthorId, DateTimeOffset.UtcNow, Tramo.Manana,
            (Valoracion)99, Valoracion.Bien, EstadoDeAnimo.Alegre,
            Valoracion.Bien, Valoracion.Bien, Medicacion.No));
        Assert.Throws<ArgumentOutOfRangeException>(() => Parte.Registrar(
            Guid.NewGuid(), SpaceId, AuthorId, DateTimeOffset.UtcNow, Tramo.Manana,
            Valoracion.Bien, Valoracion.Bien, (EstadoDeAnimo)99,
            Valoracion.Bien, Valoracion.Bien, Medicacion.No));
        Assert.Throws<ArgumentOutOfRangeException>(() => Parte.Registrar(
            Guid.NewGuid(), SpaceId, AuthorId, DateTimeOffset.UtcNow, Tramo.Manana,
            Valoracion.Bien, Valoracion.Bien, EstadoDeAnimo.Alegre,
            Valoracion.Bien, Valoracion.Bien, (Medicacion)99));
    }

    [Fact]
    public void Invalid_tramo_and_mismatched_period_are_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(tramo: default(Tramo)));
        Assert.Throws<ArgumentException>(() => Create(tramo: Tramo.Tarde));
    }

    [Theory]
    [InlineData(6, "Noche", 27)]
    [InlineData(7, "Manana", 28)]
    [InlineData(12, "Mediodia", 28)]
    [InlineData(16, "Tarde", 28)]
    [InlineData(20, "Noche", 28)]
    public void Madrid_boundaries_determine_care_day(int hour, string expected, int day)
    {
        var instant = new DateTimeOffset(2026, 9, 28, hour, 0, 0, TimeSpan.FromHours(2));
        var period = Tramo.ForInstant(instant);
        Assert.Equal(expected, period.Tramo.ToString());
        Assert.Equal(new DateOnly(2026, 9, day), period.CareDay);
        Assert.Equal(instant, period.OccurredAt);
    }

    [Fact]
    public void Madrid_timezone_is_used_instead_of_supplied_offset()
    {
        var period = Tramo.ForInstant(new DateTimeOffset(2026, 9, 28, 0, 30, 0, TimeSpan.Zero));
        Assert.Equal(Tramo.Noche, period.Tramo);
        Assert.Equal(new DateOnly(2026, 9, 27), period.CareDay);
    }

    [Fact]
    public void Note_has_500_character_limit()
    {
        Assert.Equal(500, Create(note: new string('x', 500)).Note!.Length);
        Assert.Throws<ArgumentException>(() => Create(note: new string('x', 501)));
    }

    [Fact]
    public void Correction_is_a_new_immutable_part_referencing_the_original()
    {
        var original = Create();
        var correction = Create(correctsId: original.Id);
        Assert.NotEqual(original.Id, correction.Id);
        Assert.Equal(original.Id, correction.CorrectsId);
        Assert.Null(original.CorrectsId);
        Assert.Throws<ArgumentException>(() => Parte.Registrar(
            original.Id, SpaceId, AuthorId, original.OccurredAt, original.Tramo,
            original.Alimentacion, original.Descanso, original.EstadoDeAnimo,
            original.Higiene, original.Actividad, original.Medicacion, correctsId: original.Id));
        Assert.All(typeof(Parte).GetProperties(), property => Assert.False(property.CanWrite));
    }
}
