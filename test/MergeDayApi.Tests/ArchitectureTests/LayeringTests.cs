namespace MergeDayApi.Tests.ArchitectureTests;

using System.Reflection;
using MergeDayApi.Tests.Infrastructure;
using NetArchTest.Rules;

[Trait("Category", TestCategories.Architecture)]
public class LayeringTests
{
    private readonly Assembly _assembly = typeof(Endpoints.IEndpoint).Assembly;

    private readonly string _features = NamespaceRegexHelpers.ExactOrChildNamespace("MergeDayApi.Features.Absences");
    private readonly string _domain = NamespaceRegexHelpers.ExactOrChildNamespace("MergeDayApi.Domain");
    private readonly string _endpoints = NamespaceRegexHelpers.ExactOrChildNamespace("MergeDayApi.Endpoints");

    [Fact]
    public void Domain_ShouldNotDependOnOtherLayers()
    {
        // Act
        var result = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespace(_domain)
            .Should()
            .NotHaveDependencyOn(_features)
            .And()
            .NotHaveDependencyOn(_endpoints)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"Domain layer should not depend on other layers. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Endpoints_ShouldFollowNamingConvention()
    {
        // Arrange & Act
        var result = Types.InAssembly(_assembly)
            .That()
            .ImplementInterface(typeof(Endpoints.IEndpoint))
            .Should()
            .HaveNameEndingWith("Endpoint")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful,
            $"All IEndpoint implementations should end with 'Endpoint'. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Handlers_ShouldBeStaticMethods()
    {
        // Get all types in the Features namespace
        var featureTypes = Types.InAssembly(_assembly)
            .That()
            .ResideInNamespaceMatching(_features)
            .GetTypes();

        // Find all Handler methods
        var handlerMethods = featureTypes
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(m => m.Name == "Handler")
            .ToList();

        Assert.NotEmpty(handlerMethods);
        Assert.All(handlerMethods, m => Assert.True(m.IsStatic, $"Handler method in {m.DeclaringType?.Name} should be static"));
    }
}
