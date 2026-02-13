namespace MAUI_TTRPG_DiceRoller.Models;

public class DiceRoll
{
    public DiceType DiceType { get; set; }
    public int NumberOfDice { get; set; }
    public int Modifier { get; set; }
    public List<int> IndividualRolls { get; set; } = [];
    public List<int> RemovedSixes { get; set; } = [];
    public List<int> DroppedDice { get; set; } = [];
    public int KeepHighest { get; set; }
    public int KeepLowest { get; set; }
    public int Total { get; set; }
    public DateTime RolledAt { get; set; }
    public bool IsPoolRoll { get; set; }
    public List<DicePoolRollResult> PoolResults { get; set; } = [];

    public bool HasAdvantage { get; set; }
    public bool HasDisadvantage { get; set; }
    public int? AdvantageRoll1 { get; set; }
    public int? AdvantageRoll2 { get; set; }

    public bool IsCriticalSuccess => DiceType == DiceType.D20 && IndividualRolls.Any(r => r == 20);
    public bool IsCriticalFailure => DiceType == DiceType.D20 && IndividualRolls.Any(r => r == 1);

    public string FormattedRoll => IsPoolRoll ? "Pool Roll" : $"{NumberOfDice}d{(int)DiceType}{(Modifier >= 0 ? "+" : "")}{Modifier}";
    public string IndividualRollsDisplay => string.Join(", ", IndividualRolls);

    public bool Roll1IsKept => HasAdvantage 
        ? AdvantageRoll1 >= AdvantageRoll2 
        : AdvantageRoll1 <= AdvantageRoll2;

    public bool Roll2IsKept => HasAdvantage 
        ? AdvantageRoll2 >= AdvantageRoll1 
        : AdvantageRoll2 <= AdvantageRoll1;

    public string Roll1Color => (HasAdvantage || HasDisadvantage) && AdvantageRoll1.HasValue && AdvantageRoll2.HasValue
        ? Roll1IsKept ? "#00AA00" : "#CC0000"
        : "#808080";

    public string Roll2Color => (HasAdvantage || HasDisadvantage) && AdvantageRoll1.HasValue && AdvantageRoll2.HasValue
        ? Roll2IsKept ? "#00AA00" : "#CC0000"
        : "#808080";
}
