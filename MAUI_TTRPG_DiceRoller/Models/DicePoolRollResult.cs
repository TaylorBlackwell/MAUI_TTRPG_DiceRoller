namespace MAUI_TTRPG_DiceRoller.Models;

public class DicePoolRollResult
{
    public DiceType DiceType { get; set; }
    public int NumberOfDice { get; set; }
    public int Modifier { get; set; }
    public List<int> IndividualRolls { get; set; } = [];
    public int Subtotal { get; set; }

    public string DisplayText => $"{NumberOfDice}d{(int)DiceType}{(Modifier != 0 ? $"{(Modifier > 0 ? "+" : "")}{Modifier}" : "")}";
    public string RollsDisplay => string.Join(", ", IndividualRolls);
}
