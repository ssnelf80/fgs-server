namespace FGS.Adapters.RandomNameGenerator;

public class RandomNameGenerator
{
    private static readonly IReadOnlyList<string> Names =
    [
        "Alice", "Bella", "Chloe", "Daisy", "Emma", 
        "Fiona", "Grace", "Hazel", "Ivy", "Jade", 
        "Kira", "Lily", "Maya", "Nora", "Olivia", 
        "Piper", "Quinn", "Ruby", "Stella", "Tessa", 
        "Una", "Violet", "Willow", "Xenia", "Yara", "Zoe"
    ]; 
    
    public static IReadOnlyList<string> GetUniqueNames(int count)
    {
        if (count <= 26)
            return Names.Take(count).ToList();
        return Enumerable.Range(0, count).Select(x => $"{Names[x % 26]}{x / 26}").ToList();
    }
}