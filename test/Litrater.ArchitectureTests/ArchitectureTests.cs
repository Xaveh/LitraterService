using System.Reflection;
using NetArchTest.Rules;
using Shouldly;

namespace Litrater.ArchitectureTests;

public class ArchitectureTests
{
    private const string ApplicationNamespace = "Litrater.Application";
    private const string DomainNamespace = "Litrater.Domain";
    private const string InfrastructureNamespace = "Litrater.Infrastructure";
    private const string PresentationNamespace = "Litrater.Presentation";

    [Fact]
    public void Domain_ShouldNotHaveDependency_OnOtherProjects()
    {
        // Arrange
        var domainAssembly = Assembly.Load(DomainNamespace);

        var otherProjects = new[] { ApplicationNamespace, InfrastructureNamespace, PresentationNamespace };

        // Act
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Application_ShouldNotHaveDependency_OnOtherProjects()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);

        var otherProjects = new[] { InfrastructureNamespace, PresentationNamespace };

        // Act
        var result = Types.InAssembly(applicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldNotHaveDependency_OnOtherProjects()
    {
        // Arrange
        var infrastructureAssembly = Assembly.Load(InfrastructureNamespace);

        var otherProjects = new[] { PresentationNamespace };

        // Act
        var result = Types.InAssembly(infrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Application_ShouldHaveDependency_OnDomain()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);

        // Act
        var result = Types.InAssembly(applicationAssembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void AllHandlers_ShouldBeInternalSealed()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);

        // Act
        var handlers = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Handler", StringComparison.Ordinal) && t.IsClass)
            .ToList();

        var invalidHandlers = handlers
            .Where(h => !h.IsNotPublic || !h.IsSealed)
            .ToList();

        // Assert
        invalidHandlers.ShouldBeEmpty(
            $"The following handlers are not internal sealed: {string.Join(", ", invalidHandlers.Select(h => h.FullName))}");
    }

    [Fact]
    public void AllValidators_ShouldBeInternalSealed()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);

        // Act
        var validators = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Validator", StringComparison.Ordinal) && t.IsClass)
            .ToList();

        var invalidValidators = validators
            .Where(v => !v.IsNotPublic || !v.IsSealed)
            .ToList();

        // Assert
        invalidValidators.ShouldBeEmpty(
            $"The following validators are not internal sealed: {string.Join(", ", invalidValidators.Select(v => v.FullName))}");
    }

    [Fact]
    public void AllDomainEntities_ShouldNotHavePublicSetters()
    {
        // Arrange
        var domainAssembly = Assembly.Load(DomainNamespace);

        // Act
        var entities = domainAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                        t.Namespace?.StartsWith(DomainNamespace, StringComparison.Ordinal) == true &&
                        !IsRecord(t))
            .ToList();

        var propertiesWithPublicSetters = new List<string>();

        foreach (var entity in entities)
        {
            var publicSetterProperties = entity.GetProperties()
                .Where(p => p.GetSetMethod()?.IsPublic == true)
                .ToList();

            foreach (var prop in publicSetterProperties)
            {
                propertiesWithPublicSetters.Add($"{entity.FullName}.{prop.Name}");
            }
        }

        // Assert
        propertiesWithPublicSetters.ShouldBeEmpty(
            $"The following properties have public setters: {string.Join(", ", propertiesWithPublicSetters)}");
    }

    [Fact]
    public void AllCommands_ShouldBePublicSealedRecord()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);

        // Act
        var commands = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Command", StringComparison.Ordinal) && t is { IsClass: true, IsAbstract: false })
            .ToList();

        var invalidCommands = commands
            .Where(c => !c.IsPublic || !c.IsSealed || !IsRecord(c))
            .ToList();

        // Assert
        invalidCommands.ShouldBeEmpty(
            $"The following commands are not public sealed records: {string.Join(", ", invalidCommands.Select(c => c.FullName))}");
    }

    [Fact]
    public void AllQueries_ShouldBePublicRecord()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);

        // Act
        var queries = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Query", StringComparison.Ordinal) && t is { IsClass: true, IsAbstract: false })
            .ToList();

        var invalidQueries = queries
            .Where(q => !q.IsPublic || !IsRecord(q))
            .ToList();

        // Assert
        invalidQueries.ShouldBeEmpty(
            $"The following queries are not public records: {string.Join(", ", invalidQueries.Select(q => q.FullName))}");
    }

    [Fact]
    public void AllDomainEntities_ShouldBeSealed()
    {
        // Arrange
        var domainAssembly = Assembly.Load(DomainNamespace);

        // Act
        var entities = domainAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                        t.Namespace?.StartsWith(DomainNamespace, StringComparison.Ordinal) == true &&
                        !t.Name.EndsWith("Base", StringComparison.Ordinal) && !t.Namespace.Contains("Common", StringComparison.Ordinal))
            .ToList();

        var unsealedEntities = entities
            .Where(e => !e.IsSealed)
            .ToList();

        // Assert
        unsealedEntities.ShouldBeEmpty(
            $"The following domain entities are not sealed: {string.Join(", ", unsealedEntities.Select(e => e.FullName))}");
    }

    private static bool IsRecord(Type type)
    {
        // Records have a compiler-generated EqualityContract property and PrintMembers method
        var hasEqualityContract = type.GetProperty("EqualityContract",
            BindingFlags.Instance | BindingFlags.NonPublic) != null;

        var hasPrintMembers = type.GetMethod("PrintMembers",
            BindingFlags.Instance | BindingFlags.NonPublic) != null;

        return hasEqualityContract && hasPrintMembers;
    }
}