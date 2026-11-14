namespace Tanda.Domain.Entities;

public static class SizePresets
{
    public const string Personal = "Personal";
    public const string SixToEightPeople = "6-8 personas";
    public const string EightToTenPeople = "8-10 personas";
    public const string TwelveToEighteenPeople = "12-18 personas";
    public const string TwentyToTwentyFourPeople = "20-24 personas";

    public static IReadOnlyList<string> All { get; } =
    [
        Personal,
        SixToEightPeople,
        EightToTenPeople,
        TwelveToEighteenPeople,
        TwentyToTwentyFourPeople
    ];

    public static bool Contains(string? value) => value is not null && All.Contains(value);
}
