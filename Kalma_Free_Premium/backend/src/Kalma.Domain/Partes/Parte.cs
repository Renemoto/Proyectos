namespace Kalma.Domain.Partes;

public enum Valoracion { Bien, Regular, Mal, NoAplica }
public enum EstadoDeAnimo { Alegre, Tranquilo, Comunicativo, Triste, Nervioso, Adormilado }
public enum Medicacion { Si, No, NoAplica }

public sealed class Parte
{
    private Parte(Guid id, Guid spaceId, Guid authorId, PeriodoTramo period,
        Valoracion alimentacion, Valoracion descanso, EstadoDeAnimo estadoDeAnimo,
        Valoracion higiene, Valoracion actividad, Medicacion medicacion,
        string? note, Guid? correctsId)
    {
        Id = id;
        SpaceId = spaceId;
        AuthorId = authorId;
        Period = period;
        Alimentacion = alimentacion;
        Descanso = descanso;
        EstadoDeAnimo = estadoDeAnimo;
        Higiene = higiene;
        Actividad = actividad;
        Medicacion = medicacion;
        Note = note;
        CorrectsId = correctsId;
    }

    public Guid Id { get; }
    public Guid SpaceId { get; }
    public Guid AuthorId { get; }
    public PeriodoTramo Period { get; }
    public DateTimeOffset OccurredAt => Period.OccurredAt;
    public DateOnly CareDay => Period.CareDay;
    public Tramo Tramo => Period.Tramo;
    public Valoracion Alimentacion { get; }
    public Valoracion Descanso { get; }
    public EstadoDeAnimo EstadoDeAnimo { get; }
    public Valoracion Higiene { get; }
    public Valoracion Actividad { get; }
    public Medicacion Medicacion { get; }
    public string? Note { get; }
    public Guid? CorrectsId { get; }

    public static Parte Registrar(Guid id, Guid spaceId, Guid authorId,
        DateTimeOffset occurredAt, Tramo tramo, Valoracion alimentacion,
        Valoracion descanso, EstadoDeAnimo estadoDeAnimo, Valoracion higiene,
        Valoracion actividad, Medicacion medicacion, string? note = null,
        Guid? correctsId = null)
    {
        if (id == Guid.Empty) throw new ArgumentException("An ID is required.", nameof(id));
        if (spaceId == Guid.Empty) throw new ArgumentException("A space ID is required.", nameof(spaceId));
        if (authorId == Guid.Empty) throw new ArgumentException("An author ID is required.", nameof(authorId));
        if (!tramo.IsValid) throw new ArgumentOutOfRangeException(nameof(tramo));
        if (!Enum.IsDefined(alimentacion)) throw new ArgumentOutOfRangeException(nameof(alimentacion));
        if (!Enum.IsDefined(descanso)) throw new ArgumentOutOfRangeException(nameof(descanso));
        if (!Enum.IsDefined(estadoDeAnimo)) throw new ArgumentOutOfRangeException(nameof(estadoDeAnimo));
        if (!Enum.IsDefined(higiene)) throw new ArgumentOutOfRangeException(nameof(higiene));
        if (!Enum.IsDefined(actividad)) throw new ArgumentOutOfRangeException(nameof(actividad));
        if (!Enum.IsDefined(medicacion)) throw new ArgumentOutOfRangeException(nameof(medicacion));
        if (note?.Length > 500) throw new ArgumentException("Note exceeds 500 characters.", nameof(note));
        if (correctsId == Guid.Empty || correctsId == id)
            throw new ArgumentException("A correction must reference another part.", nameof(correctsId));

        var period = Tramo.ForInstant(occurredAt);
        if (period.Tramo != tramo) throw new ArgumentException("Tramo does not match the Madrid time.", nameof(tramo));
        return new Parte(id, spaceId, authorId, period, alimentacion, descanso,
            estadoDeAnimo, higiene, actividad, medicacion, note, correctsId);
    }
}
