using Kalma.Application.Partes;
using Kalma.Domain.Partes;
using Xunit;

namespace Kalma.Application.Tests;

public class RegistrarParteTests
{
    private readonly InMemoryParts parts = new();
    private readonly Membership membership = new();
    private readonly Guid space = Guid.NewGuid();
    private readonly Guid author = Guid.NewGuid();

    private RegistrarParteRequest Request(Guid id) => new(
        id, space, author,
        new DateTimeOffset(2026, 9, 28, 14, 5, 0, TimeSpan.FromHours(2)),
        Tramo.Mediodia, Valoracion.Bien, Valoracion.Regular,
        EstadoDeAnimo.Tranquilo, Valoracion.Bien, Valoracion.NoAplica, Medicacion.Si);

    [Fact]
    public async Task Member_can_register_but_retry_with_same_id_does_not_duplicate()
    {
        membership.Allowed = true;
        var useCase = new RegistrarParte(parts, membership);
        var id = Guid.NewGuid();
        Assert.Equal(RegistrationResult.Registered, await useCase.Execute(Request(id)));
        Assert.Equal(RegistrationResult.AlreadyExists, await useCase.Execute(Request(id)));
        Assert.Single(parts.Saved);
        Assert.Equal(id, parts.Saved[0].Id);
    }

    [Fact]
    public async Task Nonmember_cannot_register_even_when_id_already_exists()
    {
        membership.Allowed = true;
        var useCase = new RegistrarParte(parts, membership);
        var request = Request(Guid.NewGuid());
        await useCase.Execute(request);
        membership.Allowed = false;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.Execute(request));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.Execute(Request(Guid.NewGuid())));
        Assert.Single(parts.Saved);
    }

    private sealed class Membership : IComprobadorMembresia
    {
        public bool Allowed { get; set; }
        public Task<bool> EsMiembro(Guid accountId, Guid spaceId) => Task.FromResult(Allowed);
    }

    private sealed class InMemoryParts : IRepositorioPartes
    {
        public List<Parte> Saved { get; } = [];
        public Task<bool> Existe(Guid id) => Task.FromResult(Saved.Exists(p => p.Id == id));
        public Task Guardar(Parte part) { Saved.Add(part); return Task.CompletedTask; }
    }
}
