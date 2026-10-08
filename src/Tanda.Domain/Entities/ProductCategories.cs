namespace Tanda.Domain.Entities;

public static class ProductCategories
{
    public const string Cakes = "Tortas";
    public const string Desserts = "Postres";
    public const string SweetCakesAndBreads = "Ponqués y panes dulces";
    public const string SavorySnacks = "Pasabocas de sal";
    public const string CookiesAndCandyTable = "Galletería y mesa de dulces";

    public static IReadOnlyList<string> All { get; } =
    [
        Cakes,
        Desserts,
        SweetCakesAndBreads,
        SavorySnacks,
        CookiesAndCandyTable
    ];

    public static bool Contains(string? value) => value is not null && All.Contains(value);
}
