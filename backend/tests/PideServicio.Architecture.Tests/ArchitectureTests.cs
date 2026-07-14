namespace PideServicio.Architecture.Tests;

using NetArchTest.Rules;
using Xunit;

public class DependenciasEntreCapasTests
{
    private const string NsDomain = "PideServicio.Domain";
    private const string NsApplication = "PideServicio.Application";
    private const string NsInfrastructure = "PideServicio.Infrastructure";
    private const string NsPersistence = "PideServicio.Persistence";
    private const string NsApi = "PideServicio.Api";

    [Fact]
    public void Domain_NoDebeDependeDe_NingunaOtraCapa()
    {
        var resultado = Types.InAssembly(typeof(PideServicio.Domain.Common.BaseEntity).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(NsApplication, NsInfrastructure, NsPersistence, NsApi)
            .GetResult();

        Assert.True(resultado.IsSuccessful,
            $"Domain depende de capas no permitidas: {string.Join(", ", resultado.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Application_NoDebeDependeDe_InfrastructurePersistenceNiApi()
    {
        var resultado = Types.InAssembly(typeof(PideServicio.Application.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(NsInfrastructure, NsPersistence, NsApi)
            .GetResult();

        Assert.True(resultado.IsSuccessful,
            $"Application depende de capas no permitidas: {string.Join(", ", resultado.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Infrastructure_NoDebeDependeDe_PersistenceNiApi()
    {
        var resultado = Types.InAssembly(typeof(PideServicio.Infrastructure.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(NsPersistence, NsApi)
            .GetResult();

        Assert.True(resultado.IsSuccessful,
            $"Infrastructure depende de capas no permitidas: {string.Join(", ", resultado.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Persistence_NoDebeDependeDe_InfrastructureNiApi()
    {
        var resultado = Types.InAssembly(typeof(PideServicio.Persistence.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(NsInfrastructure, NsApi)
            .GetResult();

        Assert.True(resultado.IsSuccessful,
            $"Persistence depende de capas no permitidas: {string.Join(", ", resultado.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Domain_Excepciones_DebenExtenderDomainException()
    {
        var resultado = Types.InAssembly(typeof(PideServicio.Domain.Common.BaseEntity).Assembly)
            .That()
            .ResideInNamespace($"{NsDomain}.Exceptions")
            .And()
            .AreNotAbstract()
            .Should()
            .Inherit(typeof(PideServicio.Domain.Exceptions.DomainException))
            .GetResult();

        Assert.True(resultado.IsSuccessful,
            $"Excepciones del dominio que no heredan DomainException: {string.Join(", ", resultado.FailingTypeNames ?? [])}");
    }
}
