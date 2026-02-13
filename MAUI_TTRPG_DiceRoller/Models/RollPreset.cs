namespace MAUI_TTRPG_DiceRoller.Models;

public class RollPreset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DiceType DiceType { get; set; }
    public int NumberOfDice { get; set; }
    public int Modifier { get; set; }
    public bool ExplodingDice { get; set; }
    public bool Advantage { get; set; }
    public bool Disadvantage { get; set; }
    public int KeepHighest { get; set; }
    public int KeepLowest { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string DisplayText => $"{Name}: {NumberOfDice}d{(int)DiceType}{(Modifier >= 0 ? "+" : "")}{Modifier}";
}
