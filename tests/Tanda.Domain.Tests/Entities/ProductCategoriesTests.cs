using FluentAssertions;
using Xunit;
using Tanda.Domain.Entities;

namespace Tanda.Domain.Tests.Entities;

/// <summary>
/// Pruebas para la validación de categorías en <see cref="ProductCategories"/>.
/// </summary>
public sealed class ProductCategoriesTests
{
    [Fact]
    public void Contiene_CadaCategoriaDefinida_RetornaTrue()
    {
        foreach (var category in ProductCategories.All)
        {
            ProductCategories.Contains(category).Should().BeTrue();
        }
    }

    [Fact]
    public void Contiene_CategoriaInexistente_RetornaFalse()
    {
        ProductCategories.Contains("Categoria inexistente").Should().BeFalse();
    }

    [Fact]
    public void Contiene_CadenaVaciaONull_RetornaFalse()
    {
        ProductCategories.Contains(string.Empty).Should().BeFalse();
        ProductCategories.Contains(null).Should().BeFalse();
    }

    [Fact]
    public void Contiene_CategoriaConDistintaCapitalizacion_RetornaFalse()
    {
        // Usamos la categoría 'Tortas' cambiando la capitalización para dejar explícito
        // que la comparación distingue mayúsculas/minúsculas.
        var original = ProductCategories.Cakes; // "Tortas"
        var differentCase = original.ToLowerInvariant();

        ProductCategories.Contains(differentCase).Should().BeFalse();
    }
}