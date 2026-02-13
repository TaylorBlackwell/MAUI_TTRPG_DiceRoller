using MAUI_TTRPG_DiceRoller.Models;
using System.Text.RegularExpressions;

namespace MAUI_TTRPG_DiceRoller.Services;

public partial class DiceNotationParser
{
    [GeneratedRegex(@"^(\d+)d(\d+)(?:kh(\d+))?(?:kl(\d+))?(?:([\+\-])(\d+))?$", RegexOptions.IgnoreCase)]
    private static partial Regex DiceNotationRegex();

    public bool TryParse(string notation, out DiceType diceType, out int numberOfDice, out int modifier, out int keepHighest, out int keepLowest)
    {
        diceType = DiceType.D20;
        numberOfDice = 1;
        modifier = 0;
        keepHighest = 0;
        keepLowest = 0;

        if (string.IsNullOrWhiteSpace(notation))
        {
            return false;
        }

        var match = DiceNotationRegex().Match(notation.Trim());
        if (!match.Success)
        {
            return false;
        }

        numberOfDice = int.Parse(match.Groups[1].Value);
        var diceSize = int.Parse(match.Groups[2].Value);

        if (match.Groups[3].Success)
        {
            keepHighest = int.Parse(match.Groups[3].Value);
        }

        if (match.Groups[4].Success)
        {
            keepLowest = int.Parse(match.Groups[4].Value);
        }

        if (match.Groups[5].Success && match.Groups[6].Success)
        {
            var sign = match.Groups[5].Value;
            var modValue = int.Parse(match.Groups[6].Value);
            modifier = sign == "+" ? modValue : -modValue;
        }

        diceType = diceSize switch
        {
            4 => DiceType.D4,
            6 => DiceType.D6,
            8 => DiceType.D8,
            10 => DiceType.D10,
            12 => DiceType.D12,
            20 => DiceType.D20,
            100 => DiceType.D100,
            1000 => DiceType.D1000,
            _ => DiceType.D20
        };

        if (diceSize != 4 && diceSize != 6 && diceSize != 8 && diceSize != 10 && 
            diceSize != 12 && diceSize != 20 && diceSize != 100 && diceSize != 1000)
        {
            return false;
        }

        return true;
    }

    public string GetExampleNotations()
    {
        return "Examples: 1d20+5, 2d6-1, 4d6kh3, 3d8kl2+2";
    }
}
