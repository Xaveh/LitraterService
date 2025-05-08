using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;

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

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            PresentationNamespace
        };

        // Act
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotHaveDependency_OnOtherProjects()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            PresentationNamespace
        };

        // Act
        var result = Types.InAssembly(applicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldNotHaveDependency_OnOtherProjects()
    {
        // Arrange
        var infrastructureAssembly = Assembly.Load(InfrastructureNamespace);

        var otherProjects = new[]
        {
            PresentationNamespace
        };

        // Act
        var result = Types.InAssembly(infrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Presentation_ShouldNotHaveDependency_OnOtherProjects()
    {
        // Arrange
        var presentationAssembly = Assembly.Load(PresentationNamespace);

        var otherProjects = new[]
        {
            InfrastructureNamespace
        };

        // Act
        var result = Types.InAssembly(presentationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_ShouldHaveDependency_OnDomain()
    {
        // Arrange
        var applicationAssembly = Assembly.Load(ApplicationNamespace);

        // Act
        var result = Types.InAssembly(applicationAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Controllers_ShouldHaveDependency_OnMediatR()
    {
        // Arrange
        var presentationAssembly = Assembly.Load(PresentationNamespace);

        // Act
        var result = Types.InAssembly(presentationAssembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}