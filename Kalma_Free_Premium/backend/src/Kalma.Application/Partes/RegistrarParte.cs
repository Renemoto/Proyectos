using Kalma.Domain.Partes;

namespace Kalma.Application.Partes;

public interface IRepositorioPartes
{
    Task<bool> Existe(Guid id);
    Task Guardar(Parte parte);
}

public interface IComprobadorMembresia
{
    Task<bool> EsMiembro(Guid accountId, Guid spaceId);
}

public sealed record RegistrarParteRequest(
    Guid Id, Guid SpaceId, Guid AuthorId, DateTimeOffset OccurredAt,
    Tramo Tramo, Valoracion Alimentacion, Valoracion Descanso,
    EstadoDeAnimo EstadoDeAnimo, Valoracion Higiene, Valoracion Actividad,
    Medicacion Medicacion, string? Note = null, Guid? CorrectsId = null);

public enum RegistrationResult { Registered, AlreadyExists }

public sealed class RegistrarParte(IRepositorioPartes parts, IComprobadorMembresia membership)
{
    public async Task<RegistrationResult> Execute(RegistrarParteRequest request)
    {
        if (!await membership.EsMiembro(request.AuthorId, request.SpaceId))
            throw new UnauthorizedAccessException("The account is not a member of the space.");

        if (await parts.Existe(request.Id)) return RegistrationResult.AlreadyExists;

        var part = Parte.Registrar(request.Id, request.SpaceId, request.AuthorId,
            request.OccurredAt, request.Tramo, request.Alimentacion, request.Descanso,
            request.EstadoDeAnimo, request.Higiene, request.Actividad,
            request.Medicacion, request.Note, request.CorrectsId);
        await parts.Guardar(part);
        return RegistrationResult.Registered;
    }
}
