namespace MAUI_TTRPG_DiceRoller.Models;

public class DicePoolEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DiceType DiceType { get; set; }
    public int NumberOfDice { get; set; }
    public int Modifier { get; set; }

    public string DisplayText => $"{NumberOfDice}d{(int)DiceType}" + (Modifier != 0 ? $"{(Modifier > 0 ? "+" : "")}{Modifier}" : "");
}
